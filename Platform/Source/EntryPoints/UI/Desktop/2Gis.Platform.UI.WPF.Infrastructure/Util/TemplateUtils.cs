using System;
using System.Windows;
using System.Windows.Markup;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Util
{
    public static class TemplateUtils
    {
        public static DataTemplate CreateDataTemplate(Type viewModelType, Type viewType)
        {
            if (viewModelType == null)
            {
                throw new ArgumentNullException("viewModelType");
            }
            
            if (viewModelType.Namespace == null)
            {
                throw new InvalidOperationException("Viewmodel type has undefined namespace. Type: " + viewModelType);
            }

            if (viewType == null)
            {
                throw new ArgumentNullException("viewType");
            }

            if (viewType.Namespace == null)
            {
                throw new InvalidOperationException("View type has undefined namespace. Type: " + viewType);
            }

            var context = new ParserContext { XamlTypeMapper = new XamlTypeMapper(new string[0]) };
            context.XmlnsDictionary.Add(string.Empty, "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            context.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

            string xaml;
            if (viewModelType.IsGenericType)
            {
                var genericType = viewModelType.GetGenericTypeDefinition();
                var genericArgument = viewModelType.GenericTypeArguments[0];
                
                const string XamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}(m:{1})}}\"><v:{2} /></DataTemplate>";
                xaml = string.Format(XamlTemplate, genericType.GetNameWithoutGenericArity(), genericArgument.Name, viewType.Name);

                context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
                context.XamlTypeMapper.AddMappingProcessingInstruction("m", genericArgument.Namespace, genericArgument.Assembly.FullName);
                context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);

                context.XmlnsDictionary.Add("vm", "vm");
                context.XmlnsDictionary.Add("m", "m");
                context.XmlnsDictionary.Add("v", "v");
            }
            else
            {
                const string XamlTemplate = "<DataTemplate DataType=\"{{x:Type vm:{0}}}\"><v:{1} /></DataTemplate>";
                xaml = string.Format(XamlTemplate, viewModelType.Name, viewType.Name);

                context.XamlTypeMapper.AddMappingProcessingInstruction("vm", viewModelType.Namespace, viewModelType.Assembly.FullName);
                context.XamlTypeMapper.AddMappingProcessingInstruction("v", viewType.Namespace, viewType.Assembly.FullName);
                
                context.XmlnsDictionary.Add("vm", "vm");
                context.XmlnsDictionary.Add("v", "v");
            }

            var template = (DataTemplate)XamlReader.Parse(xaml, context);
            return template;
        }

        public static string GetNameWithoutGenericArity(this Type t)
        {
            var name = t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}
