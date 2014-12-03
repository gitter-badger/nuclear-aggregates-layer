using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public interface ICardSettingsProcessor
    {
        void ProcessCardSettings<TEntity, TViewModel>(TViewModel model)
            where TEntity : IEntity
            where TViewModel : IViewModelAbstract;
    }
}