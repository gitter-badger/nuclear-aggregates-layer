using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV
{
    public static class DynamicEntityPropertyMapper<TPropertyInstance> where TPropertyInstance : IDynamicEntityPropertyInstance
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
            Getters[typeof(Guid)] = x => new Guid(x.TextValue);
            Getters[typeof(DateTime)] = x => x.DateTimeValue;
            Getters[typeof(DateTime?)] = x => x != null ? x.DateTimeValue : null;
            Getters[typeof(ActivityPriority)] = x => x.NumericValue.HasValue ? (ActivityPriority)x.NumericValue : ActivityPriority.NotSet;
            Getters[typeof(ActivityStatus)] = x => x.NumericValue.HasValue ? (ActivityStatus)x.NumericValue : ActivityStatus.NotSet;
            Getters[typeof(ActivityPurpose)] = x => x.NumericValue.HasValue ? (ActivityPurpose)x.NumericValue : ActivityPurpose.NotSet;
            Getters[typeof(ActivityTaskType)] = x => x.NumericValue.HasValue ? (ActivityTaskType)x.NumericValue : ActivityTaskType.NotSet;
            Getters[typeof(EntityPartType)] = x => x.NumericValue.HasValue ? (EntityPartType)x.NumericValue : EntityPartType.NotSet;
            
            Setters[typeof(string)] = (x, y) => x.TextValue = (string)y;
            Setters[typeof(byte)] = (x, y) => x.NumericValue = (byte)y;
            Setters[typeof(int)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(long)] = (x, y) => x.NumericValue = (long)y;
            Setters[typeof(Guid)] = (x, y) => x.TextValue = ((Guid)y).ToString();
            Setters[typeof(DateTime)] = (x, y) => x.DateTimeValue = (DateTime)y;
            Setters[typeof(DateTime?)] = (x, y) => x.DateTimeValue = (DateTime?)y;
            Setters[typeof(ActivityPriority)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(ActivityStatus)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(ActivityPurpose)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(ActivityTaskType)] = (x, y) => x.NumericValue = (int)y;
            Setters[typeof(EntityPartType)] = (x, y) => x.NumericValue = (int)y;
        }

        public static Func<TPropertyInstance, object> GetGetter(Type propertyType)
        {
            return Getters[propertyType];
        }

        public static Action<TPropertyInstance, object> GetSetter(Type propertyType)
        {
            return Setters[propertyType];
        }
    }
}
