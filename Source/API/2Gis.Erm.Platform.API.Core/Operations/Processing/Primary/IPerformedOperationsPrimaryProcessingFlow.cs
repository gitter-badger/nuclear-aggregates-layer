using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    /// <summary>
    /// Маркерный интерфейс потоков выполненных операций, для их первичной обработки
    /// </summary>
    
    // TODO {i.maslennikov, 25.08.2014}: Нужно поработать над маппингом потоков обработки и сложить все в одно место.
    //                                   Так, сейчас есть этот интерфейс, есть ISourceMessageFlow<>, есть IMessageFlowRegistry, по которым разбросано знание кто и как с кем связан.
    //                                   Скорее всего, подойдет что-то типа builder-а
    //                                   Кроме того, настройку actors для этапов обработки сообщений в этих потоках также нужно включить в этот builder
    public interface IPerformedOperationsPrimaryProcessingFlow : IMessageFlow
    {
    }
}