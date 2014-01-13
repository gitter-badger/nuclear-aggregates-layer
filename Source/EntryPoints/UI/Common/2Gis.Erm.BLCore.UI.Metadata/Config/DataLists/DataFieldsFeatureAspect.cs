using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataFieldsFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, DataListStructure, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : DataListStructure
    {
        public DataFieldsFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder) : base(aspectHostBuilder)
        {
        }

        public TBuilder Attach(params DataFieldStructure[] dataFields)
        {
            if (dataFields == null || dataFields.Length == 0)
            {
                return AspectHostBuilder;
            }

            var dataFieldsFeature = AspectHostBuilder.Features.OfType<DataFieldsFeature>().SingleOrDefault();
            if (dataFieldsFeature == null)
            {
                dataFieldsFeature = new DataFieldsFeature(Enumerable.Empty<DataFieldStructure>());
                AspectHostBuilder.Features.Add(dataFieldsFeature);
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