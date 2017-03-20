using System;
using System.Collections.Generic;
using System.Text;
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
                var client = new ImapClient(data.ImapServer, data.Port, data.Ssl)
                {
                    Behavior = {MessageFetchMode = MessageFetchMode.Attachments}
                };
                if (!client.Connect()) continue;
                if (client.Login(data.Login, data.Password))
                {
                    var d = client.Folders.Inbox.Messages;

                    result.Add($"{data.Email} is successful authenticated at {DateTime.Now.ToLongDateString()}, mess: {d}");
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