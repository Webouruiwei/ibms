using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdlIBMS.Repositories
{
    public interface IUserRepository : IRepository<string, User>
    {
        User FindByUserNameAndPassword(string userName, string password);
    }
}
