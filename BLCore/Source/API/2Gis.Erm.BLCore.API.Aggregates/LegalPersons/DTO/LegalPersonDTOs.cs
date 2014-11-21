using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class LegalPersonWithProfiles
    {
        public LegalPerson LegalPerson { get; set; }
        public IEnumerable<LegalPersonProfile> Profiles { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class LegalPersonName
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class LegalPersonFor1CExportDto
    {
        public LegalPersonProfile Profile { get; set; }
        public LegalPerson LegalPerson { get; set; }
        public string LegalPersonSyncCode1C { get; set; }
    }
}