﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeActivityStatus
{
    public interface IChangeActvityStatusEntityService : IOperation<ChangeActivityStatusIdentity>
    {
        void ChangeStatus(long entityId, ActivityStatus status);
    }
}
