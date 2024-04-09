using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MR.Authentication.Models;
using MR.WebApi.Core.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MR.Authentication.Controllers;

[Route("api/identity")]
public class AuthController : MainController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AppSettings _appSettings;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IOptions<AppSettings> appSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _appSettings = appSettings.Value;
    }

    [HttpPost("new-account")]
    public async Task<ActionResult> Register(UserRegisterViewModel userRegister)
    {
        if (!ModelState.IsValid)
        {
            return CustomResponse(ModelState);
        }

        var user = new IdentityUser
        {
            UserName = userRegister.Email,
            Email = userRegister.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, userRegister.Password);

        if (result.Succeeded)
        {
            return CustomResponse(await GenerateJwt(userRegister.Email));
        }

        foreach (var erro in result.Errors)
        {
            AddProcessError(erro.Description);
        }

        return CustomResponse();
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult> Login(UserLoginViewModel userLogin)
    {
        if (!ModelState.IsValid)
        {
            return CustomResponse(ModelState);
        }

        var result = await _signInManager.PasswordSignInAsync(userLogin.Email, userLogin.Password, false, true);

        if (result.Succeeded)
        {
            return CustomResponse(await GenerateJwt(userLogin.Email));
        }

        if (result.IsLockedOut)
        {
            AddProcessError("Usuário temporariamente bloqueado por tentativas inválidas");
            return CustomResponse();
        }

        AddProcessError("Usuário e/ou senha incorretos");
        return CustomResponse();
    }

    private async Task<UserLoginResponseViewModel> GenerateJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var claims = await _userManager.GetClaimsAsync(user);

        var identityClaims = await GetUserClaim(user, claims);
        string encodedToken = EncodeToken(identityClaims);

        return GetResponseToken(user, claims, encodedToken);
    }

    private UserLoginResponseViewModel GetResponseToken(IdentityUser? user, IList<Claim> claims, string encodedToken)
    {
        return new UserLoginResponseViewModel
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
            UserToken = new UserTokenViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(c => new UserClaimViewModel { Type = c.Type, Value = c.Value })
            }
        };
    }

    private string EncodeToken(ClaimsIdentity identityClaims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _appSettings.Emissor,
            Audience = _appSettings.ValidoEm,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);
        return encodedToken;
    }

    private async Task<ClaimsIdentity> GetUserClaim(IdentityUser user, ICollection<Claim> claims)
    {
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.Now).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64));

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim("role", userRole));
        }

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);
        return identityClaims;
    }

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}
