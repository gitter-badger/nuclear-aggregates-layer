using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels
{
    // Введено для описания метаданных. 
    // Это временное и вынужденное явление. 
    // Реальность такова, что сейчас вью-модели для разных клиентов существуют параллельно и их состав вызывает вопросы.
    public interface IEntityViewModelAbstract<TEntity> : IViewModelAbstract
        where TEntity : IEntityKey
    {
        long Id { get; }
        bool IsNew { get; }
    }
}
