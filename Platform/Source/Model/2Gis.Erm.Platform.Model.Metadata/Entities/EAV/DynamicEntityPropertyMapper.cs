using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class DynamicEntityPropertyMapper<TPropertyInstance> where TPropertyInstance : class, IDynamicEntityPropertyInstance
    {
        private static readonly Dictionary<Type, Func<TPropertyInstance, object>> Getters =
            new Dictionary<Type, Func<TPropertyInstance, object>>();

        private static readonly Dictionary<Type, Action<TPropertyInstance, object>> Setters =
            new Dictionary<Type, Action<TPropertyInstance, object>>();

        static DynamicEntityPropertyMapper()
        {
            Getters[typeof(string)] = x => x.TextValue;
            Getters[typeof(byte)] = x => Convert.ToByte(x.NumericValue);
            Getters[typeof(int)] = x => Convert.ToInt32(x.NumericValue);
            Getters[typeof(long)] = x => Convert.ToInt64(x.NumericValue);
            Getters[typeof(long?)] = x => x == null ? null : x.NumericValue.HasValue ? (long?)Convert.ToInt64(x.NumericValue.Value) : null;
            Getters[typeof(Guid)] = x => new Guid(x.TextValue);
            Getters[typeof(DateTime)] = x => x.DateTimeValue;
            Getters[typeof(DateTime?)] = x => x != null ? x.DateTimeValue : null;
            Getters[typeof(IEntityType)] = x => x.NumericValue.HasValue ? EntityType.Instance.Parse((int)x.NumericValue) : EntityType.Instance.None();
            Getters[typeof(AccountType)] = x => x.NumericValue.HasValue ? (AccountType)x.NumericValue : AccountType.NotSet;
            Getters[typeof(TaxationType)] = x => x.NumericValue.HasValue ? (TaxationType)x.NumericValue : TaxationType.NotSet;
            
            Setters[typeof(BusinessModel)] = (x, y) => x.NumericValue = (int)y;

            Setters[typeof(string)] = (x, y) => x.TextValue = (string)y;
            Setters[typeof(byte)] = (x, y) => x.NumericValue = (byte)y;
            Setters[typeof(int)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(long)] = (x, y) => x.NumericValue = (long)y;
            Setters[typeof(long?)] = (x, y) => x.NumericValue = (long?)y;
            Setters[typeof(Guid)] = (x, y) => x.TextValue = ((Guid)y).ToString();
            Setters[typeof(DateTime)] = (x, y) => x.DateTimeValue = (DateTime)y;
            Setters[typeof(DateTime?)] = (x, y) => x.DateTimeValue = (DateTime?)y;
            Setters[typeof(IEntityType)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(AccountType)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(TaxationType)] = (x, y) => x.NumericValue = (int)y;
        }

        public static Func<TPropertyInstance, object> GetGetter(Type propertyType)
        {
            return Getters[ConvertToContact(propertyType)];
        }

        public static Action<TPropertyInstance, object> GetSetter(Type propertyType)
        {
            return Setters[ConvertToContact(propertyType)];
        }

        private static Type ConvertToContact(Type type)
        {
            var entityType = typeof(IEntityType);
            return entityType.IsAssignableFrom(type) ? entityType : type;
        }
    }
}
