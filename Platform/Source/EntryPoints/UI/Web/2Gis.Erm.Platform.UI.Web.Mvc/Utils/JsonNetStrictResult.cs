using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.Common.Serialization;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class JsonNetStrictResult : ActionResult
    {
        private static readonly DotNet2JavaScriptDateTimeConverter CustomDateTimeConverter = new DotNet2JavaScriptDateTimeConverter();
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings();

        public JsonNetStrictResult()
        {
            _serializerSettings.Converters.Add(CustomDateTimeConverter);
            Formatting = Formatting.None;
        }

        public JsonNetStrictResult(object data) : this()
        {
            Data = data;
        }

        public string ContentType { get; set; }
        public Encoding ContentEncoding { private get; set; }
        public Formatting Formatting { get; private set; }
        public object Data { get; private set; }

        public IList<JsonConverter> Converters
        {
            get { return _serializerSettings.Converters; }
        } 

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

            var serializer = JsonSerializer.Create(_serializerSettings);
            serializer.Serialize(writer, Data);
            writer.Flush();
        } 
    }
}