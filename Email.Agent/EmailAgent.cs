using System;
using System.Collections.Generic;
using System.IO.Compression;
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
using SharpCompress.Readers;
using Email.Domain.Entities;

namespace Email.Agent
{
    public class EmailAgent:IDisposable
    {

        //RabbitMQ connections and properties
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private string QueueName { get; }

        //

        public Guid AgentGuid { get; }
        public bool NeedToDispose { get; set; }
        private EventingBasicConsumer _consumer;

        private readonly Regex _searchFileExt = new Regex("^.*\\.(csv|txt|xls|xlsx|zip|rar)$");
        private readonly Regex _archFileExt = new Regex("^.*\\.(zip|rar)$");

        private const string SavePath = @"C:\DownloadedFiles\";

        //ImapX helpers
        private IList<Message> _messageList;
        private IEnumerable<string> _receivedMessagesUids;


        public  EmailAgent(string queueName, string host)
        {
            AgentGuid = Guid.NewGuid();
            NeedToDispose = false;
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
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
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
                    //Dispose(100,result.Result.Result);
                    //return;
                }
                _channel.BasicAck(args.DeliveryTag, false);
                if (NeedToDispose)
                {
                    Dispose();
                }
            };
            _channel.BasicConsume(QueueName, false, _consumer);
        }

        public EmailData TestEmails(EmailData data)
        {
            var client = new ImapClient(data.ImapServer, data.Port, data.Ssl);
            try
            {
                client.Connect();
                client.Login(data.Login, data.Password);
                var dt = new EmailHelpers().DateForEmailFiler(data.InquireDate);
                _messageList = client.Folders.Inbox.Search($"SINCE {dt}").Where(message => message.Attachments.Any(attachment => _searchFileExt.IsMatch(attachment.FileName))).ToList();

                if (_messageList != null && _messageList.Any())
                {
                    foreach (var message in _messageList)
                    {
                        if (_receivedMessagesUids.Contains(message.UId.ToString())) continue;
                        foreach (var attachment in message.Attachments)
                        {
                            if (!attachment.Downloaded)
                            {
                                attachment.Download();
                            }
                            var dir = $"{SavePath}{data.Email}\\{DateTime.Today.ToLongDateString()}";
                            var mes = SaveAttachmentToDisk(attachment, dir);
                            SaveResultToDatabase(new EmailMessage
                            {
                                Parsed = false,
                                Subject =  message.Subject,
                                Received = DateTime.Now,
                                Uid = message.UId.ToString(),
                                ReceipientMailAddress = data.Email,
                                FileAttachName = mes,
                                From = message.From.Address
                            });
                        }
                    }
                    data.Result = new EmailLoadResult()
                    {
                        Result =
                            $"{data.Email} is successful authenticated at {DateTime.Now.ToLongDateString()}, mess: {string.Join(" / ", _messageList.Select(message => message.Subject))} with agent: {AgentGuid}",
                        Success = true
                    };
                }
                else
                {
                    data.Result = new EmailLoadResult()
                    {
                        Result =
                            $"{data.Email} is successful authenticated at {DateTime.Now.ToLongDateString()}, but no files recieved with agent: {AgentGuid}",
                        Success = true
                    };
                }
            }
            catch (Exception e)
            {
                data.Result = new EmailLoadResult()
                {
                    Result = $"{data.Email} throw exception:\"{e.Message}\" at {DateTime.Now.ToLongDateString()} with agent: {AgentGuid}",
                    Success = false
                };
            }
            //client.Logout();
            return data;
        }

        private string SaveAttachmentToDisk(Attachment attachment, string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var path = Path.Combine(dir, attachment.FileName);
                if (_archFileExt.IsMatch(attachment.FileName))
                {
                    using (var st = new MemoryStream(attachment.FileData))
                    using (var file = ReaderFactory.Open(st))
                    {
                        while (file.MoveToNextEntry())
                        {
                            if (!file.Entry.IsDirectory)
                            {
                                file.WriteEntryToDirectory(dir, new ExtractionOptions { Overwrite = true });
                                return Path.Combine(dir,file.Entry.Key);

                            }
                        }
                    }
                }
                else
                {
                    if (!File.Exists(path))
                    {
                        File.WriteAllBytes(path, attachment.FileData);
                    }
                    else
                    {
                        File.Delete(path);
                        File.WriteAllBytes(path, attachment.FileData);
                    }
                }
                return path;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private static void SaveResultToDatabase(EmailMessage message)
        {
            using (var ctx = new DbMailRepository())
            {
                ctx.SaveMessage(message);
            }
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

        private void Dispose(ushort code, string message)
        {
            _channel.Close(code, message);
            _connection.Close();
        }
    }

}
