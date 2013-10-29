using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.UI.Web.Mvc.Models;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;

using MessageType = DoubleGis.Erm.Platform.Web.Mvc.ViewModel.MessageType;

namespace DoubleGis.Erm.UI.Web.Mvc.Services.Cards
{
    public class CyprusLegalPersonProfileViewModelCustomizationService : IGenericViewModelCustomizationService<LegalPersonProfile>, ICyprusAdapted
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (CyprusLegalPersonProfileViewModel)viewModel;

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
                                (int)OperatesOnTheBasisType.Underfined,
                                (int)OperatesOnTheBasisType.Certificate
                            };
                        break;
                    }

                case LegalPersonType.Businessman:
                    {
                        entityViewModel.DisabledDocuments = new[]
                            {
                                (int)OperatesOnTheBasisType.Underfined,
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