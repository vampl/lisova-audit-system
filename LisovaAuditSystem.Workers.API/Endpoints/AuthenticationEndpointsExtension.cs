using LisovaAuditSystem.Workers.API.Dtos;
using LisovaAuditSystem.Workers.API.Interfaces.Services;
using LisovaAuditSystem.Workers.API.Models.Requests;

using Microsoft.AspNetCore.Http.HttpResults;

namespace LisovaAuditSystem.Workers.API.Endpoints;

public static class AuthenticationEndpointsExtension
{
    const int ProblemHttpResultStatusCode = 500;

    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder group = routes.MapGroup("api/v1/authentications");

        group.MapPost("register", PostUserRegistration)
            .AllowAnonymous();
        group.MapPost("login", PostUserLogin)
            .AllowAnonymous();
    }

    public static async Task<Results<Ok<string>, BadRequest<string>, ProblemHttpResult>> PostUserRegistration(
        IAuthenticationService authenticationService,
        PostUserRegistrationRequest request)
    {
        try
        {
            return TypedResults.Ok(
                await authenticationService.RegisterAsync(
                    new UserRegisterCredentialsDto(
                        username: request.PostUserRegistrationPayload.UserName,
                        email: request.PostUserRegistrationPayload.Email,
                        password: request.PostUserRegistrationPayload.Password
                    )));
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("already exists"))
        {
            return TypedResults.BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            return TypedResults.Problem(detail: exception.Message, statusCode: ProblemHttpResultStatusCode);
        }
    }

    public static async Task<Results<Ok<string>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> PostUserLogin(
        IAuthenticationService authenticationService,
        PostUserLoginRequest request)
    {
        try
        {
            return TypedResults.Ok(
                await authenticationService.LoginAsync(
                    new UserLoginCredentialsDto(
                        email: request.PostUserLoginPayload.Email,
                        password: request.PostUserLoginPayload.Password)));
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("not found"))
        {
            return TypedResults.NotFound(exception.Message);
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("password"))
        {
            return TypedResults.BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            return TypedResults.Problem(detail: exception.Message, statusCode: ProblemHttpResultStatusCode);
        }
    }
}
