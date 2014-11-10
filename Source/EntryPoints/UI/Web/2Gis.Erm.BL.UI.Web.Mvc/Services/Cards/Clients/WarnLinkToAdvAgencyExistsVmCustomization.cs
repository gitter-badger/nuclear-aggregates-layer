using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Clients
{
    public class WarnLinkToAdvAgencyExistsVmCustomization : IViewModelCustomization
    {
        private readonly IClientReadModel _clientReadModel;

        public WarnLinkToAdvAgencyExistsVmCustomization(IClientReadModel clientReadModel)
        {
            _clientReadModel = clientReadModel;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            if (viewModel.MessageType == MessageType.CriticalError)
            {
                return;
            }

            var masters = FindParentAdvAgencyClientNames(viewModel);

            var clientNames = masters.Aggregate(string.Empty, (current, master) => current + (CreateLinkToCard("Client", master.Name, master.Id) + "; "));
             
            if (!string.IsNullOrEmpty(clientNames))
            {
                viewModel.MessageType = MessageType.Warning;
                viewModel.Message = Resources.Server.Properties.Resources.ClientHasParentLinkToAdvAgency + "\n" + clientNames;
            }
        }

        private static string CreateLinkToCard(string entityName, string linkText, long id)
        {
            return string.Format("<{0}:{1}:{2}>", entityName, linkText, id);
        }

        private IEnumerable<MasterClientDto> FindParentAdvAgencyClientNames(IEntityViewModelBase clientViewModel)
        {
            return _clientReadModel.GetMasterAdvertisingAgencies(clientViewModel.Id);
        }
    }
}