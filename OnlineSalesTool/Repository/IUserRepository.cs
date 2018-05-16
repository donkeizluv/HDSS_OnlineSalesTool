using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Repository
{
    public interface IUserRepository
    {
        Task<AppUser> GetUser(string userName);
    }
}
