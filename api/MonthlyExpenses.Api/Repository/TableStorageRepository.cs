// <copyright file="TableStorageRepository.cs" company="Simon Stanford">
// Copyright (c) Simon Stanford. All rights reserved.
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MonthlyExpenses.Api.Interfaces;
using MonthlyExpenses.Api.Models;
using MonthlyExpenses.Api.Repository.Repository;
using MonthlyExpenses.Api.Utils;

namespace MonthlyExpenses.Api.Repository;

/// <inheritdoc/>
public class TableStorageRepository : IRepository
{
    private const string PartitionKey = "user-expense-data";
    private readonly ITableClientFactory tableClientFactory;

    public TableStorageRepository(ITableClientFactory tableClientFactory)
    {
        this.tableClientFactory = tableClientFactory;
    }

    /// <inheritdoc/>
    public async Task<UserExpenses> GetUserExpenses(User user, ILogger log)
    {
        try
        {
            var table = await tableClientFactory.GetExpensesTable();
            var entity = await table.GetEntityIfExistsAsync<UserExpenseEntity>(PartitionKey, user.Id);

            if (entity?.HasValue == true && entity.Value?.Expenses is string expensesJson)
            {
                return Serializer.Deserialize<UserExpenses>(expensesJson);
            }
            else
            {
                return new UserExpenses(user.Name);
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Unable to fetch data from storage: {ex}");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task SaveUserExpenses(User user, UserExpenses data, ILogger log)
    {
        try
        {
            var table = await tableClientFactory.GetExpensesTable();
            var entity = CreateEntity(user, data);
            await table.UpsertEntityAsync(entity);
        }
        catch (Exception ex)
        {
            log.LogError($"Unable to add data to storage: {ex}");
            throw;
        }
    }

    private static UserExpenseEntity CreateEntity(User user, UserExpenses data)
    {
        return new UserExpenseEntity()
        {
            RowKey = user.Id,
            PartitionKey = PartitionKey,
            Expenses = Serializer.Serialize(data),
        };
    }
}