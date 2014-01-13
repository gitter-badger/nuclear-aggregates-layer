using System;
using System.Diagnostics;
using System.Xml;

using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration
{
    /// <summary>
    /// Служит для выполнения вспомогательных операций при обработке локальных сообщений.
    /// Это можно было бы решить наследованием, но на наследование хендлеров наложено табу.
    /// </summary>
    public static class LocalMessageHelper
    {
        public static TData Read<TData>(string handlerName, XmlReader xmlReader, string nodeName, Func<XmlReader, TData, bool> readSingle)
            where TData : new()
        {
            Debug.Assert(xmlReader.NodeType == XmlNodeType.Element);
            Debug.Assert(xmlReader.Name == nodeName);

            // Пустой элемент с аттрибутами
            var accumulator = new TData();
            if (ElementEnd(xmlReader, nodeName) && xmlReader.HasAttributes)
            {
                readSingle(xmlReader, accumulator);                
            }
            else
            {
                while (!ElementEnd(xmlReader, nodeName) && xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    if (!readSingle(xmlReader, accumulator))
                    {
                        throw new NotificationException(string.Format("{0}: при разборе сообщения в xml-узле '{1}' обнаружен неожиданный узел '{2}'", handlerName, nodeName, xmlReader.Name));
                    }
                }

                if (!ElementEnd(xmlReader, nodeName))
                {
                    throw new NotificationException(string.Format("Неожиданный конец файла во время чтения узла '{0}'", nodeName));
                }
            }

            return accumulator;
        }

        public static bool ElementEnd(XmlReader xmlReader, string elementName)
        {
            return string.Equals(xmlReader.Name, elementName, StringComparison.OrdinalIgnoreCase) &&
                (xmlReader.NodeType == XmlNodeType.EndElement || (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsEmptyElement));
        }

        public static bool TryReadElementText(XmlReader xmlReader, string elementName, out string value)
        {
            value = string.Empty;
            if (!string.Equals(xmlReader.Name, elementName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (xmlReader.IsEmptyElement)
            {
                return true;
            }

            if (!xmlReader.Read())
            {
                return false;
            }

            value = xmlReader.Value;
            return true;
        }
    }
}
