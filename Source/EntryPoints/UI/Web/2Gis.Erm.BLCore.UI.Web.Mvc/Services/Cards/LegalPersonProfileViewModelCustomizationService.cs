using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class LegalPersonProfileViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPersonProfile>, IRussiaAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (LegalPersonProfileViewModel)viewModel;

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
                                (int)OperatesOnTheBasisType.Undefined,
                                (int)OperatesOnTheBasisType.Certificate
                            };
                        break;
                    }

                case LegalPersonType.Businessman:
                    {
                        entityViewModel.DisabledDocuments = new[]
                            {
                                (int)OperatesOnTheBasisType.Undefined,
                                (int)OperatesOnTheBasisType.Charter,
                                (int)OperatesOnTheBasisType.Bargain,
                                (int)OperatesOnTheBasisType.FoundingBargain
                            };
                        break;
                    }

                case LegalPersonType.NaturalPerson:
                    {
                        entityViewModel.DisabledDocuments = new[]
                            {
                                (int)OperatesOnTheBasisType.Certificate,
                                (int)OperatesOnTheBasisType.Charter,
                                (int)OperatesOnTheBasisType.Bargain,
                                (int)OperatesOnTheBasisType.FoundingBargain
                            };
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}