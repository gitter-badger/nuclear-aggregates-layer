using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class PositionViewModelCustomizationService : IGenericViewModelCustomizationService<Position>
    {
        private readonly IPositionRepository _positionRepository;

        public PositionViewModelCustomizationService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (PositionViewModel)viewModel;

            if (entityViewModel.Id != 0)
            {
                entityViewModel.IsReadonlyTemplate = _positionRepository.IsReadOnlyAdvertisementTemplate(entityViewModel.Id);
            }
        }
    }
}