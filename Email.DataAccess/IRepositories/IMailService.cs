using System;
using System.Collections.Generic;
using Email.Domain.Context;
using Email.Domain.Entities;

namespace Email.DataAccess.IRepositories
{
    public interface IMailService
    {
        IEnumerable<string> GetReceivedUidsOnDate(DateTime date, string recipient);
        void SaveMessage(EmailMessage message);
    }
}