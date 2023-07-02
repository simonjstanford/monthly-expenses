// <copyright file="MonthData.cs" company="Simon Stanford">
// Copyright (c) Simon Stanford. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MonthlyExpenses.Api.Models
{
    public sealed class MonthData : IEquatable<MonthData>
    {
        /// <summary>
        /// The month that this expense info represents.
        /// </summary>
        [JsonPropertyName("monthStart")]
        public DateTime MonthStart { get; set; }

        /// <summary>
        /// The income that is being received this month.
        /// </summary>
        [JsonPropertyName("income")]
        public Expense[] Income { get; set; }

        /// <summary>
        /// The outgoings that are paid out this month.
        /// </summary>
        [JsonPropertyName("outgoings")]
        public Expense[] Outgoings { get; set; }

        public static bool operator ==(MonthData data1, MonthData data2)
        {
            if (((object)data1) == null || ((object)data2) == null)
            {
                return Equals(data1, data2);
            }

            return data1.Equals(data2);
        }

        public static bool operator !=(MonthData data1, MonthData data2)
        {
            if (((object)data1) == null || ((object)data2) == null)
            {
                return !Equals(data1, data2);
            }

            return !data1.Equals(data2);
        }

        public bool Equals(MonthData other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Equals(MonthStart, other.MonthStart) &&
                Income.SequenceEqual(other.Income) &&
                Outgoings.SequenceEqual(other.Outgoings))
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj) => Equals(obj as MonthData);

        public override int GetHashCode()
        {
            var hash = default(HashCode);
            hash.Add(MonthStart);

            foreach (var income in Income)
            {
                hash.Add(income);
            }

            foreach (var outgoing in Outgoings)
            {
                hash.Add(outgoing);
            }

            return hash.ToHashCode();
        }
    }
}