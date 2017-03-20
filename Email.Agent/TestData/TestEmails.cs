using System;
using System.Collections.Generic;

namespace Email.Agent.TestData
{
    public class TestEmails
    {
        private readonly List<EmailData> _emails = new List<EmailData>();

        public TestEmails()
        {
            _emails.Add(new EmailData {Email = "c592@yandex.ru", Password = "1q2w3eOP", Login = "c592@yandex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData (TimeSpan.FromMinutes(10)) {Email = "manfice@yandex.ru", Password = "1q2w3eOP", Login = "manfice@yandex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
            _emails.Add(new EmailData {Email = "petr.id@yandex.ru", Password = "1q2w3eOP", Login = "petr.id@yandex.ru", ImapServer = "imap.yandex.ru", Port = 993, Ssl = true});
        }

        public List<EmailData> GetTestListEmails()
        {
            return _emails;
        }
    }
}