using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    // TODO {all, 14.02.2014}: Вероятно, рано или поздно Core сделают аналогичный функционал или заберут этот
    public static class PartableEntityExtensions
    {
        public static ChileLegalPersonProfilePart ChilePart(this LegalPersonProfile profile)
        {
            return GetPart<ChileLegalPersonProfilePart>(profile);
        }

        public static ChileLegalPersonPart ChilePart(this LegalPerson legalPerson)
        {
            return GetPart<ChileLegalPersonPart>(legalPerson);
        }

        public static ChileBranchOfficeOrganizationUnitPart ChilePart(this BranchOfficeOrganizationUnit entity)
        {
            return GetPart<ChileBranchOfficeOrganizationUnitPart>(entity);
        }

        public static UkraineLegalPersonProfilePart UkrainePart(this LegalPersonProfile profile)
        {
            return GetPart<UkraineLegalPersonProfilePart>(profile);
        }

        public static UkraineLegalPersonPart UkrainePart(this LegalPerson legalPerson)
        {
            return GetPart<UkraineLegalPersonPart>(legalPerson);
        }

        public static UkraineBranchOfficePart UkrainePart(this BranchOffice entity)
        {
            return GetPart<UkraineBranchOfficePart>(entity);
        }

        private static T GetPart<T>(IPartable entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var parts = entity.Parts.Cast<T>();
            return parts.Single();
        }
    }
}
