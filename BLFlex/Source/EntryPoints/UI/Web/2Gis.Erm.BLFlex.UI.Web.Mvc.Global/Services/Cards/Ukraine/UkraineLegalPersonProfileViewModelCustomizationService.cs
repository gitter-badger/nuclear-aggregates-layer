using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Ukraine
{
    public class UkraineLegalPersonProfileViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPersonProfile>, IUkraineAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (UkraineLegalPersonProfileViewModel)viewModel;

            if (entityViewModel.IsMainProfile)
            {
                entityViewModel.Message = BLResources.LegalPersonProfileIsMain;
                entityViewModel.MessageType = MessageType.Info;
            }

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