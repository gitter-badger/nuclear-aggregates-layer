using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Kazakhstan
{
    public class KazakhstanLegalPersonProfileViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPersonProfile>, IKazakhstanAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (KazakhstanLegalPersonProfileViewModel)viewModel;

            if (entityViewModel.IsMainProfile)
            {
                entityViewModel.Message = BLResources.LegalPersonProfileIsMain;
                entityViewModel.MessageType = MessageType.Info;
            }

            entityViewModel.DisabledDocuments = GetDisabledDocuments(entityViewModel.LegalPersonType);
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