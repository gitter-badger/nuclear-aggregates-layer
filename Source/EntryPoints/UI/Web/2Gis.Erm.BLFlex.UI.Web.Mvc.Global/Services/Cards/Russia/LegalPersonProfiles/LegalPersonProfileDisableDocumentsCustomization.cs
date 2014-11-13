using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.LegalPersonProfiles
{
    public class LegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization, IRussiaAdapted
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LegalPersonProfileViewModel)viewModel;

            switch (entityViewModel.LegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    {
                        entityViewModel.DisabledDocuments = new[]
                            {
                                OperatesOnTheBasisType.Undefined.ToString(),
                                OperatesOnTheBasisType.Certificate.ToString()
                            };
                        break;
                    }

                case LegalPersonType.Businessman:
                    {
                        entityViewModel.DisabledDocuments = new[]
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
                        entityViewModel.DisabledDocuments = new[]
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