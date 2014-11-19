using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Advertisements
{
    public sealed class AdvertisementAccessCustomization : IViewModelCustomization<AdvertisementViewModel>
    {
        public void Customize(AdvertisementViewModel viewModel, ModelStateDictionary modelState)
        {
            // FIXME {all, 18.11.2014}: �������: ���� UserDoesntHaveRightsToEditFirm �� ��������� �� � dto �� �� ViewModel.
            //                          ����� ��������� ����� ��� � �� ����������� ���������� ReadOnly
            viewModel.ViewConfig.ReadOnly |= viewModel.UserDoesntHaveRightsToEditFirm;
        }
    }
}