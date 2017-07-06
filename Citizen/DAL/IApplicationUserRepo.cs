﻿using Citizen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Citizen.DAL
{
    public interface IApplicationUserRepo
    {
        IQueryable<ApplicationUser> GetApplicationUsers();

        ApplicationUser GetApplicationUserById(string id);
    }
}
