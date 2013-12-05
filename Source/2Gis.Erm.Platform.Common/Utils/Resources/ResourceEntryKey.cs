using System;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.Common.Utils.Resources
{
    public sealed class ResourceEntryKey : IEquatable<ResourceEntryKey>
    {
        private readonly Type _resourceHostType;
        private readonly string _resourceEntryName;

        public ResourceEntryKey(Type resourceHostType, string resourceEntryName)
        {
            if (resourceHostType == null)
            {
                throw new ArgumentNullException("resourceHostType");
            }

            if (resourceEntryName == null)
            {
                throw new ArgumentNullException("resourceEntryName");
            }

            _resourceHostType = resourceHostType;
            _resourceEntryName = resourceEntryName;
        }

        public Type ResourceHostType
        {
            get
            {
                return _resourceHostType;
            }
        }

        public string ResourceEntryName
        {
            get
            {
                return _resourceEntryName;
            }
        }

        public static ResourceEntryKey Create(Expression<Func<object>> resourceEntryExpression)
        {
            var resourceEntryName = StaticReflection.GetMemberName(resourceEntryExpression);
            var resourceEntryHost = StaticReflection.GetMemberDeclaringType(resourceEntryExpression);

            return new ResourceEntryKey(resourceEntryHost, resourceEntryName);
        }

        public static ResourceEntryKey Create<TKey>(Expression<Func<TKey>> resourceEntryExpression)
        {
            var resourceEntryName = StaticReflection.GetMemberName(resourceEntryExpression);
            var resourceEntryHost = StaticReflection.GetMemberDeclaringType(resourceEntryExpression);

            return new ResourceEntryKey(resourceEntryHost, resourceEntryName);
        }

        public static bool operator ==(ResourceEntryKey first, ResourceEntryKey second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(ResourceEntryKey first, ResourceEntryKey second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(ResourceEntryKey) != obj.GetType())
            {
                return false;
            }

            var other = (ResourceEntryKey)obj;

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _resourceHostType == other._resourceHostType && string.CompareOrdinal(_resourceEntryName, other._resourceEntryName) == 0;
        }

        public override int GetHashCode()
        {
            int hash = 269;
            hash = (hash * 47) + _resourceHostType.GetHashCode();
            hash = (hash * 47) + _resourceEntryName.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return _resourceHostType.FullName + "+" + _resourceEntryName;
        }

        bool IEquatable<ResourceEntryKey>.Equals(ResourceEntryKey other)
        {
            return Equals(other);
        }
    }
}