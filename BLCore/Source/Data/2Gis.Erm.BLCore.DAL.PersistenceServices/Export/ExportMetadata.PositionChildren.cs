using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<PositionChildren> PositionChildren = QueryRuleContainer<PositionChildren>.Create(
           () => EntityOperationMapping<PositionChildren>.ForEntity(EntityType.Instance.PositionChildren())
                 .Operation<CreateIdentity>()
                 .Operation<UpdateIdentity>()
                 .Operation<DeleteIdentity>()
                 .Use((query, ids) => query.For(Specs.Find.ByIds<PositionChildren>(ids))),

            () => EntityOperationMapping<PositionChildren>.ForEntity(EntityType.Instance.Position())
                 .Operation<DeleteIdentity>()
                 .Operation<ActivateIdentity>()
                 .Operation<DeactivateIdentity>()
                 .Use((query, ids) => query.For(Specs.Find.ByIds<Position>(ids)).SelectMany(x => x.ChildPositions)));
    }
}