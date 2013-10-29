using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings
{
    public sealed class OperationProcessingDescriptor : IOperationProcessingDescriptor
    {
        private readonly Guid _operationToken;
        private readonly int _processingItemsCount;
        private readonly Guid? _progressConsumerId;
        private readonly object _synch = new object();
        private readonly List<IOperationResult> _currentResults = new List<IOperationResult>();

        public OperationProcessingDescriptor(Guid operationToken, int processingItemsCount, Guid? progressConsumerId)
        {
            _operationToken = operationToken;
            _processingItemsCount = processingItemsCount;
            _progressConsumerId = progressConsumerId;
        }

        public Guid Id 
        {
            get { return _operationToken; }
        }

        public int ProcessingItemsCount
        {
            get { return _processingItemsCount; }
        }

        public IOperationResult[] CurrentProgress 
        {
            get
            {
                lock (_synch)
                {
                    return _currentResults.ToArray();
                }
            }
        }

        public bool IsFinished 
        {
            get 
            {
                lock (_synch)
                {
                    return _currentResults.Count == _processingItemsCount;
                }
            }
        }

        public Guid? ProgressConsumerId
        {
            get { return _progressConsumerId; }
        }

        public void UpdateOperationProgress(IOperationResult[] operationsResults)
        {
            lock (_synch)
            {
                if (_currentResults.Count + operationsResults.Length == _processingItemsCount)
                {
                    throw new InvalidOperationException("Can't add operation result - total results count exceed specified processing items count");
                }

                _currentResults.AddRange(operationsResults);
            }
        }
    }
}