using System.Collections.Generic;
using Email.Domain.Context;

namespace Email.DataAccess.IRepositories
{
    public interface IUser
    {
        IEnumerable<ApplicationUser> GetUserList();
    }
}