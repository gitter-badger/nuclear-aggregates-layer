using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<Position> Position = QueryRuleContainer<Position>.Create(
            () => EntityOperationMapping<Position>.ForEntity(EntityName.Position)
                  .Operation<CreateIdentity>()
                  .Operation<UpdateIdentity>()
                  .Operation<ActivateIdentity>()
                  .Operation<DeactivateIdentity>()
                  .Operation<DeleteIdentity>()
                  .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Position>(ids))));
    }
}