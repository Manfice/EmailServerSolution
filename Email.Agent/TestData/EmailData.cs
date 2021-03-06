﻿using System;

namespace Email.Agent.TestData
{
    public class EmailData
    {
        public Guid Guid { get; set; }
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
        public DateTime InquireDate { get; set; }
        public virtual EmailLoadResult Result { get; set; }

        public EmailData()
        {
            Guid = Guid.NewGuid();
            CreatedTime = DateTime.Now;
            AttamptsToConnect = 5;
            InquireDate = DateTime.Today;
            WaitBetweenAttampts = TimeSpan.FromMinutes(5);
        }

        public EmailData(DateTime inquiredate)
        {
            Guid = Guid.NewGuid();
            CreatedTime = DateTime.Now;
            AttamptsToConnect = 5;
            WaitBetweenAttampts = TimeSpan.FromMinutes(5);
            this.InquireDate = inquiredate;
        }

        public EmailData(TimeSpan wait)
        {
            Guid = Guid.NewGuid();
            CreatedTime = DateTime.Now;
            AttamptsToConnect = 5;
            WaitBetweenAttampts = wait;
            InquireDate = DateTime.Today;
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