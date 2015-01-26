using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;

using DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.CreateOrUpdate.ViewModels
{
    public sealed class CreateOrUpdateTestViewModel : ViewModelBase, IDocument
    {
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        private readonly IDesktopClientProxyFactory _clientProxyFactory;
        private readonly IStandartConfigurationSettings _configuration;
        private readonly IApiSettings _apiSettings;

        public CreateOrUpdateTestViewModel(IDesktopClientProxyFactory clientProxyFactory, IStandartConfigurationSettings configuration, IApiSettings apiSettings)
        {
            _clientProxyFactory = clientProxyFactory;
            _configuration = configuration;
            _apiSettings = apiSettings;
            GetHiCommand = new DelegateCommand(GetHiFromService);
        }

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public string DocumentName
        {
            get { return "Create or update Test"; }
        }

        Guid IDocument.Id 
        {
            get
            {
                return _identity.Id;
            }
        }

        public ICommand GetHiCommand { get; set; }

        private void GetHiFromService()
        {
            var advertisementElement = EntityType.Instance.AdvertisementElement();

            var getDomainEntityDtoServiceProxy = _clientProxyFactory.GetClientProxy<IGetDomainEntityDtoApplicationService, WSHttpBinding>();

            var createOrUpdateServiceProxy = _clientProxyFactory.GetClientProxy<ICreateOrUpdateApplicationService, WSHttpBinding>();

            try
            {
                var dto = getDomainEntityDtoServiceProxy.Execute(x => x.GetDomainEntityDto(advertisementElement, 52217));
                var id = createOrUpdateServiceProxy.Execute(x => x.Execute(advertisementElement, dto));
                MessageBox.Show(string.Format("Entity Id = {0}", id));
            }
            catch (FaultException<GetDomainEntityDtoOperationErrorDescription> ex)
            {
                MessageBox.Show(ex.Detail.Message);
            }
            catch (FaultException<CreateOrUpdateOperationErrorDescription> ex)
            {
                MessageBox.Show(ex.Detail.Message);
            }
        }
    }
}
