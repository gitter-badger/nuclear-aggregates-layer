using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<PositionChildren> PositionChildren = QueryRuleContainer<PositionChildren>.Create(
           () => EntityOperationMapping<PositionChildren>.ForEntity(EntityName.PositionChildren)
                 .Operation<CreateIdentity>()
                 .Operation<UpdateIdentity>()
                 .Operation<DeleteIdentity>()
                 .Use((finder, ids) => finder.Find(Specs.Find.ByIds<PositionChildren>(ids))),
                 
            () => EntityOperationMapping<PositionChildren>.ForEntity(EntityName.Position)
                 .Operation<DeleteIdentity>()
                 .Operation<ActivateIdentity>()
                 .Operation<DeactivateIdentity>()
                 .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Position>(ids)).SelectMany(x => x.ChildPositions)));
    }
}