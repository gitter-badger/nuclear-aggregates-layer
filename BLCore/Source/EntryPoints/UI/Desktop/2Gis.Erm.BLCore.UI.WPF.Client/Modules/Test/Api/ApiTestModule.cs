using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;

using DoubleGis.Erm.BLCore.API.Operations.Generic.ActionHistory;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeTerritory;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Get;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Api.Settings;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Extensions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Test.Api
{
    public class ApiTestModule : IStandaloneWorkerModule
    {
        private readonly IUnityContainer _container;
        private readonly IApiTestModuleSettings _moduleSettings;

        public ApiTestModule(IUnityContainer container, IApiTestModuleSettings moduleSettings)
        {
            _container = container;
            _moduleSettings = moduleSettings;
        }

        #region Implementation of IModule

        public Guid Id
        {
            get
            {
                return new Guid("51EEAD3F-C913-4353-BDED-5B1B3923AA06");
            }
        }

        public string Description
        {
            get
            {
                return "Erm WPF Client Test module";
            }
        }

        public void Configure()
        {
            // время для регистраций
            // var settings = new TestModuleSettings(ConfigFileFullPath);
            //_container.RegisterInstance<IWpfErmClientSettings>(settings);
            //_container.RegisterInstance<ITestModuleSettings>(settings);
        }

        #endregion

        #region Implementation of IStandaloneWorker

        public void Run()
        {
            var extension = _container.Resolve<QueryableContainerExtension>(Mapping.QueryableExtension);

            var testSequence = new List<Action>
                {
                    //Metadata,
                    //Logger,
                    //List,
                    //Deactivate,
                    //Activate,
                    //Assign,
                    //Qualify,
                    //Disqualify,
                    //Delete,
                    //ActionsHistory,
                    //Edit,
                    //ChangeClient,
                    //ChangeTerritory,
                    //CheckForDebts,
                    //Append
                };

            foreach (var action in testSequence)
            {
                action();
            }
        }

        private void Metadata()
        {
            var service = _container.Resolve<IOperationsMetadataProvider>();
            var allOperations = service.GetApplicableOperations();
            //var allOperationsForUser = service.GetApplicableOperationsForCallingUser();
            //var allOperationsForContext = service.GetApplicableOperationsForContext(new[] { EntityName.Order }, new[] { 29977L });
        }

        private void Logger()
        {
            var logger = _container.Resolve<ICommonLog>();
            logger.Error("test message 1");
            logger.Error(new InvalidOperationException("test exception 1"), "test message 2");
        }

        private void List()
        {
            ListAdvertismentElements();
            //ListFirms();
            //ListOrganizationUnits();
        }

        private void ListAdvertismentElements()
        {
            var service = //_container.Resolve<IListGenericEntityDtoService<AdvertisementElement, ListAdvertisementElementDto>>();
                _container.Resolve<IListNonGenericEntityService>();
            var searchModel = new SearchListModel { Start = 0, Limit = 40, Sort = "Id", Dir = "ASC" };
            //new SearchListModel { Start = 0, Limit = 0, Sort = "Id", Dir = "ASC", WhereExp = "AdvertisementId=1" };
            var result = service.List(EntityName.OrderPositionAdvertisement, searchModel);
        }

        private void ListOrganizationUnits()
        {
            var service = _container.Resolve<IListGenericEntityDtoService<OrganizationUnit, ListOrganizationUnitDto>>();
            var searchModel = new SearchListModel { Start = 0, Limit = 20, Sort = "Id", Dir = "DESC" };
            var result = service.List(searchModel);
            var searchModel2 = new SearchListModel { Start = 20, Limit = 20, Sort = "Id", Dir = "DESC" };
            var result2 = service.List(searchModel2);
        }

        private void ListFirms()
        {
            var service = _container.Resolve<IListGenericEntityDtoService<Firm, FirmGridDoc>>();
            var searchModel = new SearchListModel { Start = 0, Limit = 20, Sort = "Id", Dir = "DESC" };
            var result = service.List(searchModel);
            var searchModel2 = new SearchListModel { Start = 20, Limit = 20, Sort = "Id", Dir = "DESC" };
            var result2 = service.List(searchModel2);
        }

        private void Deactivate()
        {
            var service = _container.Resolve<IDeactivateGenericEntityService<Firm>>();
            var result = service.Deactivate(4644873986741278, 1);
        }

        private void Activate()
        {
            var service = _container.Resolve<IActivateGenericEntityService<Firm>>();
            var result = service.Activate(1157667);
        }

        private void Assign()
        {
            var service = _container.Resolve<IAssignGenericEntityService<Firm>>();
            var result = service.Assign(1157667, 1, false, false);
        }
        
        private void Qualify()
        {
            var service = _container.Resolve<IQualifyGenericEntityService<Firm>>();
            var result = service.Qualify(1157667, 1, null);
        }
        
        private void Disqualify()
        {
            var service = _container.Resolve<IDisqualifyGenericEntityService<Firm>>();
            var result = service.Disqualify(1157667, false);
        }
        
        private void Delete()
        {
            var service = _container.Resolve<IDeleteGenericEntityService<Firm>>();
            //var result1 = service.GetConfirmation(1157667);
            var result2 = service.Delete(1);
        }

        private void ActionsHistory()
        {
            var service = _container.Resolve<IActionsHistoryService>();
            var result = service.GetActionHistory(EntityName.Order, 55523);
        }

        private void Edit()
        {
            //var getService = _container.Resolve<IGetDomainEntityDtoService<CategoryOrganizationUnit>>();
            //var getResult = getService.GetDomainEntityDto(87839, true, null, EntityName.None, string.Empty);
            var getService = _container.Resolve<IGetDomainEntityDtoService<AdvertisementElement>>();
            var saveService = _container.Resolve<IModifyBusinessModelEntityService<AdvertisementElement>>();

            try
            {
                var getResult = getService.GetDomainEntityDto(1, false, null, EntityName.None, null);
                var saveResult = saveService.Modify(getResult);
            }
            catch (FaultException<GetDomainEntityDtoOperationErrorDescription> ex)
            {
                MessageBox.Show(ex.Detail.Message);
            }
            catch (FaultException<CreateOrUpdateOperationErrorDescription> ex)
            {
                MessageBox.Show(ex.Detail.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private void ChangeClient()
        {
            var service = _container.Resolve<IChangeGenericEntityClientService<Firm>>();
            var validationResult = service.Validate(18, 4841);
            var changeResult = service.Execute(18, 4841, false);
        }
        
        private void ChangeTerritory()
        {
            var service = _container.Resolve<IChangeGenericEntityTerritoryService<Firm>>();
            service.ChangeTerritory(1, 1);
        }
        
        private void CheckForDebts()
        {
            var service = _container.Resolve<ICheckGenericEntityForDebtsService<DoubleGis.Erm.Platform.Model.Entities.Erm.Client>>();
            var result = service.CheckForDebts(5);
        }

        private void Append()
        {
            var service = _container.Resolve<IAppendGenericEntityService<OrganizationUnit, User>>();
            service.Append(new AppendParams { AppendedId = 106, AppendedType = EntityName.OrganizationUnit, ParentId = 1, ParentType = EntityName.User });
        }

        public void TryStop()
        {
        }

        public void Wait()
        {
        }

        #endregion
    }
}
