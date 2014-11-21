using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.GeoClassifier;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Project;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    // FIXME {a.tukaev, 05.05.2014}: Более явным будет указание контрактов IImportServiceBusDtoService<> и IOperation<> в имплементации самого сервиса, что позволит не иметь таких производных фасадных контрактов
    // COMMENT {d.ivanov, 06.05.2014}: Текущий механизм масспроцессинга operation service'ов предполагает как раз таки, что сам контракт сервиса закрывает/реализует IOperation<>.
    //                                 Закрывать IOperation<> в IImportServiceBusDtoService<> (т.е. по сути сделать IImportServiceBusDtoService<> контрактом операции) тоже нельзя, 
    //                                 т.к. операция получится в нашем понимании базовой, инфраструктура которых на текущий момент четко завязана на то, что есть некая сущность, над которой операция выполняется.
    //                                 В случае же с импортом было принято решение не завязываться на тип сущности
    // DONE {a.tukaev, 08.05.2014}: Ок, принято. Согласен, что типизация операции IImportServiceBusDtoService определяется типом DTO, что в свою очередь определяет Identity операции. 
    // Сознательно идем на то, что данные, импортируемые извне необязательно должны иметь отражение в виде сущности в нашей системе. Есть просто некая DTO импорта, которая может разложиться в одну или несколько наших сущностей
    public interface IImportBranchService : IImportServiceBusDtoService<BranchServiceBusDto>, IOperation<ImportBranchIdentity>
    {
    }
}