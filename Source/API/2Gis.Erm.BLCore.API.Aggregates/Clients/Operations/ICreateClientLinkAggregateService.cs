﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations
{
    public interface ICreateClientLinkAggregateService : IAggregateSpecificOperation<Client, CreateIdentity>
    {
        void Create(ClientLink client, IEnumerable<DenormalizedClientLink> denormalization);
    }
}