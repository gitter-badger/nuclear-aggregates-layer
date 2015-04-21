namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete
{
    public sealed class NamedHandlerFeature : IHandlerFeature
    {
        public NamedHandlerFeature(string handlerName)
        {
            HandlerName = handlerName;
        }

        public string HandlerName { get; private set; }

        public override string ToString()
        {
            return HandlerName;
        }
    }
}
