namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Mode
{
    public sealed class SharedFeatureAspect<TBuilder, TConfigElement> : ConfigElementBuilderAspectBase<TBuilder, ISharable, TConfigElement>
        where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
        where TConfigElement : ConfigElement, ISharable
    {
        public SharedFeatureAspect(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
            : base(aspectHostBuilder)
        {
        }

        public TBuilder Shared()
        {
            AspectHostBuilder.Features.Add(new SharedFeature());
            return AspectHostBuilder;
        }
    }
}
