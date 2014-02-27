using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared
{
    // TODO {all, 14.02.2014}: Вероятно, рано или поздно Core сделают аналогичный функционал или заберут этот
    public static class LegalPersonProfileExtensions
    {
        public static LegalPersonProfilePart ChilePart(this LegalPersonProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException("profile");
            }

            var parts = profile.Parts.OfType<LegalPersonProfilePart>();
            return parts.Single();
        }
    }
}
