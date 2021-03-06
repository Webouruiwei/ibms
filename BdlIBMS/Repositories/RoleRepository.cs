﻿using BdlIBMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BdlIBMS.Repositories
{
    public class RoleRepository : AbstractRespository<int, Role>, IRoleRepository
    {
        public override DbSet<Role> GetAll()
        {
            return this.db.Roles;
        }

        public override bool IsExist(int id)
        {
            return db.Roles.Count(e => e.ID == id) > 0;
        }

        public IEnumerable<dynamic> FindRolesByUUID(string uuid)
        {
            var results = from item in db.Roles
                          where item.UUID == uuid
                          select
                              new
                              {
                                  ID = item.ID,
                                  UUID = item.UUID,
                                  ModuleID = item.ModuleID,
                                  ModuleName = item.Module.Name,
                                  ModuleStatus = item.Module.Status,
                                  Name = item.Name,
                                  Description = item.Description,
                                  CanRead = item.CanRead,
                                  CanWrite = item.CanWrite,
                                  Status = item.Status,
                                  Remark = item.Remark,
                                  CreateTime = item.CreateTime
                              };
            return results;
        }

        public async Task ModifyRolesStatusAsync(IEnumerable<dynamic> roles, bool status)
        {
            foreach (dynamic item in roles)
            {
                Role role = await GetByIdAsync(item.ID);
                role.Status = status;
                if (role != null)
                    db.Entry(role).State = EntityState.Modified;
            }
            await this.db.SaveChangesAsync();
        }

        public async Task ModifyRolesBasicAsync(IEnumerable<dynamic> roles, string name, string description)
        {
            foreach (dynamic item in roles)
            {
                Role role = await GetByIdAsync(item.ID);
                role.Name = name;
                role.Description = description;
                if (role != null)
                    db.Entry(role).State = EntityState.Modified;
            }
            await this.db.SaveChangesAsync();
        }

        public string FindRoleNameByUUID(string uuid)
        {
            var results = from item in db.Roles where item.UUID == uuid select item;
            string roleName = results.FirstOrDefault().Name;
            return roleName;
        }
    }
}