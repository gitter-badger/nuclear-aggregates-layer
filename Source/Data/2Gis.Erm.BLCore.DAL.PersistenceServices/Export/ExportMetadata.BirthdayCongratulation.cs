using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<BirthdayCongratulation> BirthdayCongratulation = QueryRuleContainer<BirthdayCongratulation>.Create(
            () => EntityOperationMapping<BirthdayCongratulation>.ForEntity(EntityName.BirthdayCongratulation)
                                                                .Operation<CreateIdentity>()
                                                                .Use((finder, ids) => finder.Find(Specs.Find.ByIds<BirthdayCongratulation>(ids))));
    }
}