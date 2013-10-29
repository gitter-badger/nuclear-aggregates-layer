namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class VerifierContext<TComparableContext> : IVerifierContext
        where TComparableContext : class
    {
        private readonly TComparableContext _comparableContext;
        private readonly OperationScopeNode _operationScopesHierarchy;

        public VerifierContext(OperationScopeNode operationScopesHierarchy, TComparableContext comparableContext)
        {
            _operationScopesHierarchy = operationScopesHierarchy;
            _comparableContext = comparableContext;
        }

        public TComparableContext ComparableContext
        {
            get { return _comparableContext; }
        }

        public OperationScopeNode OperationScopesHierarchy
        {
            get { return _operationScopesHierarchy; }
        }
    }
}