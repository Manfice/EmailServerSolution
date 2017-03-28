using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Email.Agent.Helpers;
using Email.Agent.TestData;
using ImapX;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Email.Agent
{
    public static class EmailAgentManager
    {

        private const string QueueName = "EmailsToCheck";

        public static List<EmailData> BadRequests { get; set; } = new List<EmailData>();

        public static List<EmailData> SuccessRequests { get; set; } = new List<EmailData>();

        public static List<EmailAgent> EmailAgents = new List<EmailAgent>();


        public static void StartAgent(string host, int agentCount)
        {
            if (EmailAgents.Any())
            {
                foreach (var emailAgent in EmailAgents)
                {
                    emailAgent.NeedToDispose = true;
                }
            }
            for (var i = 0; i < agentCount; i++)
            {
                EmailAgents.Add(new EmailAgent(QueueName, host));
            }
            return;
        }

        public static void AddAgent(string host)
        {
            EmailAgents.Add(new EmailAgent(QueueName, host));
        }

        public static void StopAgent()
        {
            foreach (var emailAgent in EmailAgents)
            {
                emailAgent.Dispose();
            }
            return;
        }
    }
}
