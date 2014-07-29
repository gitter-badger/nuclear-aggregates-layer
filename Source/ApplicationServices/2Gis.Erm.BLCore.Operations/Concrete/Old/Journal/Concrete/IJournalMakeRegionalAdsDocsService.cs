using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Concrete
{
    // FIXME {all, 23.10.2013}: нужен рефакторинг - реализовать нормальный специфический контракт, т.к. нет никакого смысла, что то сливать в обобщенный контейнер данных, а потом оттуда выколупывать эти данные, ещё и используя строки как Keys
    public interface IJournalMakeRegionalAdsDocsService : IJournalBusinessOperationsService, ISimplifiedModelConsumer
    {
    }
}