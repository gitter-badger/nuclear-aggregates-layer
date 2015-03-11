using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.Operations.Remote.ActionsHistory;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Activate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Append;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeClient;
using DoubleGis.Erm.BLCore.API.Operations.Remote.ChangeTerritory;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CheckForDebts;
using DoubleGis.Erm.BLCore.API.Operations.Remote.CreateOrUpdate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Deactivate;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Delete;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Qualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Settings;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.Remote.List;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.Config
{
    public static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            var metadataSvcSettings = container.Resolve<IAPIIntrospectionServiceSettings>();
            var releasingSvcSettings = container.Resolve<IAPIReleasingServiceSettings>();
            var operationsSvcSettings = container.Resolve<IAPIOperationsServiceSettings>();
            var orderValidationSvcSettings = container.Resolve<IAPIOrderValidationServiceSettings>();
            var sharedTypesBehaviorFactory = container.Resolve<ISharedTypesBehaviorFactory>();

            var wsHttpbinding = BindingConfig.WsHttp
                                       .UseTransportSecurity(HttpClientCredentialType.Windows)
                                       .MaxReceivedMessageSize(50000000)
                                       .Timeouts(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));

            var wsDualHttpbinding = BindingConfig.WsDualHttp
                                        .UseMessageSecurity(MessageCredentialType.Windows)
                                        .MaxReceivedMessageSize(20000000)
                                        .Timeouts(TimeSpan.FromHours(1), TimeSpan.FromMinutes(1))
                                        .ClientBaseAddress("http://localhost:8000/erm_{E9AF926E-29E3-4B8A-BA45-512923857199}/");

            var sharedTypeResolverEndpointBehavior = sharedTypesBehaviorFactory.Create();

            provider.AddEndpoint<IActionsHistoryApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "ActionsHistory.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IActivateApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Activate.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IAppendApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Append.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IAssignApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Assign.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IGroupAssignApplicationService>(wsDualHttpbinding, operationsSvcSettings.BaseUrl, "GroupAssign.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IChangeClientApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "ChangeClient.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IChangeTerritoryApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "ChangeTerritory.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<ICheckForDebtsApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "CheckForDebts.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IDeactivateApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Deactivate.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IDeleteApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Delete.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IDisqualifyApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Disqualify.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IQualifyApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "Qualify.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IGetDomainEntityDtoApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "GetDomainEntityDto.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<ICreateOrUpdateApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "CreateOrUpdate.svc/Soap", sharedTypeResolverEndpointBehavior)
                    .AddEndpoint<IListApplicationService>(wsHttpbinding, operationsSvcSettings.BaseUrl, "List.svc/Soap", sharedTypeResolverEndpointBehavior)

                    .AddEndpoint<IMetadataProviderApplicationService>(wsHttpbinding, metadataSvcSettings.BaseUrl, "Metadata.svc/Soap", sharedTypeResolverEndpointBehavior)

                    .AddEndpoint<IOrderValidationApplicationService>(wsHttpbinding, orderValidationSvcSettings.BaseUrl, "Validate.svc/Soap", sharedTypeResolverEndpointBehavior)

                    .AddEndpoint<IReleaseApplicationService>(wsHttpbinding, releasingSvcSettings.BaseUrl, "Release.svc/Soap", sharedTypeResolverEndpointBehavior);

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}