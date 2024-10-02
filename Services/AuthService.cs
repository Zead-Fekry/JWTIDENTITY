using JWT.Helpers;
using JWT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager <ApplicationUser> _userManager;
        private readonly JWTS _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWTS> jwt )
        {
            _userManager= userManager;
            _jwt=jwt.Value;
        }

      

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            var z = await _userManager.FindByEmailAsync(model.Email) ;
            if (z  != null)
            {
            return new AuthModel { Message = "Email is already registerd" };
            }
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
            return new AuthModel { Message = "UserName is already registerd" };
            }

            var User = new ApplicationUser { UserName = model.UserName, Email = model.Email , FirstName= model.FirstName ,LastName=model.LastName };
           var result= await _userManager.CreateAsync(User,model.Password);

            if (!result.Succeeded) 
            {
                String errors = String.Empty;
                foreach (var error in result.Errors) {
                    errors += ", ";
                    errors += error.Description;
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(User, "User");
            var JwtSecurityToken = await CreateJWT(User);

            return new AuthModel
            {
                Email = User.Email,
                Expires = JwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles= new List<string> { "User"},
                Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken),
                UserName = User.UserName,
            };
        }
        public async Task<AuthModel> GetTokenAsync(GetTokenModel model)
        {
            var Authmodel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, model.password)))
            {
                Authmodel.Message = "email or password is inncorrect";
                return Authmodel;
            }
            var JwtSecurityToken = await CreateJWT(user);
            var roleslist = await _userManager.GetRolesAsync(user);
            Authmodel.IsAuthenticated = true;
            Authmodel.Token = new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
            Authmodel.Email = user.Email;
            Authmodel.UserName = user.UserName;
            Authmodel.Expires = JwtSecurityToken.ValidTo;
            Authmodel.Roles = roleslist.ToList();
            return Authmodel;

        }

        private async Task<JwtSecurityToken> CreateJWT(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInDays),
                signingCredentials: credentials
                
            );

            return token;
        }
    }
}
