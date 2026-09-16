using System.Security.Claims;
using DevOpsPlatformHub.Api.Contracts;
using DevOpsPlatformHub.Application.Authentication.Contracts;
using Microsoft.AspNetCore.Authorization;
using static DevOpsPlatformHub.Api.Constants.RouteKeys;
using Microsoft.AspNetCore.Mvc;

namespace DevOpsPlatformHub.Api.Controllers;

[ApiController]
[Route(MainRoute)]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost(Register)]
    [ProducesResponseType<AuthenticationResult>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthenticationResult>> RegisterAsync([FromBody] RegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var registration = await authenticationService.RegisterAsync(request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, registration);
    }

    [AllowAnonymous]
    [HttpPost(Login)]
    [ProducesResponseType<AuthenticationResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResult>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authenticate = await authenticationService.AuthenticateAsync(request, cancellationToken);

        return authenticate is null ? Unauthorized() : Ok(authenticate);
    }

    [Authorize]
    [HttpGet(CurrentUser)]
    [ProducesResponseType<CurrentUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserResponse> GetCurrentUser()
    {
        var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name = User.FindFirst(ClaimTypes.GivenName)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (!Guid.TryParse(userIdValue, out var userId) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username))
        {
            return Unauthorized();
        }

        var roles = User.FindAll(ClaimTypes.Role)
            .Select(claim => claim.Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(role => role, StringComparer.Ordinal)
            .ToArray();

        return Ok(new CurrentUserResponse(userId, username, name, roles));
    }
}
