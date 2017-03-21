using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Email.Agent.TestData
{
    public class TestEmails
    {

        private readonly List<EmailData> _emails = new List<EmailData>();

        public TestEmails()
        {
            _emails.Add(new EmailData {Email = "c592@yandex.ru", Password = "1q2w3eOP", Login = "c592@yandex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData (TimeSpan.FromMinutes(10)) {Email = "manfice@yandex.ru", Password = "1q2w3eOP", Login = "manfice@yandex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData {Email = "motorsport23@ezex.ru", Password = "CPVXRNaiFQCy", Login = "motorsport23@ezex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData {Email = "dmitrij82@ezex.ru", Password = "porto777zz", Login = "dmitrij82@ezex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData {Email = "manfice@gmail.com", Password = "1q2w3eOP", Login = "manfice@gmail.com", ImapServer = "imap.gmail.com", Port = 993, Ssl = true});
        }

        public List<EmailData> GetTestListEmails()
        {
            return _emails;
        }

        public void AddToQueue()
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "EmailsToCheck",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                foreach (var emailData in _emails)
                {
                    var message = JsonConvert.SerializeObject(emailData);
                    var body = Encoding.UTF8.GetBytes(message);
                    var prop = channel.CreateBasicProperties();
                    prop.Persistent = true;
                    channel.BasicPublish("", "EmailsToCheck", prop, body);
                }
            }
        }
    }
}