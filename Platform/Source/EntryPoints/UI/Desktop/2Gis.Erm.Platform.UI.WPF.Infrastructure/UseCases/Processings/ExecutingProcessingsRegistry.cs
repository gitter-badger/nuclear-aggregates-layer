﻿using System;
using System.Collections.Concurrent;
using System.Linq;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings
{
    public sealed class ExecutingProcessingsRegistry : IExecutingProcessingsRegistry
    {
        private readonly ITracer _tracer;
        private readonly ConcurrentDictionary<Guid,IProcessingDescriptor> _registry = 
            new ConcurrentDictionary<Guid, IProcessingDescriptor>();

        public ExecutingProcessingsRegistry(ITracer tracer)
        {
            _tracer = tracer;
        }

        public IProcessingDescriptor StartProcessing(IProcessingDescriptor processingDescriptor)
        {
            if (_registry.TryAdd(processingDescriptor.Id, processingDescriptor))
            {
                _tracer.DebugFormat("Processing entry created. Processing: " + processingDescriptor);
            }

            return processingDescriptor;
        }

        public IProcessingDescriptor FinishProcessing(Guid processingId)
        {
            IProcessingDescriptor descriptor;
            if (_registry.TryRemove(processingId, out descriptor) && descriptor != null)
            {
                _tracer.DebugFormat("Processing entry removed. Processing: " + descriptor);
            }

            return descriptor;
        }

        public IProcessingDescriptor[] Processings
        {
            get
            {
                return _registry.Values.ToArray();
            }
        }
    }
}