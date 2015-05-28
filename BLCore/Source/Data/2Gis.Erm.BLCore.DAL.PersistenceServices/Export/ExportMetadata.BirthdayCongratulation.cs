using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<BirthdayCongratulation> BirthdayCongratulation = QueryRuleContainer<BirthdayCongratulation>.Create(
            () => EntityOperationMapping<BirthdayCongratulation>.ForEntity(EntityType.Instance.BirthdayCongratulation())
                                                                .Operation<CreateIdentity>()
                                                                .Use((query, ids) => query.For(Specs.Find.ByIds<BirthdayCongratulation>(ids))));
    }
}