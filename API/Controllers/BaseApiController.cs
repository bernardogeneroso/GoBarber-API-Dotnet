using Application.Core;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result is null)
            return NotFound();

        if (result.IsSuccess && result.IsSuccessNoContent)
            return NoContent();

        if (result.IsSuccess && result.Value is not null)
            return Ok(result.Value);

        if (result.IsSuccess && result.Value is null)
            return NotFound();

        if (result.IsUnauthorized)
            return Unauthorized(result.Error);

        if (result.Error is not null && result.FluentValidationError is null)
            return BadRequest(result.Error);

        result.FluentValidationError.AddToModelState(ModelState, null);

        return ValidationProblem();
    }
}
