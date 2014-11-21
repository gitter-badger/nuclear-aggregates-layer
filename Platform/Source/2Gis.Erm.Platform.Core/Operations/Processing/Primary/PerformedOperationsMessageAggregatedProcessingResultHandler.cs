﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.Operations;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary
{
    public sealed class PerformedOperationsMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IOperationsFinalProcessingEnqueueAggregateService _operationsFinalProcessingEnqueueAggregateService;
        private readonly ICommonLog _logger;

        public PerformedOperationsMessageAggregatedProcessingResultHandler(
            IOperationsFinalProcessingEnqueueAggregateService operationsFinalProcessingEnqueueAggregateService,
            ICommonLog logger)
        {
            _operationsFinalProcessingEnqueueAggregateService = operationsFinalProcessingEnqueueAggregateService;
            _logger = logger;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();

            var originalMessageIds = new HashSet<Guid>();
            var arrgeratedResults = new List<PerformedOperationFinalProcessing>();


            foreach (var processingResultBucket in processingResultBuckets)
            {
                foreach (var processingResults in processingResultBucket.Value)
                {
                    var concreteProcessingResults = processingResults as PrimaryProcessingResultsMessage;
                    if (concreteProcessingResults == null)
                    {
                        continue;
                    }

                    originalMessageIds.Add(processingResultBucket.Key);
                    arrgeratedResults.AddRange(concreteProcessingResults.Results);
                }
            }

            try
            {
                _operationsFinalProcessingEnqueueAggregateService.Push(arrgeratedResults);

                foreach (var aggregatedResultsEntry in originalMessageIds)
                {
                    handlingResults.Add(aggregatedResultsEntry, MessageProcessingStage.Handle.EmptyResult().AsSucceeded());
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Can't push aggregated results of primary processing to final processing queue");
                foreach (var aggregatedResultsEntry in originalMessageIds)
                {
                    handlingResults.Add(aggregatedResultsEntry, MessageProcessingStage.Handle.EmptyResult().WithExceptions(ex).AsFailed());
                }
            }

            return handlingResults;
        }
    }
}