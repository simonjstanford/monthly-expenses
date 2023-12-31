// <copyright file="Startup.cs" company="Simon Stanford">
// Copyright (c) Simon Stanford. All rights reserved.
// </copyright>

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MonthlyExpenses.Api.Authentication;
using MonthlyExpenses.Api.Interfaces;
using MonthlyExpenses.Api.Repository;
using MonthlyExpenses.Api.Repository.Repository;

[assembly: FunctionsStartup(typeof(MonthlyExpenses.Api.Startup))]

namespace MonthlyExpenses.Api;

/// <summary>
/// DI class for injecting interface implementations.
/// </summary>
public class Startup : FunctionsStartup
{
    /// <inheritdoc/>
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddLogging();
        builder.Services.AddSingleton<IRepository, TableStorageRepository>();
        builder.Services.AddSingleton<IAuthenticator, OAuthAuthenticator>();
        builder.Services.AddSingleton<ITableClientFactory, TableClientFactory>();
    }
}