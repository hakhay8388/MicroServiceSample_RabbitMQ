﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Core.nDTOs.nEvent.nEventItem;
using Core.nUtils.nJsonConverter;
using Newtonsoft.Json;
using Core.nRabbitConnector;

namespace PostgresApp.nRabbitListener
{
    public class cRabbitListener : IMessageReceiver
    {
        public void ReceiveMessage(string _Message)
        {
            try
            {
                JsonSerializerSettings __Settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new cBadDateFixingConverter() },
                    DateParseHandling = DateParseHandling.None
                };
                cEventItem __EventItem = JsonConvert.DeserializeObject<cEventItem>(_Message, __Settings);
            }
            catch (Exception _Ex)
            {
                //                throw new Exception("Gelen veri çözümlenemiyor.");
            }

            Console.WriteLine("################################################################################################");
            Console.WriteLine("");
            Console.WriteLine(_Message);
            Console.WriteLine("");
            Console.WriteLine("################################################################################################");
        }
    }
}
