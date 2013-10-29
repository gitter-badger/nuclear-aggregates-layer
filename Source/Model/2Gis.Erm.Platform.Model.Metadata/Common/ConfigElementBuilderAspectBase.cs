namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public abstract class ConfigElementBuilderAspectBase<TBuilder, TBuilderAspect, TConfigElement>
         where TBuilder : ConfigElementBuilder<TBuilder, TConfigElement>, new()
         where TBuilderAspect : IConfigElementAspect
         where TConfigElement : ConfigElement, TBuilderAspect 
    {
        private readonly TBuilder _aspectHostBuilder;

        protected ConfigElementBuilderAspectBase(ConfigElementBuilder<TBuilder, TConfigElement> aspectHostBuilder)
        {
            _aspectHostBuilder = (TBuilder)aspectHostBuilder;
        }

        public TBuilder AspectHostBuilder
        {
            get { return _aspectHostBuilder; }
        }
    }
}
