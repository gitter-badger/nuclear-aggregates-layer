using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    // TODO {d.ivanov, 19.02.2014}: По сути это - технический долг. Нужно сделать возможность дополнять доменную область и сущности в ней в зависимости от бизнес-модели
    public static class EntityAdaptationMapping
    {
        private static readonly Dictionary<Type, Type> EntityAdaptationParts = new Dictionary<Type, Type>
            {
                { typeof(LegalPersonPart), typeof(IChileAdapted) },
                { typeof(LegalPersonProfilePart), typeof(IChileAdapted) },
                { typeof(BranchOfficeOrganizationUnitPart), typeof(IChileAdapted) }
            };

        public static bool IsAdapted(this Type type)
        {
            if (!typeof(IEntity).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format("The given type - {0} - is not an entity", type.Name));
            }

            return EntityAdaptationParts.ContainsKey(type);
        }

        public static Type AsAdapted(this Type type)
        {
            if (!typeof(IEntity).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format("The given type - {0} - is not an entity", type.Name));
            }

            Type adated;
            if (!EntityAdaptationParts.TryGetValue(type, out adated))
            {
                throw new NotSupportedException(string.Format("The given entity - {0} - is not an adapted", type.Name));
            }

            return adated;
        }
    }
}