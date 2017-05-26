﻿// <copyright file="ElasticsearchTextResponseWriter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Extensions.Middleware.Abstractions;
using App.Metrics.Formatting;
using App.Metrics.Formatting.ElasticSearch;
using App.Metrics.Reporting;
using Microsoft.AspNetCore.Http;

namespace App.Metrics.Formatters.ElasticSearch
{
    public class ElasticsearchTextResponseWriter : IMetricsTextResponseWriter
    {
        private readonly string _index;

        public ElasticsearchTextResponseWriter(string index) { _index = index; }

        /// <inheritdoc />
        public string ContentType => "text/plain; app.metrics=vnd.app.metrics.v1.metrics.elasticsarch; elasticsearch=5.4.x;";

        public Task WriteAsync(HttpContext context, MetricsDataValueSource metricsData, CancellationToken token = default(CancellationToken))
        {
            var payloadBuilder = new BulkPayloadBuilder(
                _index,
                Constants.ElasticsearchDefaults.MetricNameFormatter,
                Constants.ElasticsearchDefaults.MetricTagValueFormatter,
                new MetricValueDataKeys(Constants.ElasticsearchDefaults.CustomHistogramDataKeys, Constants.ElasticsearchDefaults.CustomMeterDataKeys));

            var formatter = new MetricDataValueSourceFormatter();
            formatter.Build(metricsData, payloadBuilder);

            return context.Response.WriteAsync(payloadBuilder.PayloadFormatted(), token);
        }
    }
}