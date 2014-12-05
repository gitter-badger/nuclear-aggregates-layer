using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Kazakhstan
{
    public sealed class KazakhstanLegalPersonProfileDisableDocumentsCustomization : IViewModelCustomization<KazakhstanLegalPersonProfileViewModel>, IKazakhstanAdapted
    {
        public void Customize(KazakhstanLegalPersonProfileViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.DisabledDocuments = GetDisabledDocuments(viewModel.LegalPersonType);
        }

        private static string[] GetDisabledDocuments(LegalPersonType legalPersonType)
        {
            switch (legalPersonType)
            {
                case LegalPersonType.LegalPerson:
                case LegalPersonType.Businessman:
                    return new string[]
                               {
                               };
                case LegalPersonType.NaturalPerson:
                    return new string[]
                               {
                                   OperatesOnTheBasisType.Charter.ToString(),
                                   OperatesOnTheBasisType.Certificate.ToString(),
                                   OperatesOnTheBasisType.Decree.ToString(),
                               };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}