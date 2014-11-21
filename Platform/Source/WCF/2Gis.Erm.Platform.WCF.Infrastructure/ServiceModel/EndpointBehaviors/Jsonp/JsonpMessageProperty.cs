using System.ServiceModel.Channels;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Jsonp
{
    public class JsonpMessageProperty : IMessageProperty
    {
        public const string Name = "DoubleGis.Erm.WCF.Infrastructure.ServiceModel.JsonpMessageProperty";

        public JsonpMessageProperty()
        {
        }

        internal JsonpMessageProperty(JsonpMessageProperty other)
        {
            MethodName = other.MethodName;
        }

        public string MethodName { get; set; }

        public IMessageProperty CreateCopy()
        {
            return new JsonpMessageProperty(this);
        }
    }
}