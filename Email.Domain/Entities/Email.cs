using System;

namespace Email.Domain.Entities
{
    public class EmailMessage
    {
        public int Id { get; set; }
        public string Uid { get; set; }//email unic number on email server
        public string ReceipientMailAddress { get; set; }
        public string From { get; set; }
        public string FileAttachName { get; set; }
        public string Subject { get; set; }
        public bool Parsed { get; set; }
        public DateTime Received { get; set; }
    }
}