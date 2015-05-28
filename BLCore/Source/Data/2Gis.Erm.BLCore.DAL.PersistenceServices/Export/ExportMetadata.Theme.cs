using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Theme> Theme = QueryRuleContainer<Theme>.Create(
            () => EntityOperationMapping<Theme>.ForEntity(EntityType.Instance.Theme())
                                               .Operation<CreateIdentity>()
                                               .Operation<UpdateIdentity>()
                                               .Operation<DeleteIdentity>()
                                               .NonCoupledOperation<SetAsDefaultThemeIdentity>()
                                               .Use((query, ids) => query.For(Specs.Find.ByIds<Theme>(ids))),

            () => EntityOperationMapping<Theme>.ForEntity(EntityType.Instance.File())
                                               .Operation<UploadIdentity>()
                                               .Use((query, ids) => query.For(Specs.Find.ByFileIds<Theme>(ids))),

            () => EntityOperationMapping<Theme>.ForEntity(EntityType.Instance.ThemeCategory())
                                               .Operation<CreateIdentity>()
                                               .Operation<DeleteIdentity>()
                                               .Operation<UpdateIdentity>()
                                               .Use((query, ids) => query.For(Specs.Find.ByIds<ThemeCategory>(ids))
                                                                           .Select(link => link.Theme)));
    }
}

