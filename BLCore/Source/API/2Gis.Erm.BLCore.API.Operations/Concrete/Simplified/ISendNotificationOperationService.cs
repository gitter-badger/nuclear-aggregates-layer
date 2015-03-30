using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.NotificationEmail;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified
{
    public interface ISendNotificationOperationService : IOperation<SendNotificationIdentity>
    {
        void Send(IEnumerable<long> ownerCodes, string subject, string message);
    }
}
