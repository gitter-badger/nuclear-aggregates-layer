using System;
using System.Reflection;
using System.Windows.Markup;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.MarkupExtensions
{
    /// <summary>
    ///  Class for Xaml markup extension for static field and property references.
    /// </summary> 
    public class StaticExtension : MarkupExtension
    {
        public StaticExtension()
        {
        }

        /// <summary>
        ///  The static field or property represented by a string.  This string is
        ///  of the format Prefix:ClassName.FieldOrPropertyName.  The Prefix is 
        ///  optional, and refers to the XML prefix in a Xaml file.
        /// </summary> 
        private string _member;
        public string Member
        {
            get { return _member; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Member");
                }
                _member = value;
            }
        }

        /// <summary>
        ///  Return an object that should be set on the targetObject's targetProperty 
        ///  for this markup extension.  For a StaticExtension this is a static field 
        ///  or property value.
        /// </summary> 
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_member == null)
                throw new InvalidOperationException("member cannot be null");

            // Validate the _member 
            int dotIndex = _member.IndexOf('.');
            if (dotIndex < 0)
                throw new ArgumentException("dotIndex");

            // Pull out the type substring (this will include any XML prefix, e.g. "av:Button") 
            string typeString = _member.Substring(0, dotIndex);
            if (typeString == string.Empty)
                throw new ArgumentException("typeString");

            // Get the IXamlTypeResolver from the service provider
            var xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
            if (xamlTypeResolver == null)
                throw new ArgumentException("xamlTypeResolver");

            // Use the type resolver to get a Type instance 
            Type type = xamlTypeResolver.Resolve(typeString);

            // Get the member name substring
            string fieldString = _member.Substring(dotIndex + 1, _member.Length - dotIndex - 1);
            if (fieldString == string.Empty)
                throw new ArgumentException("fieldString");

            // Use the built-in parser for enum types
            if (type.IsEnum)
            {
                return Enum.Parse(type, fieldString, true);
            }

            // For other types, reflect 
            bool found = false;
            object value = null;

            object fieldOrProp = type.GetField(fieldString, BindingFlags.Public |
                                                            BindingFlags.FlattenHierarchy | BindingFlags.Static);
            if (fieldOrProp == null)
            {
                fieldOrProp = type.GetProperty(fieldString, BindingFlags.Public |
                                                            BindingFlags.FlattenHierarchy | BindingFlags.Static);
                if (fieldOrProp is PropertyInfo)
                {
                    value = ((PropertyInfo)fieldOrProp).GetValue(null, null);
                    found = true;
                }
            }
            else if (fieldOrProp is FieldInfo)
            {
                value = ((FieldInfo)fieldOrProp).GetValue(null);
                found = true;
            }

            if (found)
                return value;
            else
                throw new ArgumentException("not found");
        }
    }
}
