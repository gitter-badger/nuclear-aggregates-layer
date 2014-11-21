using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class VerifierContext<TComparableContext> : IVerifierContext
        where TComparableContext : class
    {
        private readonly TComparableContext _comparableContext;
        private readonly TrackedUseCase _useCase;

        public VerifierContext(TrackedUseCase useCase, TComparableContext comparableContext)
        {
            _useCase = useCase;
            _comparableContext = comparableContext;
        }
        
        public TrackedUseCase UseCase
        {
            get { return _useCase; }
        }

        public TComparableContext ComparableContext
        {
            get { return _comparableContext; }
        }
    }
}