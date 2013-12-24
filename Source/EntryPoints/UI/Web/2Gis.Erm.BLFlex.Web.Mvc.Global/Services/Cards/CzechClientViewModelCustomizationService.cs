using System;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.UI.Web.Mvc.Models;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
        // 2+: BLFlex\Source\EntryPoints\UI\Web\2Gis.Erm.BLFlex.Web.Mvc.Global\Services\Cards\CzechClientViewModelCustomizationService.cs
        public class CzechClientViewModelCustomizationService : IGenericViewModelCustomizationService<Client>, ICzechAdapted
        {
            public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
            {
                var entityViewModel = (CzechClientViewModel)viewModel;

                if (entityViewModel.IsNew)
                {
                    // ��� �������� ������ ������� ������ ������������� ���� ������ �� �������, �� �� ���� ����������� � ������.
                    entityViewModel.LastQualifyTime = DateTime.UtcNow.Date;
                }
            }
        }
}