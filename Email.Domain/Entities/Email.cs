using System;

namespace Email.Domain.Entities
{
    public class Email
    {
        public int Id { get; set; }
        public string Uid { get; set; }//email unic number on email server
        public string From { get; set; }
        public string Subject { get; set; }
        public DateTime DateTime { get; set; }
        public bool Recieved { get; set; }
    }
}