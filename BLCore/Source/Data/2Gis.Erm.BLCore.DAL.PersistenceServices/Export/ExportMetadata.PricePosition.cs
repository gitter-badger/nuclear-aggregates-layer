﻿using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public static partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<PricePosition> PricePosition = QueryRuleContainer<PricePosition>.Create(
            () => EntityOperationMapping<PricePosition>.ForEntity(EntityName.PricePosition)
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Operation<DeactivateIdentity>()
                                                       .Operation<ActivateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .NonCoupledOperation<CopyPricePositionIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByIds<PricePosition>(ids))));
    }
}