using System;

using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure.PolicyInjection
{
    // FIXME {all, 23.10.2013}: выпилить, убрав также references на interception
    [Obsolete("если в бизнесс требованиях нужно логирование, то нужно его делать явно, также есть межанизмы OperationLogging и ActionLogging")]
    public class JournalBusinessOperationHandler : ICallHandler
    {
        private readonly IJournalBusinessOperationsService _journalBusinessOperationsService;

        public JournalBusinessOperationHandler(IJournalBusinessOperationsService journalBusinessOperationsService)
        {
            _journalBusinessOperationsService = journalBusinessOperationsService;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var result = getNext()(input, getNext);
            if (result.Exception != null)
            {
                return result;
            }
            
            var propertyBagContainer = input.Target as IPropertyBagContainer;
            if (propertyBagContainer != null)
            {
                var propertyBag = propertyBagContainer.PropertyBag;
                _journalBusinessOperationsService.WriteJournalEntry(propertyBag);
            }
            return result;
        }

        public int Order { get; set; }
    }
}