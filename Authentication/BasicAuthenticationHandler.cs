using FullImplementaionAPI.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace FullImplementaionAPI.Authentication
{
    public class BasicAuthenticationHandler :
    AuthenticationHandler<AuthenticationSchemeOptions>
    {

        private readonly IUserDataAccess _userDataAccess;

        public
        BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions>
        options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserDataAccess userDataAccess) :base(options, logger, encoder, clock)
        {
            _userDataAccess = userDataAccess;
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            base.Response.Headers.Add("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""");
            var authHeader = base.Request.Headers["Authorization"].ToString();

            //Check the Authorization header
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing or invalid Authorization header"));
            }

            try
            {
                //Decoding base 64 credentials
                var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                var decodedBytes = Convert.FromBase64String(encodedCredentials);
                var decodedCredentials = Encoding.UTF8.GetString(decodedBytes);

                //Split the credentials
                var credentials = decodedCredentials.Split(':', 2);
                if (credentials.Length != 2)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header format"));
                }
                var email = credentials[0];
                var password = credentials[1];






                // Look up the user by email
                var user = _userDataAccess.GetUsers().FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Authentication failed: User not found."));
                }

                // Verify password hash
                bool isValid = Argon2Help.VerifyPassword(password, user.PasswordHash);
                if (!isValid)
                {
                    var hasher = new PasswordHasher<UserModel>();
                    var pwResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                    isValid = (pwResult == PasswordVerificationResult.Success);
                }

                if (!isValid)
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail("Authentication failed: Incorrect password."));
                }



                // Build claims
                var claims = new[]
                {
                    new Claim("name", $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);


                return Task.FromResult(AuthenticateResult.Success(ticket));




            }
            catch
            {
                Response.StatusCode = 401;
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
            }
        }
    }

}
