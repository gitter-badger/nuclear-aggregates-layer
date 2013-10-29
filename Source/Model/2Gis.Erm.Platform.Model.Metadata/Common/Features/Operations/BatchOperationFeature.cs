using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public sealed class BatchOperationFeature : IConfigFeature
    {
        private readonly List<IBoundOperationFeature> _operationFeatures = new List<IBoundOperationFeature>();

        public ICollection<IBoundOperationFeature> OperationFeatures
        {
            get
            {
                return _operationFeatures;
            }
        }
    }
}