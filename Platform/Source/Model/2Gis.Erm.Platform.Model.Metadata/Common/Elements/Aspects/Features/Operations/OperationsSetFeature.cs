using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations
{
    public sealed class OperationsSetFeature : IMetadataFeature
    {
        private readonly List<OperationFeature> _operationFeatures = new List<OperationFeature>();

        public ICollection<OperationFeature> OperationFeatures
        {
            get
            {
                return _operationFeatures;
            }
        }
    }
}