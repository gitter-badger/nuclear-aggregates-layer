using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public static class OperatiosFeatureUtils
    {
        public static TBuilder AddOperation<TBuilder, TConfigElement>(this TBuilder builder, IBoundOperationFeature operationFeature)
            where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
            where TConfigElement : ConfigElement
        {
            var batchOperationFeature = builder.Features.OfType<BatchOperationFeature>().SingleOrDefault();
            if (batchOperationFeature == null)
            {
                batchOperationFeature = new BatchOperationFeature();
                builder.Features.Add(batchOperationFeature);
            }

            batchOperationFeature.OperationFeatures.Add(operationFeature);
            return builder;
        }
    }
}
