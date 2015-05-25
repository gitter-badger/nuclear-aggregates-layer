using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<ThemeOrganizationUnit> ThemeOrganizationUnit = QueryRuleContainer<ThemeOrganizationUnit>.Create(
            () => EntityOperationMapping<ThemeOrganizationUnit>.ForEntity(EntityType.Instance.ThemeOrganizationUnit())
                                                               .Operation<CreateIdentity>()
                                                               .Operation<UpdateIdentity>()
                                                               .Operation<DeleteIdentity>()
                                                               .Use((finder, ids) => finder.Find(Specs.Find.ByIds<ThemeOrganizationUnit>(ids))),

            () => EntityOperationMapping<ThemeOrganizationUnit>.ForEntity(EntityType.Instance.Theme())
                                                               .Operation<CreateIdentity>()
                                                               .Operation<UpdateIdentity>()
                                                               .Operation<DeleteIdentity>()
                                                               .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Theme>(ids))
                                                                                           .SelectMany(theme => theme.ThemeOrganizationUnits)));
    }
}

