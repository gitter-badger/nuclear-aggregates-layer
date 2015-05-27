using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<Position> Position = QueryRuleContainer<Position>.Create(
            () => EntityOperationMapping<Position>.ForEntity(EntityType.Instance.Position())
                  .Operation<CreateIdentity>()
                  .Operation<UpdateIdentity>()
                  .Operation<ActivateIdentity>()
                  .Operation<DeactivateIdentity>()
                  .Operation<DeleteIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Position>(ids))));
    }
}