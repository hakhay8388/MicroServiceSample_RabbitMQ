using Core.nRabbitConnector.cRabbitClientType;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Core.nRabbitConnector
{
    public class cRabbitConnector
    {
        public cRabbitProducer Producer { get; set; }
        public cRabbitConsumer Consumer { get; set; }


        public string RabbitServer { get; set; }
        public string Channel { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public cRabbitConnector(string _RabbitServer, string _Channel, string _UserName, string _Password)
        {
            RabbitServer = _RabbitServer;
            Channel = _Channel;
            UserName = _UserName;
            Password = _Password;
            Init();
        }

        private void Init()
        {
            Producer = new cRabbitProducer(this);
            Consumer = new cRabbitConsumer(this);
        }
      
        
    }
}
