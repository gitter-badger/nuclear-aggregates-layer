using System.Diagnostics.CodeAnalysis;

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
        public static readonly QueryRuleContainer<ThemeTemplate> ThemeTemplate = QueryRuleContainer<ThemeTemplate>.Create(
            () => EntityOperationMapping<ThemeTemplate>.ForEntity(EntityType.Instance.ThemeTemplate())
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByIds<ThemeTemplate>(ids))),

            () => EntityOperationMapping<ThemeTemplate>.ForEntity(EntityType.Instance.File())
                                                       .Operation<UploadIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByFileIds<ThemeTemplate>(ids))));
    }
}

