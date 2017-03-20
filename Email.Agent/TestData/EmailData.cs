using System;

namespace Email.Agent.TestData
{
    public class EmailData
    {
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ImapServer { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
        //Сколько раз пробовать связаться с сервером
        public int AttamptsToConnect { get; set; }
        //промежуток времени между попытками. по умолчанию 5 минут
        public TimeSpan WaitBetweenAttampts { get; set; }
        public DateTime CreatedTime { get; private set; }
        public virtual EmailLoadResult Result { get; set; }

        public EmailData()
        {
            CreatedTime = DateTime.Now;
            AttamptsToConnect = 5;
            WaitBetweenAttampts = TimeSpan.FromMinutes(5);
        }

        public EmailData(TimeSpan wait)
        {
            CreatedTime = DateTime.Now;
            AttamptsToConnect = 5;
            WaitBetweenAttampts = wait;
        }
    }

    public class EmailLoadResult
    {
        public DateTime InitDateTime { get; private set; }
        public string Result { get; set; }
        public bool Success { get; set; }

        public EmailLoadResult()
        {
            InitDateTime = DateTime.Now;
        }
    }
}