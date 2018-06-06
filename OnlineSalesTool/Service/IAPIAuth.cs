using OnlineSalesTool.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineSalesTool.DTO;
using static OnlineSalesTool.Controllers.AccountController;

namespace OnlineSalesTool.Service
{
    public interface IAPIAuth
    {
        bool Check(string sig, string guid);
        string Forge(string guid);
    }
}
