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
        private EventingBasicConsumer consumer;
        private BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private IBasicProperties props;

        private static readonly RabbitMQClient instance = new RabbitMQClient();

        private string correlationId = Guid.NewGuid().ToString();

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
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            Console.WriteLine(correlationId);
            Console.WriteLine(replyQueueName);
            props.CorrelationId = correlationId;
            // props.ReplyTo = replyQueueName;
            props.ReplyTo = replyQueueName;

            // consumer.Received += (model, ea) =>
            // {
            //     var body = ea.Body;
            //     var response = Encoding.UTF8.GetString(body);
            //     // if (ea.BasicProperties.CorrelationId == correlationId)
            //     // {
            //     //     respQueue.Add(response);
            //     // }
            // };
        }

        public Object GetCorrelateIDAndQueueName() {
            return new {id = props.CorrelationId, name = replyQueueName};
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

        public string SendMessage(string message)
        {

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };

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

        public Object SendMessageWithAsync(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: "rpc_queue",
                basicProperties: props,
                body: messageBytes);

            return new { id = props.CorrelationId, name = replyQueueName };
        }
    }
}