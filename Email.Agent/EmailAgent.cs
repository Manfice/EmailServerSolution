using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
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

        private readonly Regex _searchFileExt = new Regex("^.*\\.(csv|txt|xls|xlsx|zip|rar)$");

        private const string _savePath = @"C:\DownloadedFiles\";

        //ImapX helpers
        private IList<Message> _messageList;
        private IEnumerable<string> _receivedMessagesUids;
        private Attachment _currentFile;
        private Message _currentMessage;


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
            CheckFilePath();
        }

        private static void CheckFilePath()
        {
            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }
        private void AddListenerQueue()
        {
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (sender, args) =>
            {
                var body = args.Body;
                var emailClientCred = Encoding.UTF8.GetString(body);
                var data = JsonConvert.DeserializeObject<EmailData>(emailClientCred);
                GetReceivedUids(data);
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
            try
            {
                client.Login(data.Login, data.Password);
                var dt = new EmailHelpers().DateForEmailFiler(data.InquireDate);
                _messageList = client.Folders.Inbox.Search($"SINCE {dt}").Where(message => message.Attachments.Any(attachment => _searchFileExt.IsMatch(attachment.FileName))).ToList();

                if (_messageList.Any())
                {
                    foreach (var message in _messageList)
                    {
                        if (_receivedMessagesUids.Contains(message.UId.ToString())) continue;
                        message.Download(MessageFetchMode.Attachments, true);
                        foreach (var attachment in _currentMessage.Attachments)
                        {
                            if (!attachment.Downloaded)
                            {
                                attachment.Download();

                            }
                            var dir = $"{_savePath}\\{data.Email}\\{DateTime.Today.ToLongDateString()}";
                            var path = Path.Combine(dir, attachment.FileName);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            if (!File.Exists(path))
                            {
                                File.WriteAllBytes(path, attachment.FileData);
                            }

                        }
                    }
                }

                data.Result = new EmailLoadResult()
                {
                    Result = $"{data.Email} is successful authenticated at {DateTime.Now.ToLongDateString()}, mess: {string.Join(" / ", _messageList.Select(message => message.Subject))} with agent: {AgentGuid}",
                    Success = true
                };

            }
            catch (Exception e)
            {
                data.Result = new EmailLoadResult()
                {
                    Result = $"{data.Email} has an exeption {e.Message} at {DateTime.Now.ToLongDateString()} with agent: {AgentGuid}",
                    Success = false
                };

            }

            return data;
        }

        private void GetReceivedUids(EmailData account)
        {
            using (var ctx = new DbMailRepository())
            {
                _receivedMessagesUids = ctx.GetReceivedUidsOnDate(account.InquireDate, account.Email)?? new List<string>();
            }
        }

        public void Dispose()
        {
            _channel.Close(200, $"Application with id:{AgentGuid} shutdown at {DateTime.Now}");
            _connection.Close();
        }
    }

}
