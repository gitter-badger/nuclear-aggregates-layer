using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Emirates;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Russia;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    // TODO {d.ivanov, 19.02.2014}: По сути это - технический долг. Нужно сделать возможность дополнять доменную область и сущности в ней в зависимости от бизнес-модели
    // COMMENT {all, 07.04.2014}: большие сомнения насчет необходимости данного mapping, а также методов использования, для начала неплохо былобы каждый parts чтобы был помечен маркером, производным от Iadapted, тогда и доп. mapping станут не нужны
    public static class EntityAdaptationMapping
    {
        private static readonly Dictionary<Type, Type> EntityAdaptationParts = new Dictionary<Type, Type>
            {
                { typeof(ChileLegalPersonPart), typeof(IChileAdapted) },
                { typeof(UkraineLegalPersonPart), typeof(IUkraineAdapted) },
                { typeof(ChileLegalPersonProfilePart), typeof(IChileAdapted) },
                { typeof(RussiaLegalPersonProfilePart), typeof(IRussiaAdapted) },
                { typeof(UkraineLegalPersonProfilePart), typeof(IUkraineAdapted) },
                { typeof(ChileBranchOfficeOrganizationUnitPart), typeof(IChileAdapted) },
                { typeof(UkraineBranchOfficePart), typeof(IUkraineAdapted) },
                { typeof(EmiratesLegalPersonPart), typeof(IEmiratesAdapted) },
                { typeof(EmiratesLegalPersonProfilePart), typeof(IEmiratesAdapted) },
                { typeof(EmiratesClientPart), typeof(IEmiratesAdapted) },
                { typeof(EmiratesBranchOfficeOrganizationUnitPart), typeof(IEmiratesAdapted) },
                { typeof(EmiratesFirmAddressPart), typeof(IEmiratesAdapted) },
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