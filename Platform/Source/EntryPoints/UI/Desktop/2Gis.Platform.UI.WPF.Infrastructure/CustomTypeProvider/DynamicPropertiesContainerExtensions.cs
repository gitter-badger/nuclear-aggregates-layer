using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Common.Utils;
using DoubleGis.Erm.Model.Entities.Interfaces;
using DoubleGis.UI.WPF.Infrastructure.CustomTypeProvider;

namespace DoubleGis.Erm.UI.WPF.Client.Util
{
    public static class DynamicPropertiesContainerExtensions
    {
        public static void AddProperty<TKey>(this IDynamicPropertiesContainerConfigurator containerConfigurator, Expression<Func<TKey, object>> propertyExpression) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            var propertyType = StaticReflection.GetMemberType(propertyExpression);
            containerConfigurator.AddProperty(propertyName, propertyType);
        }

        public static void AddProperty<TKey>(this IDynamicPropertiesContainerConfigurator containerConfigurator, Expression<Func<TKey, object>> propertyExpression, object value, IEnumerable<Attribute> attributes) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            var propertyType = StaticReflection.GetMemberType(propertyExpression);
            containerConfigurator.AddProperty(propertyName, propertyType, value, attributes);
        }

        public static void AddProperty<TKey, TProp>(this IDynamicPropertiesContainerConfigurator containerConfigurator, Expression<Func<TKey, TProp>> propertyExpression) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            var propertyType = StaticReflection.GetMemberType(propertyExpression);
            containerConfigurator.AddProperty(propertyName, propertyType);
        }

        public static void AddProperty<TKey, TProp>(this IDynamicPropertiesContainerConfigurator containerConfigurator, Expression<Func<TKey, TProp>> propertyExpression, TProp value, IEnumerable<Attribute> attributes) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            var propertyType = StaticReflection.GetMemberType(propertyExpression);
            containerConfigurator.AddProperty(propertyName, propertyType, value, attributes);
        }
        
        public static object GetPropertyValue<TKey>(this IDynamicPropertiesContainer dynamicPropertiesContainer, Expression<Func<TKey, object>> propertyExpression) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            return dynamicPropertiesContainer.GetPropertyValue(propertyName);
        }

        public static void SetPropertyValue<TKey>(this IDynamicPropertiesContainer dynamicPropertiesContainer, Expression<Func<TKey, object>> propertyExpression, object value) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            dynamicPropertiesContainer.SetPropertyValue(propertyName, value);
        }

        public static TProp GetPropertyValue<TKey, TProp>(this IDynamicPropertiesContainer dynamicPropertiesContainer, Expression<Func<TKey, TProp>> propertyExpression) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            return (TProp) dynamicPropertiesContainer.GetPropertyValue(propertyName);
        }

        public static void SetPropertyValue<TKey, TProp>(this IDynamicPropertiesContainer dynamicPropertiesContainer, Expression<Func<TKey, TProp>> propertyExpression, TProp value) where TKey : IDomainEntityDto
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            dynamicPropertiesContainer.SetPropertyValue(propertyName, value);
        }
    }
}