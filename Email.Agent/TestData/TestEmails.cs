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
            _emails.Add(new EmailData(DateTime.Today.AddDays(-10)) {Email = "pobeda@ezex.ru", Password = "rD7t4yfgzp", Login = "pobeda@ezex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData (TimeSpan.FromMinutes(10)) {Email = "avtozone@ezex.ru", Password = "c08LYitWhd28", Login = "avtozone@ezex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "motorsport23@ezex.ru", Password = "CPVXRNaiFQCy", Login = "motorsport23@ezex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "dmitrij82@ezex.ru", Password = "porto777zz", Login = "dmitrij82@ezex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "avtoelement@ezex.ru", Password = "DY7M60TvChd8", Login = "avtoelement@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "avtoritet@ezex.ru", Password = "aM5AYERoOMYn", Login = "avtoritet@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "ruslan@ezex.ru", Password = "CKd0T780l7F0", Login = "ruslan@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "autodom@ezex.ru", Password = "Dk6fWLMnxf", Login = "autodom@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "gornostay@ezex.ru", Password = "0UvcwLwJbA", Login = "gornostay@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "zapchastioz@ezex.ru", Password = "e1M3FvD71P73", Login = "zapchastioz@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
            _emails.Add(new EmailData(DateTime.Today) {Email = "autodom93@ezex.ru", Password = "RmMkjP7iPBbx", Login = "autodom93@ezex.ru", ImapServer = "imap.yandex.com", Port = 993, Ssl = true});
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