using System;
using System.Text;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.Common.Serialization;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class JsonNetStrictResult : ActionResult
    {
        private static readonly DotNet2JavaScriptDateTimeConverter CustomDateTimeConverter = new DotNet2JavaScriptDateTimeConverter();

        public JsonNetStrictResult()
        {
            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.Converters.Add(CustomDateTimeConverter);

            Formatting = Formatting.None;
        }

        public JsonNetStrictResult(object data) : this()
        {
            Data = data;
        }

        public Encoding ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public object Data { get; set; }

        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data == null)
            {
                return;
            }

            var writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

            var serializer = JsonSerializer.Create(SerializerSettings);
            serializer.Serialize(writer, Data);
            writer.Flush();
        } 
    }
}