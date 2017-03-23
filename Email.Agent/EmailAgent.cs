using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Email.Agent.Helpers;
using Email.Agent.TestData;
using Email.DataAccess.Repositories;
using ImapX;
using ImapX.Enums;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Email.Agent
{
    public class EmailAgent:IDisposable
    {

        //RabbitMQ connections and properties
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private IBasicProperties _properties;
        private string QueueName { get; }

        //

        public Guid AgentGuid { get; }

        private EventingBasicConsumer _consumer;

        private Regex searchFileExt = new Regex("^.*\\.(csv|txt|xls|xlsx|zip|rar)$");

        public  EmailAgent(string queueName, string host)
        {
            AgentGuid = Guid.NewGuid();
            QueueName = queueName;
            var factory = new ConnectionFactory() { HostName = host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.BasicQos(0, 1, false);
            _channel.QueueDeclare(queueName, true, false, false, null);
            AddListenerQueue();
        }

        private void AddListenerQueue()
        {
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (sender, args) =>
            {
                var body = args.Body;
                var emailClientCred = Encoding.UTF8.GetString(body);
                var data = JsonConvert.DeserializeObject<EmailData>(emailClientCred);
                var result = TestEmails(data);
                if (result.Result.Success)
                {
                    EmailAgentManager.SuccessRequests.Add(result);
                }
                else
                {
                    EmailAgentManager.BadRequests.Add(result);
                }
                _channel.BasicAck(args.DeliveryTag, false);
            };
            _channel.BasicConsume(QueueName, false, _consumer);
        }

        public EmailData TestEmails(EmailData data)
        {
            var client = new ImapClient(data.ImapServer, data.Port, data.Ssl);
            if (!client.Connect())
            {
                data.Result = new EmailLoadResult
                {
                    Result = $"{data.Email} connecting false at {DateTime.Now.ToLongDateString()} with agent: {AgentGuid}",
                    Success = false
                };
                return data;
            }
            if (client.Login(data.Login, data.Password))
            {
                var dt = new EmailHelpers().DateForEmailFiler(DateTime.Now.AddDays(-10));
                var d = client.Folders.Inbox.Search($"SINCE {dt}").Where(message => message.Attachments.Any(attachment => searchFileExt.IsMatch(attachment.FileName)));

                data.Result = new EmailLoadResult()
                {
                    Result = $"{data.Email} is successful authenticated at {DateTime.Now.ToLongDateString()}, mess: {string.Join(" / ", d.Select(message => message.Subject))} with agent: {AgentGuid}",
                    Success = true
                };
            }
            else
            {
                data.Result = new EmailLoadResult()
                {
                    Result = $"{data.Email} login false at {DateTime.Now.ToLongDateString()} with agent: {AgentGuid}",
                    Success = false
                };
            }
            return data;
        }

        private List<Message> CheckMessageToDownload(IEnumerable<Message> messages)
        {
            var result = messages.ToList();
            if (!result.Any())
            {
                return null;
            }
            using (var ctx = new DbUserRepository())
            {
                
            }

            return result;
        }

        public void Dispose()
        {
            _channel.Close(200, $"Application with id:{AgentGuid} shutdown at {DateTime.Now}");
            _connection.Close();
        }
    }

}
