using System;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    public static class AggregateDescriptorUtils
    {
        public static AggregateDescriptor ToDescriptor<TEnum>(this TEnum aggregateRoot)
            where TEnum : struct
        {
            var aggregateAliasType = typeof(TEnum);
            var aliasEnumValues = (TEnum[])Enum.GetValues(aggregateAliasType);
            var entityNames = Array.ConvertAll(aliasEnumValues, input => input.AsEntityName());

            return new AggregateDescriptor(aggregateRoot.AsEntityName(), entityNames, aggregateAliasType);
        }

        public static EntityName AsEntityName<TEnum>(this TEnum aggregateMember)
            where TEnum : struct
        {
            return (EntityName)Enum.ToObject(typeof(EntityName), aggregateMember);
        }

        public static TEnum AsAggregate<TEnum>(this EntityName entityName)
            where TEnum : struct 
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), entityName);
        }
    }
}
