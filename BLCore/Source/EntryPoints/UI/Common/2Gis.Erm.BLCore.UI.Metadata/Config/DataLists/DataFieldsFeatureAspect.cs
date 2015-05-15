using System.Linq;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldsFeatureAspect<TBuilder, TMetadataElement> : MetadataElementBuilderAspectBase<TBuilder, DataListMetadata, TMetadataElement>
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
        where TMetadataElement : DataListMetadata
    {
        public DataFieldsFeatureAspect(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder) : base(aspectHostBuilder)
        {
        }

        public TBuilder Attach(params DataFieldMetadata[] dataFields)
        {
            if (dataFields == null || dataFields.Length == 0)
            {
                return AspectHostBuilder;
            }

            var dataFieldsFeature = AspectHostBuilder.Features.OfType<DataFieldsFeature>().SingleOrDefault();
            if (dataFieldsFeature == null)
            {
                dataFieldsFeature = new DataFieldsFeature(Enumerable.Empty<DataFieldMetadata>());
                AspectHostBuilder.WithFeatures(dataFieldsFeature);
            }

            foreach (var dataField in dataFields)
            {
                dataFieldsFeature.DataFields.Add(dataField);
                foreach (var extraDataField in dataField.ExtraDataFields)
                {
                    dataFieldsFeature.DataFields.Add(extraDataField);
                }
            }

            return AspectHostBuilder;
        }
    }
}