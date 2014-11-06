using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;

using MessageType = DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels.MessageType;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards
{
    public class WarnLinkToAdvAgencyExistsVmCustomization : IViewModelCustomization
    {
        private readonly IClientReadModel _clientReadModel;

        public WarnLinkToAdvAgencyExistsVmCustomization(IClientReadModel clientReadModel)
        {
            _clientReadModel = clientReadModel;
        }

        public void Customize(IEntityViewModelBase viewModel)
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
                viewModel.Message = BLResources.ClientHasParentLinkToAdvAgency + "\n" + clientNames;
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