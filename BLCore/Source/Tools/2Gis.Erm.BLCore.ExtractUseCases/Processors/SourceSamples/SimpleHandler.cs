using System;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.SourceSamples
{
    [Obsolete("Moving code to DeletePriceService needed")]
    internal sealed class DeletePriceHandler : RequestHandler<DeleteRequest<Price>, EmptyResponse>
    {
        protected override EmptyResponse Handle(DeleteRequest<Price> request)
        {
            throw new NotSupportedException("Current implementation is DeletePriceService");
        }
    }
}