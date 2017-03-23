using System;
using System.Collections.Generic;
using Email.DataAccess.IRepositories;
using Email.Domain.Context;

namespace Email.DataAccess.Repositories
{
    public class DbUserRepository : IUser, IDisposable
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        public void Dispose()
        {
            
        }

        public IEnumerable<ApplicationUser> GetUserList()
        {
            return null;
        }
    }
}