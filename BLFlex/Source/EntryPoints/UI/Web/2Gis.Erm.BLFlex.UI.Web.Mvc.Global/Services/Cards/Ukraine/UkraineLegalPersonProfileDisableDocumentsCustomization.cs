using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Ukraine
{
    public sealed class UkraineLegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization<UkraineLegalPersonProfileViewModel>, IUkraineAdapted
    {
        public void Customize(UkraineLegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
        {
            switch (viewModel.LegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    {
                        viewModel.DisabledDocuments = new[]
                            {
                                OperatesOnTheBasisType.Undefined.ToString(),
                                OperatesOnTheBasisType.Certificate.ToString()
                            };
                        break;
                    }

                case LegalPersonType.Businessman:
                    {
                        viewModel.DisabledDocuments = new[]
                            {
                                OperatesOnTheBasisType.Undefined.ToString(),
                                OperatesOnTheBasisType.Charter.ToString(),
                                OperatesOnTheBasisType.FoundingBargain.ToString()
                            };
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}