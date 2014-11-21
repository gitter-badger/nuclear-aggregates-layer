﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Theme> Theme = QueryRuleContainer<Theme>.Create(
            () => EntityOperationMapping<Theme>.ForEntity(EntityName.Theme)
                                               .Operation<CreateIdentity>()
                                               .Operation<UpdateIdentity>()
                                               .Operation<DeleteIdentity>()
                                               .NonCoupledOperation<SetAsDefaultThemeIdentity>()
                                               .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Theme>(ids))),

            () => EntityOperationMapping<Theme>.ForEntity(EntityName.File)
                                               .Operation<UploadIdentity>()
                                               .Use((finder, ids) => finder.Find(Specs.Find.ByFileIds<Theme>(ids))),

            () => EntityOperationMapping<Theme>.ForEntity(EntityName.ThemeCategory)
                                               .Operation<CreateIdentity>()
                                               .Operation<DeleteIdentity>()
                                               .Operation<UpdateIdentity>()
                                               .Use((finder, ids) => finder.Find(Specs.Find.ByIds<ThemeCategory>(ids))
                                                                           .Select(link => link.Theme)));
    }
}

