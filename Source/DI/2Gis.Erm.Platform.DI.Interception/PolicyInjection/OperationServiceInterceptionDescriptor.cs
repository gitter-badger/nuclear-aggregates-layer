using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Interception.PolicyInjection
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public interface IOperationServiceInterceptionDescriptor<out TOperation> where TOperation : IOperation
    {
        Type OperationServiceType { get; }
        IEnumerable<InjectionParameter> HandlerInjectionParameters { get; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class OperationServiceInterceptionDescriptor<TOperation> : IOperationServiceInterceptionDescriptor<TOperation>
        where TOperation : IOperation
    {
        private readonly object[] _handlerInjectionParameters;

        public OperationServiceInterceptionDescriptor(params object[] handlerInjectionParameters)
        {
            _handlerInjectionParameters = handlerInjectionParameters;
        }

        public Type OperationServiceType
        {
            get { return typeof(TOperation); }
        }

        public IEnumerable<InjectionParameter> HandlerInjectionParameters
        {
            get
            {
                return _handlerInjectionParameters != null
                           ? _handlerInjectionParameters.Select(x => new InjectionParameter(x))
                           : Enumerable.Empty<InjectionParameter>();
            }
        }
    }
}