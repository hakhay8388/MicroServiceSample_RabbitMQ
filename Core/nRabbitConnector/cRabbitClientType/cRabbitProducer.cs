using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Core.nRabbitConnector.cRabbitClientType
{
    public  class cRabbitProducer
    {
        cRabbitConnector m_RabbitConnector { get; set; }
        ConnectionFactory m_Factory { get; set; }
        IConnection m_Connection { get; set; }
        IModel m_Channel { get; set; }

        string m_ExchangeType { get; set; }
        public cRabbitProducer(cRabbitConnector _RabbitConnector)
        {
            m_RabbitConnector = _RabbitConnector;
        }

        public void Init(string _ExchangeType)
        {
            m_ExchangeType = _ExchangeType;
            m_Factory = new ConnectionFactory() { HostName = m_RabbitConnector.RabbitServer, UserName = m_RabbitConnector.UserName, Password = m_RabbitConnector.Password };
            m_Connection = m_Factory.CreateConnection();
            m_Channel = m_Connection.CreateModel();

            if (_ExchangeType == ExchangeType.Fanout)
            {
                m_Channel.ExchangeDeclare(exchange: m_RabbitConnector.Channel, type: ExchangeType.Fanout, durable: false, autoDelete: false);
            }
            else
            {
                m_Channel.QueueDeclare(queue: m_RabbitConnector.Channel,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            }

        }

        public void Produce(string _Value)
        {
            byte[] __Body = Encoding.UTF8.GetBytes(_Value);
            
            if (m_ExchangeType == ExchangeType.Fanout)
            {
                m_Channel.BasicPublish(exchange: m_RabbitConnector.Channel,
                              routingKey: "",
                              basicProperties: null,
                              body: __Body);
            }
            else
            {
                m_Channel.BasicPublish(exchange: "",
                               routingKey: m_RabbitConnector.Channel,
                               basicProperties: null,
                               body: __Body);
            }
        }
    }
}
