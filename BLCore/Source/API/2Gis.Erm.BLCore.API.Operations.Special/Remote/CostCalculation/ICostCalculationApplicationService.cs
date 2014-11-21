using System;
using System.Collections.Generic;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Remote.CostCalculation
{
    // FIXME {d.ivanov, 05.11.2013}: Явно запретить инфраструктуре WCF оборачивать (wrap) входные и выходные параметры операций и перейти на IEnumerable<> для выходных параметров
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
    public interface ICostCalculationApplicationService
    {
        [OperationContract]
        [FaultContract(typeof(CostCalculatonErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        ICostCalculationResult[] CalculateOrderProlongation(long orderId);

        [OperationContract]
        [FaultContract(typeof(CostCalculatonErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        ICostCalculationResult[] CalculateOrderCostWithShortcutData(long destProjectCode,
                                                                    IEnumerable<ICalcPositionInfo> positionsInfo);

        [OperationContract]
        [FaultContract(typeof(CostCalculatonErrorDescription), Namespace = ServiceNamespaces.FinancialOperations.FinancialOperations201310)]
        ICostCalculationResult[] CalculateOrderCostWithFullData(DateTime beginDistributionDate,
                                                                long sourceProjectCode,
                                                                long firmId,
                                                                IEnumerable<ICalcPositionInfo> positionsInfo);
    }
}
