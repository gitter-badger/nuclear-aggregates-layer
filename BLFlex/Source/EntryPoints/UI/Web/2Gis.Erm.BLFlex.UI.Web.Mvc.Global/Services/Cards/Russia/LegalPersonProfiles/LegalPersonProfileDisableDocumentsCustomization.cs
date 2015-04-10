using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.LegalPersonProfiles
{
    public sealed class LegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization<LegalPersonProfileViewModel>, IRussiaAdapted
    {
        public void Customize(LegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
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
                                OperatesOnTheBasisType.Bargain.ToString(),
                                OperatesOnTheBasisType.FoundingBargain.ToString()
                            };
                        break;
                    }

                case LegalPersonType.NaturalPerson:
                    {
                        viewModel.DisabledDocuments = new[]
                            {
                                OperatesOnTheBasisType.Certificate.ToString(),
                                OperatesOnTheBasisType.Charter.ToString(),
                                OperatesOnTheBasisType.Bargain.ToString(),
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