using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MR.WebApi.Core.Identity;

public class CustomAutorization
{
    public static bool ValidateUserClaim(HttpContext context, string claimName, string claimValue)
        => context.User.Identity.IsAuthenticated &&
            context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
}

public class ClaimsAuthorizeAttribute : TypeFilterAttribute
{
    public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequirementClaimFilter))
    {
        Arguments = new object[] { new Claim(claimName, claimValue) };
    }
}

public class RequirementClaimFilter : IAuthorizationFilter
{
    private readonly Claim _claim;

    public RequirementClaimFilter(Claim claim)
    {
        _claim = claim;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Identity.IsAuthenticated)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        if (!CustomAutorization.ValidateUserClaim(context.HttpContext, _claim.Type, _claim.Value))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        }
    }
}
