using System;

using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Concrete;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure.PolicyInjection
{
    // FIXME {all, 23.10.2013}: выпилить, убрав также references на interception
    [Obsolete("если в бизнесс требованиях нужно логирование, то нужно его делать явно, также есть межанизмы OperationLogging и ActionLogging")]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class JournalBusinessOperationAttribute : HandlerAttribute
    {
        private readonly BusinessOperation _operation;

        public JournalBusinessOperationAttribute(BusinessOperation operation)
        {
            _operation = operation;
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            IJournalBusinessOperationsService service;
            switch (_operation)
            {
                case BusinessOperation.MakeRegionalAdsDocs:
                    service = container.Resolve<IJournalMakeRegionalAdsDocsService>(Mapping.SimplifiedModelConsumerScope);
                    break;
                default:
                    service = new NullJournalBusinessOperationsService();
                    break;
            }

            return new JournalBusinessOperationHandler(service);
        }
    }
}