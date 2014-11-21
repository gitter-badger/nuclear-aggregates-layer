using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.OrderPositions
{
    public sealed class OrderPositionRateCustomization : IViewModelCustomization<ICustomizableOrderPositionViewModel>
    {
        public void Customize(ICustomizableOrderPositionViewModel viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.IsNew)
            {
                return;
            }

            if (viewModel.IsRated && viewModel.CategoryRate != 1 && string.IsNullOrEmpty(viewModel.Message))
            {
                // ���������� � double ������������, ����� ��������� ���������� � �������, ���������� � decimal � �� �������� ���������� ���� ������
                viewModel.SetInfo(string.Format(BLResources.CategoryGroupInfoMessage, (double)viewModel.CategoryRate));
            }
        }
    }
}