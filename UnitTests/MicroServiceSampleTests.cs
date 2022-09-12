using Core.nDTOs.nEvent.nEventItem;
using Core.nRabbitConnector;
using MicroServiceSample.nWebGraph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Unity.Injection;

namespace UnitTests
{
    [TestClass]
    public class MicroServiceSampleTests : cBaseTest, IMessageReceiver
    {
        static cRabbitConnector RabbitConnectorSender { get; set; }

        static cRabbitConnector RabbitConnectorReceiever { get; set; }
        static string RABBIT_TEST = "RABBIT_TEST";

        static string RabbitTestString = "";

        private bool ControlRabbit()
        {
            string __Output = "";
            using (Process __Process = new Process())
            {
                __Process.StartInfo.FileName = "docker";
                __Process.StartInfo.UseShellExecute = false;
                __Process.StartInfo.Arguments = "ps -a";
                __Process.StartInfo.RedirectStandardOutput = true;
                __Process.StartInfo.CreateNoWindow = true;
                __Process.StartInfo.RedirectStandardError = true;
                __Process.Start();


                StreamReader __Reader = __Process.StandardOutput;
                __Output = __Reader.ReadToEnd();


                __Process.WaitForExit();
            }

            Match __Match = Regex.Match(__Output, "rabbitmq");
            return __Match.Success;

        }



        public void ReceiveMessage(string _Message)
        {
            RabbitTestString = _Message;
        }

        [TestMethod]        
        public void Test1_RabbitTest_RabbitIsExistTest()
        {
            if (!ControlRabbit())
            {
                //throw new Exception("Öncelikle rabbit servisini ayaða kaldýrmanýz gerekiyor! \"docker-compose -f docker-compose.yml up\"");
                Assert.Fail("Öncelikle rabbit servisini ayaða kaldýrmanýz gerekiyor! \"docker-compose -f docker-compose.yml up\"");
            }
        }

        [TestMethod]
        public void Test2_RabbitTest_Consumer()
        {
            RabbitTestString = "";
            RabbitConnectorSender = new cRabbitConnector("127.0.0.1:5672", "topic", "myuser", "mypassword");
            ///
            /// Þuanda ayaða kalkan rabbit servisi ayný grup ID ile tek consumer üzerinden çalýþýyor.
            /// Birden fazla peþpeþe test sýrasýnda testler baþarýsýz çýktýðý için random bir grup oluþturuluyor.
            /// Bu gruba belirli bir süre istek olmayýnca rabbit tarafýndan düþürüldüðü için poroblem olmuyor.
            //
            RabbitConnectorSender.Consumer.StartListener(ExchangeType.Fanout, this);
            Thread.Sleep(10000);
        }


        [TestMethod]
        public void Test3_RabbitTest_ProduceAndControl()
        {
            RabbitConnectorReceiever = new cRabbitConnector("127.0.0.1:5672", "topic", "myuser", "mypassword");
            RabbitConnectorReceiever.Producer.Init(ExchangeType.Fanout);
            RabbitConnectorReceiever.Producer.Produce(RABBIT_TEST);

            int __Counter = 0;
            while(__Counter < 30)
            {
                if (RabbitTestString == RABBIT_TEST)
                {
                    break;
                }
                Thread.Sleep(1000);
                __Counter++;
            }

            Assert.AreEqual(RabbitTestString, RABBIT_TEST);
        }

        [TestMethod]
        public void Test4_EventTest()
        {
            cEventGraph __EventGraph = new cEventGraph();
            WebApiController __ForTest = new WebApiController(__EventGraph);
            JObject __JsonObject = new JObject();
            JArray __JsonArray = new JArray();
            string __SampleJSON = "{\"app\":\"277cdc8c-b0ea-460b-a7d2-592126f5bbb0\",\"type\":\"HOTEL_CREATE\",\"time\":\"2020-02-10T13:40:27.650Z\",\"isSucceeded\":true,\"meta\":{},\"user\":{\"isAuthenticated\":true,\"provider\":\"b2c-internal\",\"email\":\"test1@hotmail.com\",\"id\":1},\"attributes\":{\"hotelId\":1,\"hotelRegion\":\"Kýbrýs\",\"hotelName\":\"Rixos\"}}";
            __JsonArray.Add(JObject.Parse(__SampleJSON));
            __JsonObject["events"] = __JsonArray;
            __ForTest.Events = __JsonObject;
            __EventGraph.Interpret(__ForTest);

            int __Counter = 0;
            while (__Counter < 30)
            {
                if (RabbitTestString != "" && RabbitTestString != RABBIT_TEST)
                {
                    break;
                }
                Thread.Sleep(1000);
                __Counter++;
            }

            cEventItem __EventItem = JsonConvert.DeserializeObject<cEventItem>(RabbitTestString);


            Assert.AreEqual(__EventItem.app.ToString(), "277cdc8c-b0ea-460b-a7d2-592126f5bbb0");
        }
    }
}