using System.Security.Claims;
using System.Text.Encodings.Web;
using announce_backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace announce_backend.Business.Auth.Authentification;

public class Authenticator(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<AuthSettings> authSettingsOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly AuthSettings _authSettings = authSettingsOptions.Value;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var tokenValue))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }
        
        var authToken = tokenValue.ToString().Split(" ").Last();
        if (authToken.Equals(_authSettings.Token))
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "User") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        
        return AuthenticateResult.Fail("Unauthorized");
    }
}