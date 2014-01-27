using System;
using System.IO;

namespace DoubleGis.Erm.BLCore.Reports
{
    public interface IReportsSqlConnectionWrapper
    {
        // FIXME {v.lapeev, 24.01.2014}: Похоже этот метод вызвается из хендлера, который нигде не вызывается, учтонить и удалить если так
        Stream GetLegalPersonPaymentsStream(long organizationUnitId, DateTime reportDate, long currentUser, bool considerOnRegistration, bool considerOnApproval);
        
        Stream GetPlanningReportStream(long organizationUnitId, DateTime planningMonth, bool isAdvertisingAgency); 
    }
}