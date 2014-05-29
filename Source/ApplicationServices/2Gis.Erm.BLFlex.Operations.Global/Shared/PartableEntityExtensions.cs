using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    // FIXME {d.ivanov, 19.05.2014}: Убрать
    public static class PartableEntityExtensions
    {
        [Obsolete]
        public static ChileLegalPersonPart ChilePart(this LegalPerson legalPerson)
        {
            return GetPart<ChileLegalPersonPart>(legalPerson);
        }

        [Obsolete]
        public static UkraineLegalPersonPart UkrainePart(this LegalPerson legalPerson)
        {
            return GetPart<UkraineLegalPersonPart>(legalPerson);
        }

        private static T GetPart<T>(IPartable entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            // COMMENT {v.lapeev, 19.05.2014}: Cast использовать нельзя, т.к. потенциально может быть много дополнений разных типов
            var parts = entity.Parts.OfType<T>();
            return parts.SingleOrDefault();
        }
    }
}
