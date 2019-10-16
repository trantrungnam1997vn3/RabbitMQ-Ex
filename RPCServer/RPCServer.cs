using System;
using System.Linq;
using System.Text;

using RabbitMQ.Client;

using RabbitMQ.Client.Events;

namespace RPCServer
{


    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "10.0.1.222", Port = 5672, UserName = "admin", Password = "admin", VirtualHost = "/" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
                // var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
                // var message = (args.Length > 1)
                //               ? string.Join(" ", args.Skip(1).ToArray())
                //               : "Hello World!";

                // var body = Encoding.UTF8.GetBytes(message);

                foreach (var bindingKey in args)
                {
                    channel.QueueBind(queue: "rpc_queue",
                                      exchange: "topic_logs",
                                      routingKey: bindingKey
                  );
                }

                Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");


                // channel.QueueDeclare(queue: "rpc_queue", durable: false,
                //   exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                // channel.BasicQos(10);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    Console.WriteLine(props.CorrelationId);
                    var replyProps = channel.CreateBasicProperties();
                    Console.WriteLine(replyProps);
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        // int n = int.Parse(message);
                        Console.WriteLine(" [.] fib({0})", message);
                        response = getHello(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
                    }
                    finally
                    {
                        Console.WriteLine(props.ReplyTo);
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }

                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static int fib(int n)
        {
            if (n == 0 || n == 1) return n;
            return fib(n - 1) + fib(n - 2);
        }

        private static String getHello(String name)
        {
            return "Hello" + name;
        }
    }
}
