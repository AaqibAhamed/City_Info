using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public class AuthenticationRequestBody()
        {
            public string? UserName { get; set; }

            public string? Password { get; set; }
        }

        private class CityUserInfo
        {
            public int UserId { get; set; }

            public string UserName { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string City { get; set; }


            public CityUserInfo(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;

            }

        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            //Validate User
            var authenticate = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);

            if (authenticate == null)
            {
                return Unauthorized();
            }

            // create Token
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"] ));

            Console.WriteLine("securityKey "+ securityKey);

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Console.WriteLine("signingCredentials " + signingCredentials);

            // Claim the token

            var claimsForToken = new List<Claim>();

            claimsForToken.Add(new Claim("sub", authenticate.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", authenticate.FirstName));
            claimsForToken.Add(new Claim("family_name", authenticate.LastName));
            claimsForToken.Add(new Claim("city", authenticate.City));

            Console.WriteLine("claimsForToken "+ claimsForToken.ToList());


            Console.WriteLine("_configuration " + _configuration);

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            Console.WriteLine("jwtSecurityToken " + jwtSecurityToken);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);

        }


        private CityUserInfo ValidateUserCredentials(string? userName, string? password)
        {
            // we don't have a user DB or table.  If you have, check the passed-through
            // username/password against what's stored in the database.
            //
            // For demo purposes, we assume the credentials are valid

            // return a new CityInfoUser (values would normally come from your user DB/table)

            return new CityUserInfo(
                1,
                userName ?? "",
                "Aaqib",
                "Wiki",
                "Antwerp"
            );
        }


    }
}
