using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities
{
    public sealed class PropertyDescriptor : IPropertyDescriptor
    {
        public PropertyDescriptor(EntityName entity, string propertyName)
        {
            PropertyName = propertyName;
            Entity = entity;
        }

        public string PropertyName { get; private set; }
        public EntityName Entity { get; private set; }
        
        public static PropertyDescriptor Create<TEntity>(Expression<Func<TEntity, object>> propertyNameExpression)
            where TEntity : IEntity
        {
            var propertyName = StaticReflection.GetMemberName(propertyNameExpression);
            return new PropertyDescriptor(typeof(TEntity).AsEntityName(), propertyName);
        }
    }
}
