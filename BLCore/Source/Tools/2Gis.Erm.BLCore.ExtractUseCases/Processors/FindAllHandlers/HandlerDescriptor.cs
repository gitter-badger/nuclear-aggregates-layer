using Roslyn.Compilers.Common;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers
{
    public class HandlerDescriptor
    {
        public string RequestKey { get; set; }
        public INamedTypeSymbol RequestType { get; set; }
        public INamedTypeSymbol HandlerType { get; set; }
        public string HandlerKey { get; set; }

        public override string ToString()
        {
            return HandlerKey;
        }
    }
}