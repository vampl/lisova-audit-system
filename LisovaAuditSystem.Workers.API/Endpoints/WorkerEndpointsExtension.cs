using LisovaAuditSystem.Workers.API.Common.Extensions.Mappings;
using LisovaAuditSystem.Workers.API.Models.Payloads;
using LisovaAuditSystem.Workers.API.Models.Requests;
using LisovaAuditSystem.Workers.API.Models.Responses;
using LisovaAuditSystem.Workers.API.Services;

using Microsoft.AspNetCore.Http.HttpResults;

namespace LisovaAuditSystem.Workers.API.Endpoints;

public static class WorkerEndpointsExtension
{
    const int ProblemHttpResultStatusCode = 500;

    public static void MapWorkersEndpoints(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder workersEndpointsGroup = routes.MapGroup("api/v1/workers");

        workersEndpointsGroup.MapGet("", GetWorkersAsync);
        workersEndpointsGroup.MapGet("{workerId:guid}", GetWorkerAsync);
        workersEndpointsGroup.MapPost("", PostWorkerAsync);
        workersEndpointsGroup.MapPut("", PutWorkerAsync);
        workersEndpointsGroup.MapDelete("{workerId:guid}", DeleteWorkerAsync);
    }

    public static async Task<Ok<GetWorkersResponse>>
        GetWorkersAsync(IWorkerService workerService)
    {
        IReadOnlyList<GetWorkerPayload> getWorkersPayloads =
            (await workerService.ReadAllAsync()).Select(worker => worker.ToPayload())
            .ToList();
        GetWorkersResponse response = new(getWorkersPayloads);

        return TypedResults.Ok(response);
    }

    public static async Task<Results<Ok<GetWorkerResponse>, NotFound<string>, ProblemHttpResult>>
        GetWorkerAsync(IWorkerService workerService, Guid workerId)
    {
        try
        {
            GetWorkerPayload getWorkerPayload =
                (await workerService.ReadByIdAsync(workerId)).ToPayload();
            GetWorkerResponse response = new(getWorkerPayload);

            return TypedResults.Ok(response);
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("not found"))
        {
            return TypedResults.NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            return TypedResults.Problem(detail: exception.Message, statusCode: ProblemHttpResultStatusCode);
        }
    }

    public static async Task<Results<Created<PostWorkerResponse>, BadRequest<string>, ProblemHttpResult>>
        PostWorkerAsync(IWorkerService workerService, PostWorkerRequest request)
    {
        try
        {
            Guid workerId = await workerService.CreateAsync(request.PostWorkerPayload.ToDto());

            return TypedResults.Created($"/api/v1/workers/{workerId}", new PostWorkerResponse(workerId));
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("Validation failed"))
        {
            return TypedResults.BadRequest(exception.Message);
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

    public static async Task<Results<NoContent, BadRequest<string>, NotFound<string>, ProblemHttpResult>>
        PutWorkerAsync(IWorkerService workerService, PutWorkerRequest request)
    {
        try
        {
            await workerService.EditAsync(request.PutWorkerPayload.ToDto());

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("Validation failed"))
        {
            return TypedResults.BadRequest(exception.Message);
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("not found"))
        {
            return TypedResults.NotFound(exception.Message);
        }
        catch (Exception exception)
        {
            return TypedResults.Problem(detail: exception.Message, statusCode: ProblemHttpResultStatusCode);
        }
    }

    public static async Task<Results<NoContent, BadRequest<string>, ProblemHttpResult>>
        DeleteWorkerAsync(IWorkerService workerService, Guid workerId)
    {
        try
        {
            await workerService.RemoveAsync(workerId);

            return TypedResults.NoContent();
        }
        catch (InvalidOperationException exception) when (exception.Message.Contains("not found"))
        {
            return TypedResults.BadRequest(exception.Message);
        }
        catch (Exception exception)
        {
            return TypedResults.Problem(detail: exception.Message, statusCode: ProblemHttpResultStatusCode);
        }
    }
}
