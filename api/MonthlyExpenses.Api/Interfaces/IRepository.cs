// <copyright file="IRepository.cs" company="Simon Stanford">
// Copyright (c) Simon Stanford. All rights reserved.
// </copyright>

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MonthlyExpenses.Api.Models;

namespace MonthlyExpenses.Api.Interfaces;

/// <summary>
/// Represents the user expenses data store.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets all expense data for the given user.
    /// </summary>
    Task<UserExpenses> GetUserExpenses(User user, ILogger log);

    /// <summary>
    /// Saves the given expense data for the specified user.
    /// </summary>
    Task SaveUserExpenses(User user, UserExpenses data, ILogger log);
}