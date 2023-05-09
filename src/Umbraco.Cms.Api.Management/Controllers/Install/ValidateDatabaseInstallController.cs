﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Install.Models;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Migrations.Install;
using Umbraco.Cms.Api.Management.ViewModels.Installer;

namespace Umbraco.Cms.Api.Management.Controllers.Install;

[ApiVersion("1.0")]
public class ValidateDatabaseInstallController : InstallControllerBase
{
    private readonly DatabaseBuilder _databaseBuilder;
    private readonly IUmbracoMapper _mapper;

    public ValidateDatabaseInstallController(
        DatabaseBuilder databaseBuilder,
        IUmbracoMapper mapper)
    {
        _databaseBuilder = databaseBuilder;
        _mapper = mapper;
    }

    [HttpPost("validate-database")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateDatabase(DatabaseInstallResponseModel responseModel)
    {
        DatabaseModel databaseModel = _mapper.Map<DatabaseModel>(responseModel)!;

        var success = _databaseBuilder.ConfigureDatabaseConnection(databaseModel, true);

        if (success)
        {
            return await Task.FromResult(Ok());
        }

        var invalidModelProblem = new ProblemDetails
        {
            Title = "Invalid database configuration",
            Detail = "The provided database configuration is invalid",
            Status = StatusCodes.Status400BadRequest,
            Type = "Error",
        };

        return await Task.FromResult(BadRequest(invalidModelProblem));
    }
}
