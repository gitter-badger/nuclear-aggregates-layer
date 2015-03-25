using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Dial
{
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
    public interface IDialApplicationService
    {
    }
}
