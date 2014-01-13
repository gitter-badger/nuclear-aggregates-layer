using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels
{
    public static class ViewModelUtils
    {
        public static bool TryGetBoundEntityName(this IViewModel viewModel, out EntityName entityName)
        {
            // TODO {all, 22.07.2013}: возможно стоит выделить общий интерфейс для ViewModel\ViewModelIdentity  к которым привязан EntityName
            entityName = EntityName.None;

            var cardViewModel = viewModel as ICardViewModel<ICardViewModelIdentity>;
            if (cardViewModel != null)
            {
                entityName = cardViewModel.ConcreteIdentity.EntityName;
                return true;
            }

            var gridViewModel = viewModel as IGridViewModel<IGridViewModelIdentity>;
            if (gridViewModel != null)
            {
                entityName = gridViewModel.ConcreteIdentity.EntityName;
                return true;
            }

            return false;
        }
    }
}
