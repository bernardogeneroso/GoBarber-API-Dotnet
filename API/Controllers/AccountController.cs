using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.User;
using Services.User.DTOs;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDtoCreateRequest user)
    {
        return HandleResult(await Mediator.Send(new Create.Command { User = user }));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDtoLoginRequest user)
    {
        return HandleResult(await Mediator.Send(new Session.Command { User = user }));
    }

    [HttpPost("avatar")]
    public async Task<IActionResult> UploadAvatar([FromForm] UploadAvatar.Command command)
    {
        return HandleResult(await Mediator.Send(command));
    }

    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        return HandleResult(await Mediator.Send(new Detail.Command()));
    }

    [AllowAnonymous]
    [HttpGet("verifyEmail")]
    public async Task<IActionResult> VerifyEmail(string token, string email)
    {
        return HandleResult(await Mediator.Send(new VerifyEmail.Command { Token = token, Email = email }));
    }

    [AllowAnonymous]
    [HttpGet("resendEmailConfirmationLink")]
    public async Task<IActionResult> ResendEmailConfirmationLink(string email)
    {
        return HandleResult(await Mediator.Send(new ResendEmailConfirmationLink.Command { Email = email }));
    }

    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        return HandleResult(await Mediator.Send(new RefreshToken.Command { RefreshToken = refreshToken }));
    }
}
