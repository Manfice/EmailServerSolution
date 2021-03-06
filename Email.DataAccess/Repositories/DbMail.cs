﻿using System;
using System.Collections.Generic;
using System.Linq;
using Email.DataAccess.IRepositories;
using Email.Domain.Context;
using Email.Domain.Entities;

namespace Email.DataAccess.Repositories
{
    public class DbMailRepository : IMailService, IDisposable
    {
        private readonly ApplicationDbContext _context = ApplicationDbContext.Create();

        public void Dispose()
        {
            
        }

        public IEnumerable<string> GetReceivedUidsOnDate(DateTime date, string recipient)
        {
            return
                _context.EmailMessages.Where(
                    r =>
                        r.ReceipientMailAddress.Equals(recipient, StringComparison.CurrentCultureIgnoreCase) &&
                        r.Received >= date).Select(message => message.Uid);
        }

        public void SaveMessage(EmailMessage message)
        {
            _context.EmailMessages.Add(message);
            _context.SaveChanges();
        }
    }
}