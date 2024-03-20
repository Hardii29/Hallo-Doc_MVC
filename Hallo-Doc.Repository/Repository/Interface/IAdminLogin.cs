using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IAdminLogin
    {
        Task<AspnetUser?> Login(Login login);
    }
}
