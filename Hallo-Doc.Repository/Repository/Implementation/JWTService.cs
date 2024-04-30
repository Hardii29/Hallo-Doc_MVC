using Hallo_Doc.Entity.Models;
using Hallo_Doc.Repository.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Hallo_Doc.Entity.Data;
using NuGet.Packaging;
using System.Linq;

namespace Hallo_Doc.Repository.Repository.Implementation
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public JWTService(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(AspnetUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.AspNetUserRoles.FirstOrDefault().RoleId),
                new Claim("UserID", user.Id),
                new Claim("Username", user.Username)
            };
            int RoleId;
            List<int> MenuIds = new List<int>();
            switch(user.AspNetUserRoles.FirstOrDefault().RoleId)
            {
                case "Admin":
                    RoleId = (int)_context.Admins.FirstOrDefault(r => r.AspNetUserId == user.Id).RoleId;
                    MenuIds = _context.RoleMenus.Where(rm => rm.RoleId == RoleId).Select(rm => rm.MenuId).ToList();
                    break;
                case "Physician":
                    RoleId = (int)_context.Physicians.FirstOrDefault(r => r.AspNetUserId == user.Id).RoleId;
                    MenuIds = _context.RoleMenus.Where(rm => rm.RoleId == RoleId).Select(rm => rm.MenuId).ToList();
                    break;
                default:
                    RoleId = 0;
                    break;
            }
            claims.Add(new Claim("RoleId", RoleId.ToString()));
            claims.AddRange(MenuIds.Select(id => new Claim("MenuId", id.ToString())));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToString(_configuration["Jwt:Key"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                Convert.ToString(_configuration["Jwt:Issuer"]),
                Convert.ToString(_configuration["Jwt:Audience"]),
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token, out JwtSecurityToken? jWTSecurityToken)
        {
            jWTSecurityToken = null;

            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Convert.ToString(_configuration["Jwt:Key"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                jWTSecurityToken = (JwtSecurityToken)validatedToken;

                if (jWTSecurityToken != null)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        private readonly string _menuId;

        public CustomAuthorize(string role = "", string menuId = "")
        {
            _role = role;
            _menuId = menuId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJWTService>();

            if (jwtService == null)
            {
                if (_role == "Admin")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminLogin", action = "Login" }));
                }
                else if (_role == "Patient")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login" }));
                }
                return;
            }

            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                if (_role == "Admin")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminLogin", action = "Login" }));
                }
                else if (_role == "Patient")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login" }));
                }
                return;
            }

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                if (_role == "Admin")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminLogin", action = "Login" }));
                }
                else if (_role == "Patient")
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Login" }));
                }
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }
            var menuCliam = jwtToken.Claims.Where(claim => claim.Type == "MenuId").Select(claim => claim.Value).ToList();
            if (_role == "Admin" && (menuCliam == null || !menuCliam.Contains(_menuId)))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }
        }
    }
}
