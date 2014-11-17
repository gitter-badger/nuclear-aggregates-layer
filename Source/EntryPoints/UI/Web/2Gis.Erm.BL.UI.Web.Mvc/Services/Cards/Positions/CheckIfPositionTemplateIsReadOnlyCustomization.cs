using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Positions
{
    public sealed class CheckIfPositionTemplateIsReadOnlyCustomization : IViewModelCustomization
    {
        private readonly IPositionRepository _positionRepository;

        public CheckIfPositionTemplateIsReadOnlyCustomization(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (PositionViewModel)viewModel;

            if (entityViewModel.Id != 0)
            {
                entityViewModel.IsReadonlyTemplate = _positionRepository.IsReadOnlyAdvertisementTemplate(entityViewModel.Id);
            }
        }
    }
}