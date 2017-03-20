using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Email.Agent.Helpers;
using Email.Agent.TestData;
using ImapX;
using ImapX.Enums;

namespace Email.Agent
{
    public static class EmailAgent
    {
        private static readonly Queue<EmailData> Queue = new Queue<EmailData>(new TestEmails().GetTestListEmails());

        public static List<EmailData> BadRequests { get; } = new List<EmailData>();


        public static object TestEmails()
        {
            var result = new List<string>();

            foreach (var data in Queue)
            {
                var client = new ImapClient(data.ImapServer, data.Port, data.Ssl);
                if (!client.Connect()) continue;
                if (client.Login(data.Login, data.Password))
                {
                    var dt = new EmailHelpers().DateForEmailFiler(DateTime.Now);
                    var d = client.Folders.Inbox.Search($"ON {dt}").Where(message => message.Attachments.Any(attachment => attachment.FileName.EndsWith(".csv")||attachment.FileName.EndsWith(".xls")||attachment.FileName.EndsWith(".xlsx"))); 
                    result.Add($"{data.Email} is successful authenticated at {DateTime.Now.ToLongDateString()}, mess: {string.Join(" / ", d.Select(message => message.Subject))}");
                }
                else
                {
                    result.Add($"{data.Email} login false at {DateTime.Now.ToLongDateString()}");
                    data.Result = new EmailLoadResult()
                    {
                        Result = $"{data.Email} login false at {DateTime.Now.ToLongDateString()}",
                        Success = false
                    };
                    BadRequests.Add(data);
                }
            }
            return result;
        }
    }
}