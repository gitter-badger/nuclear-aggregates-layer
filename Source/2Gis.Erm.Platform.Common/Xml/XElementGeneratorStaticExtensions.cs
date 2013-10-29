using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.Common.Xml
{
    public static class XElementGeneratorStaticExtensions
    {
        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression,
                                                                    Func<TProperty> getPropertyDelegate)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XElement(name, getPropertyDelegate());
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression,
                                                                   Func<TPropertyHost, TProperty> getPropertyDelegate)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XElement(name, getPropertyDelegate(host));
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression,
                                                                    Func<TProperty> getPropertyDelegate)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XAttribute(name, getPropertyDelegate());
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression,
                                                                   Func<TPropertyHost, TProperty> getPropertyDelegate)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XAttribute(name, getPropertyDelegate(host));
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression,
                                                                    TProperty propertyValue)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XElement(name, propertyValue);
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                            Expression<Func<TProperty>> propertyExpression,
                                                            object propertyValue)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XElement(name, propertyValue);
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression,
                                                                   TProperty propertyValue)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XAttribute(name, propertyValue);
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                           Expression<Func<TProperty>> propertyExpression,
                                                           object propertyValue)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XAttribute(name, propertyValue);
        }

        public static XElement ToXElement<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                    Expression<Func<TProperty>> propertyExpression)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XElement(name, propertyExpression.Compile());
        }

        public static XAttribute ToXAttribute<TPropertyHost, TProperty>(this TPropertyHost host,
                                                                   Expression<Func<TProperty>> propertyExpression)
        {
            string name = ExtractPropertyName(propertyExpression);
            return new XAttribute(name, propertyExpression.Compile());
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

        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }
    }
}
