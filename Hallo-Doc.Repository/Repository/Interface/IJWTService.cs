using Hallo_Doc.Entity.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IJWTService
    {
        string GenerateToken(AspnetUser user);
        bool ValidateToken(string token, out JwtSecurityToken jWTSecurityToken);
    }
}
