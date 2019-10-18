using System;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Client_API.Service
{
    public class RabbitMQClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private string replyQueueName;
        private EventingBasicConsumer consumer, consumer2;
        private BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private IBasicProperties props, props2;

        private static readonly RabbitMQClient instance = new RabbitMQClient();

        private string correlationId = Guid.NewGuid().ToString();
        private string correlationId2 = Guid.NewGuid().ToString();

        public static RabbitMQClient Instance
        {
            get
            {
                return instance;
            }
        }

        private RabbitMQClient()
        {
            var factory = new ConnectionFactory() { HostName = "10.0.1.222", Port = 5672, UserName = "admin", Password = "admin", VirtualHost = "/" };
            connection = factory.CreateConnection();


            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "direct_logs", type: ExchangeType.Direct);
            replyQueueName = channel.QueueDeclare().QueueName;
            // channel.QueueBind(replyQueueName, "direct_logs", replyQueueName);

            consumer = new EventingBasicConsumer(channel);

            consumer2 = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            Console.WriteLine(correlationId);
            Console.WriteLine(replyQueueName);
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            // props2
            props2 = channel.CreateBasicProperties();
            props2.CorrelationId = correlationId2;
            props2.ReplyTo = replyQueueName;
        }

        public Object GetCorrelateIDAndQueueName()
        {
            return new { id = props.CorrelationId, name = replyQueueName };
        }

        public void SendMessageToQueue(string assign)
        {

            replyQueueName = channel.QueueDeclare().QueueName;

            replyQueueName = assign;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            Console.WriteLine(correlationId);
            Console.WriteLine(replyQueueName);
            props.CorrelationId = correlationId;
            // props.ReplyTo = replyQueueName;
            replyQueueName = "rpc_queue";
            props.ReplyTo = "rpc_queue";

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                Console.WriteLine(body);
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public Object SendMessageWithSync(string message)
        {

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                Console.WriteLine(ea.BasicProperties.CorrelationId);
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    Console.WriteLine("Receive 1");
                    respQueue.Add(response);
                }
            };

            consumer2.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                Console.WriteLine(ea.BasicProperties.CorrelationId);
                if (ea.BasicProperties.CorrelationId == correlationId2)
                {
                    Console.WriteLine("Receive 2");
                    respQueue.Add(response);
                }
            };


            var messageBytes = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                // exchange: "direct_logs",
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);

            channel.BasicPublish(
                // exchange: "direct_logs",
                exchange: "",
                routingKey: "rpc_queue2",
                basicProperties: props2,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);


            channel.BasicConsume(
                consumer: consumer2,
                queue: replyQueueName,
                autoAck: true);

            // var item = respQueue.Take();
            // Console.WriteLine("value: " + item);
            return respQueue.Take() + respQueue.Take();
            // return item;
        }

        public Object SendMessageWithAsync(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "direct_logs",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);

            channel.BasicPublish(
                exchange: "direct_logs",
                routingKey: "rpc_queue2",
                basicProperties: props2,
                body: messageBytes);

            return new { id = props.CorrelationId, name = replyQueueName };
        }
    }
}