using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels
{
    public static class ViewModelUtils
    {
        public static bool TryGetBoundEntityName(this IViewModel viewModel, out IEntityType entityName)
        {
            // TODO {all, 22.07.2013}: возможно стоит выделить общий интерфейс для ViewModel\ViewModelIdentity  к которым привязан EntityName
            entityName = EntityType.Instance.None();

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
