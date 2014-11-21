using System;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.Common.Utils.Xml
{
    public static partial class XmlUtils
    {
        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression,
                                                                    Func<TProperty> getPropertyDelegate)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return new XElement(name, getPropertyDelegate());
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression,
                                                                   Func<TPropertyHost, TProperty> getPropertyDelegate)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return new XElement(name, getPropertyDelegate(host));
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression,
                                                                    Func<TProperty> getPropertyDelegate)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return CreateXAttribute(host, name, getPropertyDelegate());
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression,
                                                                   Func<TPropertyHost, TProperty> getPropertyDelegate)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return CreateXAttribute(host, name, getPropertyDelegate(host));
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression,
                                                                    TProperty propertyValue)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return new XElement(name, propertyValue);
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                            Expression<Func<TProperty>> propertyExpression,
                                                            object propertyValue)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return new XElement(name, propertyValue);
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression,
                                                                   TProperty propertyValue)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return CreateXAttribute(host, name, propertyValue);
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                           Expression<Func<TProperty>> propertyExpression,
                                                           object propertyValue)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return CreateXAttribute(host, name, propertyValue);
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return new XElement(name, propertyExpression.Compile());
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression)
        {
            string name = StaticReflection.GetMemberName(propertyExpression);
            return CreateXAttribute(host, name, propertyExpression.Compile()());
        }

        public static XElement Concat(this XElement first, XElement second)
        {
            var result = new XElement(first);
            result.Add(second.Elements());
            return result;
        }

        public static XElement ConcatBy(this XElement first, XElement second, XName tagName)
        {
            var result = new XElement(first);
            var firstTag = result.Element(tagName);
            var secondTag = second.Element(tagName);
            if (firstTag != null && secondTag != null)
            {
                firstTag.Add(secondTag.Elements());
            }

            return result;
        }

        private static XAttribute CreateXAttribute<TPropertyHost, TProperty>(TPropertyHost host, string name, TProperty value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(
                                name,
                                string.Format("An object [{0} ({1})] has null value of an attribute [{2}]",
                                                              host.GetType().Name,
                                                              host,
                                                              name));
            }

            return new XAttribute(name, value);
        }
    }
}
