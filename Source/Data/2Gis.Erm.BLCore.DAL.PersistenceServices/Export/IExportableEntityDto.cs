namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    // TODO {a.rechkalov, 22.05.2013}: В чем суть интерфейса? Почему не использовать базовый интерфейс системы IEntityKey?
    // DONE {d.ivanov, 23.05.2013}: это скорее совсем не IEntityKey, чем просто IEntityKey
    // COMMENT {a.rechkalov, 01.07.2013}: Все же не до конца понятен ответ на вопрос, оставляю в виде TODO
    // COMMENT {d.ivanov, 2013-08-01}: IEntityKey реализуют сущности системы хранения. Сущности, реализующие IExportableEntityDto тоже имеют идентификаторы,
    //                                 но не имеют прямого отношения к системе хранения
    public interface IExportableEntityDto
    {
        long Id { get; set; }
    }
}
