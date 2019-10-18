using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RPCClient
{
    public class RPCClient
    {

        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;
        public RPCClient()
        {
            var factory = new ConnectionFactory() { HostName = "10.0.1.222", Port = 5672, UserName = "admin", Password = "admin", VirtualHost = "/" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            Console.WriteLine(correlationId);
            Console.WriteLine(replyQueueName);
            props.CorrelationId = correlationId;
            // props.ReplyTo = replyQueueName;
            props.ReplyTo = "nam";

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public string Call(string message)
        {
            channel.ExchangeDeclare(exchange: "topic_logs",
                                    type: "topic");

            var messageBytes = Encoding.UTF8.GetBytes(message);
            
            channel.BasicPublish(
                exchange: "topic_logs",
                routingKey: "abcd",
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }

    public class Rpc
    {
        public static void Main()
        {
            var rpcClient = new RPCClient();

            Console.WriteLine(" [x] Requesting fib(30)");
            var response = rpcClient.Call("30");

            Console.WriteLine(" [.] Got '{0}'", response);
            rpcClient.Close();
        }
    }

}
