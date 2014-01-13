using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<FirmAddress> FirmAddress = QueryRuleContainer<FirmAddress>.Create(
            () => EntityOperationMapping<FirmAddress>.ForEntity(EntityName.FirmAddress)
                                                     .Operation<SpecifyAdditionalServicesIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<FirmAddress>(ids))),
            () => EntityOperationMapping<FirmAddress>.ForEntity(EntityName.Firm)
                                                     .Operation<SpecifyAdditionalServicesIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Firm>(ids))
                                                                                 .SelectMany(firm => firm.FirmAddresses)));
    }
}
