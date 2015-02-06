using System.Web.Mvc;

using DoubleGis.Erm.Platform.Common.Serialization;

using Newtonsoft.Json.Converters;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class JsonNetResult : ActionResult
    {
        private static readonly StringEnumConverter CustomEnumConverter = new StringEnumConverter();
        private static readonly Int64ToStringConverter Int64ToStringConverter = new Int64ToStringConverter();
        private static readonly EntityTypeToStringConverter EntityTypeToStringConverter = new EntityTypeToStringConverter();
        private readonly JsonNetStrictResult _jsonNetStrictResult;

        public JsonNetResult()
        {
            _jsonNetStrictResult = new JsonNetStrictResult();
        }

        public JsonNetResult(object data)
        {
            _jsonNetStrictResult = new JsonNetStrictResult(data);

            _jsonNetStrictResult.Converters.Add(CustomEnumConverter);
            _jsonNetStrictResult.Converters.Add(Int64ToStringConverter);
            _jsonNetStrictResult.Converters.Add(EntityTypeToStringConverter);
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