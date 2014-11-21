﻿using System;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Factories.RequestHandling
{
    public sealed class UnityHandlerFactory : AbstractRequestHandlerFactory
    {
        private readonly IUnityContainer _unityContainer;

        public UnityHandlerFactory(IUnityContainer unityContainer, IRequestHandlerRepository handlerRepository)
            : base(handlerRepository)
        {
            _unityContainer = unityContainer;
        }

        protected override IRequestHandler GetHandlerInternal(Type handlerType, string handlerScope)
        {
            ProcessUseCaseAttributes(handlerType);
            return (IRequestHandler)_unityContainer.Resolve(handlerType, handlerScope);
        }

        private void ProcessUseCaseAttributes(Type handlerType)
        {
            UseCaseDuration specifiedDuration;
            if (!handlerType.TryGetUseCaseDuration(out specifiedDuration))
            {
                return;
            }

            _unityContainer.Resolve<IProcessingContext>().EnsureDurationConsidered(specifiedDuration);
        }
    }
}