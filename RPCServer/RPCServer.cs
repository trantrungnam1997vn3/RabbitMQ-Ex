using System;
using System.Linq;
using System.Text;
using System.Threading;

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
                channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
                var queueName = channel.QueueDeclare(queue: "rpc_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null).QueueName;
                channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: "rpc_queue");
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");




                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    Console.WriteLine("Go1: " + props.CorrelationId);
                    Console.WriteLine(props.CorrelationId);
                    Console.WriteLine(props.ReplyTo);
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
                        // channel.BasicPublish(exchange: "direct_logs", routingKey: props.ReplyTo,
                        //   basicProperties: replyProps, body: responseBytes);

                        channel.BasicPublish(exchange: "direct_logs", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                    }
                };

                //consume2

                var quueueName2 = channel.QueueDeclare(queue: "rpc_queue2", durable: false,
                                  exclusive: false, autoDelete: false, arguments: null).QueueName;

                channel.QueueBind(queue: quueueName2, exchange: "direct_logs", routingKey: "rpc_queue2");
                channel.BasicQos(0, 1, false);
                var consumer2 = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue2",
                  autoAck: false, consumer: consumer2);

                consumer2.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    Console.WriteLine("Go2: " + props.CorrelationId);
                    Console.WriteLine(props.CorrelationId);
                    Console.WriteLine(props.ReplyTo);
                    var replyProps = channel.CreateBasicProperties();
                    Console.WriteLine(replyProps);
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);
                        // int n = int.Parse(message);
                        Console.WriteLine(" [.] fib({0})", message);
                        response = getHelloQueue2(message);
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
                        // channel.BasicPublish(exchange: "direct_logs", routingKey: props.ReplyTo,
                        //   basicProperties: replyProps, body: responseBytes);

                        channel.BasicPublish(exchange: "direct_logs", routingKey: props.ReplyTo,
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
            Console.WriteLine("Go 1");
            Thread.Sleep(2000);
            return "Hello" + name;
        }

        // Message for queue2
        private static String getHelloQueue2(String name)
        {
            Console.WriteLine("Go 2");
            Thread.Sleep(2000);
            return "Hello2" + name;
        }
    }
}
