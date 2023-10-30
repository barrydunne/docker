﻿using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Email.Logic.Metrics
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    internal class GetEmailsSentToRecipientQueryHandlerMetrics : IGetEmailsSentToRecipientQueryHandlerMetrics
    {
        private static readonly Counter<long> _count = LogicMetrics.Meter.CreateCounter<long>("GetEmailsSentToRecipient.Handled.Count", null, "The number of queries handled.");
        private static readonly Histogram<double> _loadTime = LogicMetrics.Meter.CreateHistogram<double>("GetEmailsSentToRecipient.Load", unit: "ms", "Time taken to load the emails.");

        /// <inheritdoc/>
        public void IncrementCount() => _count!.Add(1);

        /// <inheritdoc/>
        public void RecordLoadTime(double value) => _loadTime!.Record(value);
    }
}