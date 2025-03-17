using AdminPanelProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AuthorizationFilter : IAsyncActionFilter
{
    private readonly UserClaimsService _userClaimsService;
    private readonly RoleClaimsService _roleClaimsService;

    public AuthorizationFilter(UserClaimsService userClaimsService, RoleClaimsService roleClaimsService)
    {
        _userClaimsService = userClaimsService;
        _roleClaimsService = roleClaimsService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await _userClaimsService.IsAdminClaim(userId))
        {
            context.Result = new UnauthorizedObjectResult("User yetkisi yok.");
            return;
        }

        if (!await _roleClaimsService.CanViewProductClaim(userId))
        {
            context.Result = new UnauthorizedObjectResult("Role yetkisi yok.");
            return;
        }

        await next();
    }
}
