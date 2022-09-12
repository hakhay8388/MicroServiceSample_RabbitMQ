using Core.nRabbitConnector;
using PostgresApp.nRabbitListener;
using RabbitMQ.Client;
using System.Text;

namespace PostgresApp
{
    class Program
    {
        static void Main(string[] args)
        {
            cRabbitListener __RabbitListener = new cRabbitListener();

            //cRabbitConnector __RabbitConnector = new cRabbitConnector("localhost", "topic", "myuser", "mypassword");
            cRabbitConnector __RabbitConnector = new cRabbitConnector("host.docker.internal", "topic", "myuser", "mypassword");
            
            __RabbitConnector.Consumer.StartListener(ExchangeType.Fanout, __RabbitListener);

            Console.WriteLine("Postgres App Started");
            string __Code = Console.ReadLine();

            while (__Code != "exit")
            {
                __Code = Console.ReadLine();
            }

            __RabbitConnector.Consumer.Stop();
        }
    }
}