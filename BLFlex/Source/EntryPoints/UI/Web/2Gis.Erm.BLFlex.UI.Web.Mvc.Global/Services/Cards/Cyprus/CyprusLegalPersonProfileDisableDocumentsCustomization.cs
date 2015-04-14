using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Cyprus
{
    public sealed class CyprusLegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization<CyprusLegalPersonProfileViewModel>, ICyprusAdapted
    {
        public void Customize(CyprusLegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
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