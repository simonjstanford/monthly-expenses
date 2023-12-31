// <copyright file="GetUserExpensesFunction.cs" company="Simon Stanford">
// Copyright (c) Simon Stanford. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using MonthlyExpenses.Api.Interfaces;
using MonthlyExpenses.Api.Models;

namespace MonthlyExpenses.Api.Functions;

public class GetUserExpensesFunction
{
    private readonly IRepository repository;
    private readonly IAuthenticator authenticator;

    public GetUserExpensesFunction(IRepository repository, IAuthenticator authenticator)
    {
        this.repository = repository;
        this.authenticator = authenticator;
    }

    [FunctionName("GetUserExpenses")]
    [OpenApiOperation(operationId: "GetUserExpenses", tags: new string[0], Description = "Fetches all the expense data for the currently authenticated user")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/json", bodyType: typeof(UserExpenses), Description = "The data has been returned")]
    [OpenApiResponseWithoutBody(HttpStatusCode.Unauthorized, Description = "When not authenticated")]
    [OpenApiResponseWithoutBody(HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserExpenses(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/data")] HttpRequest req,
        ILogger log)
    {
        try
        {
            log.LogInformation("monthexpenses GET called");
            var user = await authenticator.AuthenticateRequest(req, log);
            var data = await repository.GetUserExpenses(user, log);

            if (data != null)
            {
                log.LogInformation("monthexpenses GET completed");
                return new JsonResult(data);
            }
            else
            {
                return new NotFoundResult();
            }
        }
        catch (ClientAuthenticationException ex)
        {
            log.LogError(ex.Message);
            return new UnauthorizedResult();
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
            return new InternalServerErrorResult();
        }
    }
}
