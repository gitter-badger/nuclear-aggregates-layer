using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities
{
    public sealed class DomainEntityPropertyDescriptor : PropertyDescriptor
    {
        public DomainEntityPropertyDescriptor(Type type, string propertyName) : base(type, propertyName)
        {
            Entity = type.AsEntityName();
        }

        public EntityName Entity { get; private set; }
        
        public static new DomainEntityPropertyDescriptor Create<TEntity>(Expression<Func<TEntity, object>> propertyNameExpression)
            where TEntity : IEntity
        {
            var propertyName = StaticReflection.GetMemberName(propertyNameExpression);
            return new DomainEntityPropertyDescriptor(typeof(TEntity), propertyName);
        }
    }
}
