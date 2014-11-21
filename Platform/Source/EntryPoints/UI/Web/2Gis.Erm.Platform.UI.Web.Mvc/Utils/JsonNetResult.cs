using System.Web.Mvc;

using DoubleGis.Erm.Platform.Common.Serialization;

using Newtonsoft.Json.Converters;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class JsonNetResult : ActionResult
    {
        private static readonly StringEnumConverter CustomEnumConverter = new StringEnumConverter();
        private static readonly Int64ToStringConverter Int64ToStringConverter = new Int64ToStringConverter();
        private readonly JsonNetStrictResult _jsonNetStrictResult = new JsonNetStrictResult();

        public JsonNetResult()
        {
            _jsonNetStrictResult.SerializerSettings.Converters.Add(CustomEnumConverter);
            _jsonNetStrictResult.SerializerSettings.Converters.Add(Int64ToStringConverter);
        }

        public JsonNetResult(object data) : this()
        {
            _jsonNetStrictResult.Data = data;
        }

        public string ContentType
        {
            get
            {
                return _jsonNetStrictResult.ContentType;
            }
            set
            {
                _jsonNetStrictResult.ContentType = value;
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            _jsonNetStrictResult.ExecuteResult(context);
        }
    }
}