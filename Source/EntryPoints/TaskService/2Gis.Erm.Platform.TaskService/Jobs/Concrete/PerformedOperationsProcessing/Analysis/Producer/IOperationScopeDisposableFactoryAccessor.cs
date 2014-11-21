namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    /// <summary>
    /// ‘актически фабрика фабрик, необхоимость возникла, т.к. в некоторых сценари€х (например, генератор performed operations дл€ LOAD тестировани€),
    /// Ќеобходимо €вно контролировать не только lifetime самих operationscopes, но и operationscopefactories, характерна€ черта таких сценариев - 
    /// необходимость perscope поведение, с точки зрени€ lifetime, причем scope могут быть вложенными
    /// </summary>
    public interface IOperationScopeDisposableFactoryAccessor
    {
        IOperationScopeDisposableFactory Factory { get; }
    }
}