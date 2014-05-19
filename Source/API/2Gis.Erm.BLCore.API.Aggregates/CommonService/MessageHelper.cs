using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Messages;

namespace DoubleGis.Erm.BLCore.API.Aggregates.CommonService
{
    public static class MessageHelper
    {
        public static string MakeMessage(int templateCode, string messageParameters)
        {
            if (!MessageCodesToTemplatesMapper.Map.ContainsKey(templateCode))
            {
                throw new ArgumentException(string.Format("Неизвестный код сообщения - {0}", templateCode), "templateCode");
            }

            var template = MessageCodesToTemplatesMapper.Map[templateCode].Invoke();

            return string.Format(template, ReadParameters(messageParameters));
        }

        public static string PrepareParametersContext(params object[] parameters)
        {
            var root = new XElement("Parameters");
            root.Add(parameters.Select(x => new XElement("Parameter", x)));
            return root.ToString();
        }

        public static string[] ReadParameters(string parametersContext)
        {
            var context = XElement.Parse(parametersContext);
            return context.Elements().Select(x => x.Value).ToArray();
        }
    }
}
