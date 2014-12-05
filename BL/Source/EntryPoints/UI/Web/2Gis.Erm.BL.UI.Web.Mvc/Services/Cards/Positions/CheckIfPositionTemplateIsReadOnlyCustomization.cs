using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Positions
{
    public sealed class CheckIfPositionTemplateIsReadOnlyCustomization : IViewModelCustomization<PositionViewModel>
    {
        private readonly IPositionRepository _positionRepository;

        public CheckIfPositionTemplateIsReadOnlyCustomization(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public void Customize(PositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.Id != 0)
            {
                viewModel.IsReadonlyTemplate = _positionRepository.IsReadOnlyAdvertisementTemplate(viewModel.Id);
            }
        }
    }
}