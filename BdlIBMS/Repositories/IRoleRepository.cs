using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BdlIBMS.Repositories
{
    public interface IRoleRepository : IRepository<int, Role>
    {
        IEnumerable<dynamic> FindRolesByUUID(string uuid);

        string FindRoleNameByUUID(string uuid);

        Task ModifyRolesStatusAsync(IEnumerable<dynamic> roles, bool status);

        Task ModifyRolesBasicAsync(IEnumerable<dynamic> roles, string name, string description);
    }
}
