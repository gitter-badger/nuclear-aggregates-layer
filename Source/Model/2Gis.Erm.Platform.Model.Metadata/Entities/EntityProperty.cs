using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public sealed class EntityProperty : IConfigElement<OrdinaryConfigElementIdentity> 
    {
        private readonly OrdinaryConfigElementIdentity _identity = new OrdinaryConfigElementIdentity();

        public EntityProperty()
        {
            Features = new IPropertyFeature[0];
        }

        public EntityProperty(string name, Type propertyType)
        {
            Name = name;
            Type = propertyType;
            Features = new IPropertyFeature[0];
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public Type DeclaringType { get; set; }

        #region IConfigElement

        IConfigElementIdentity IConfigElement.ElementIdentity
        {
            get
            {
                return _identity;
            }
        }

        OrdinaryConfigElementIdentity IConfigElement<OrdinaryConfigElementIdentity>.Identity
        {
            get
            {
                return _identity;
            }
        }

        IEnumerable<IConfigFeature> IConfigElement.ElementFeatures 
        {
            get
            {
                return Features;
            }
        }

        public IConfigElement Parent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int DeepLevel
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IConfigElement[] Elements
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion


        public IEnumerable<IPropertyFeature> Features
        {
            get; 
            private set;
        }

        public static EntityProperty Create<TKey>(Expression<Func<TKey, object>> propertyExpression)
        {
            var propertyName = StaticReflection.GetMemberName(propertyExpression);
            var propertyType = StaticReflection.GetMemberType(propertyExpression);
            var declaringType = StaticReflection.GetMemberDeclaringType(propertyExpression);
            return new EntityProperty(propertyName, propertyType) { DeclaringType = declaringType };
        }

        public EntityProperty WithFeatures(params IPropertyFeature[] features)
        {
            Features = features;
            foreach (var feature in features)
            {
                feature.TargetProperty = this;
            }

            return this;
        }

        public override string ToString()
        {
            return string.Format("{0}. {1}. {2}", DeclaringType.Name, Type.Name, Name);
        }
    }
}
