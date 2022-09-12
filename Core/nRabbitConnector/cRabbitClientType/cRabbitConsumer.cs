using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Core.nRabbitConnector.cRabbitClientType
{
    public class cRabbitConsumer
    {
        IMessageReceiver m_MessageReceiver { get; set; }
        string m_GroupId { get; set; }
        bool m_Stop { get; set; }
        private Thread ReceiverThread { get; set; }


        cRabbitConnector m_RabbitConnector { get; set; }
        ConnectionFactory m_Factory { get; set; }
        IConnection m_Connection { get; set; }
        IModel m_Channel { get; set; }

        string m_QueueName { get; set; }
        public cRabbitConsumer(cRabbitConnector _RabbitConnector)
        {
            m_RabbitConnector = _RabbitConnector;
            m_Stop = false;

        }

        public void Init(string _ExchangeType)
        {
            m_Factory = new ConnectionFactory() { HostName = m_RabbitConnector.RabbitServer, UserName = m_RabbitConnector.UserName, Password = m_RabbitConnector.Password };
            m_Connection = m_Factory.CreateConnection();
            m_Channel = m_Connection.CreateModel();


            if (_ExchangeType == ExchangeType.Fanout)
            {
                m_Channel.ExchangeDeclare(exchange: m_RabbitConnector.Channel, type: ExchangeType.Fanout);

                m_QueueName = m_Channel.QueueDeclare().QueueName;
                m_Channel.QueueBind(queue: m_QueueName,
                                  exchange: m_RabbitConnector.Channel,
                                  routingKey: "");
            }
            else
            {
                m_QueueName = m_RabbitConnector.Channel;
                m_Channel.QueueDeclare(queue: m_QueueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            }

        }

        public void StartListener(string _ExchangeType, IMessageReceiver _MessageReceiver)
        {
            Init(_ExchangeType);
            m_MessageReceiver = _MessageReceiver;
            ReceiverThread = new Thread(new ThreadStart(ReceiverThreadFunction));
            ReceiverThread.Start();
        }

        public void Stop()
        {
            m_Stop = true;
        }

        private void ReceiverThreadFunction()
        {
            EventingBasicConsumer __Consumer = new EventingBasicConsumer(m_Channel);
            __Consumer.Received += (_Model, _Ea) =>
            {
                byte[] __Body = _Ea.Body.ToArray();
                string __Message = Encoding.UTF8.GetString(__Body);
                if (m_MessageReceiver != null)
                {
                    m_MessageReceiver.ReceiveMessage(__Message);
                }
            };
            m_Channel.BasicConsume(queue: m_QueueName,
                                 autoAck: true,
                                 consumer: __Consumer);
        }
    }
}
