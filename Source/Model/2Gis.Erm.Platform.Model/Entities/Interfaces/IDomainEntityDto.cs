namespace DoubleGis.Erm.Platform.Model.Entities.Interfaces
{
    /// <summary>
    /// Маркерный интерфейс для Dto, которые используются для формирования ViewModel
    /// </summary>
    public interface IDomainEntityDto : IEntityKey
    {
    }

    public interface IDomainEntityDto<TEntity> : IDomainEntityDto 
        where TEntity : IEntityKey
    {
    }
}