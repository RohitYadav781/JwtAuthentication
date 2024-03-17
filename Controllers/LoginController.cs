using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class LoginController : Controller
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserModel login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(UserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel AuthenticateUser(UserModel login)
        {
            UserModel user = null;

            //Validate the User Credentials
            //Demo Purpose, I have Passed HardCoded User Information
            if (login.Username == "Rohit")
            {
                user = new UserModel { Username = "Rohit Yadav", EmailAddress = "test@gmail.com" };
            }
            return user;
        }

        public class UserModel {

            public string Username { get; set; }
            public string EmailAddress { get; set; }
        }

        [HttpGet]
[Authorize]
public ActionResult<IEnumerable<string>> Get()
{
    var currentUser = HttpContext.User;
    int spendingTimeWithCompany = 0;

    if (currentUser.HasClaim(c => c.Type == "DateOfJoing"))
    {
        DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "DateOfJoing").Value);
        spendingTimeWithCompany = DateTime.Today.Year - date.Year;
    }

    if(spendingTimeWithCompany > 5)
    {
        return new string[] { "High Time1", "High Time2", "High Time3", "High Time4", "High Time5" };
    }
    else
    {
        return new string[] { "value1", "value2", "value3", "value4", "value5" };
    }
}

    }
}
