
using Core.nDTOs.nEvent;
using Core.nDTOs.nEvent.nEventItem;
using Core.nRabbitConnector;
using Core.nUtils.nJsonConverter;
using MicroServiceSample.nWebGraph.nValidatorManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;


namespace MicroServiceSample.nWebGraph
{ 
    public class cEventGraph 
    {
        protected cValidatorManager ValidatorManager { get; set; }
        protected cRabbitConnector RabbitConnector { get; set; }
        public cEventGraph()
            :base()
        {
            ValidatorManager = new cValidatorManager();
            RabbitConnector = new cRabbitConnector("localhost", "topic", "myuser", "mypassword");
            //cRabbitConnector = new cRabbitConnector("host.docker.internal", "topic", "myuser", "mypassword");
            RabbitConnector.Producer.Init(ExchangeType.Fanout);
        }

        public void Interpret(cBaseController _Controller)
        {
            if (_Controller.Events.ContainsKey("events"))
            {
                JToken __Events = _Controller.Events["events"];
                JArray __CommandItem = (JArray)__Events;

                foreach (var __EventItem in __Events)
                {
                    List<string> __ValidationErrors = ValidatorManager.Validate<cEventItem>((JObject)__EventItem);
                    if (__ValidationErrors.Count == 0)
                    {
                        RabbitConnector.Producer.Produce(__EventItem.ToString());
                    }
                    else
                    {
                        foreach (string __ValidationError in __ValidationErrors)
                        {
                            Console.WriteLine($"Error {__ValidationError}, message can not validate! message is :  {__EventItem.ToString()}");
                        }
                    }
                }

            }
        }

    }
}
