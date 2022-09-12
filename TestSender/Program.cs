using Core.nRabbitConnector;
using RabbitMQ.Client;
using System.Text;

namespace MicroServiceSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sender Started");

            cRabbitConnector __RabbitConnector = new cRabbitConnector("localhost", "topic", "myuser", "mypassword");
            __RabbitConnector.Producer.Init(ExchangeType.Fanout);

            Console.Write("Message : ");
            string __Message = Console.ReadLine();

            while (__Message != "exit")
            {
                __RabbitConnector.Producer.Produce(__Message);
                Console.Write("Message : ");
                __Message = Console.ReadLine();
            }
        }
    }
}