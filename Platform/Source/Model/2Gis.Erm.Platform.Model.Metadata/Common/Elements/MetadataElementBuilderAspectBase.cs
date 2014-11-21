namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements
{
    public abstract class MetadataElementBuilderAspectBase<TBuilder, TBuilderAspect, TMetadataElement>
         where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
         where TBuilderAspect : IMetadataElementAspect
         where TMetadataElement : MetadataElement, TBuilderAspect 
    {
        private readonly TBuilder _aspectHostBuilder;

        protected MetadataElementBuilderAspectBase(MetadataElementBuilder<TBuilder, TMetadataElement> aspectHostBuilder)
        {
            _aspectHostBuilder = (TBuilder)aspectHostBuilder;
        }

        public TBuilder AspectHostBuilder
        {
            get { return _aspectHostBuilder; }
        }
    }
}
