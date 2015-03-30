using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations
{
    public class GroupOperationController : ControllerBase
    {
        private readonly IReplicationCodeConverter _replicationCodeConverter;
        private readonly IOperationsMetadataProvider _operationMetadataProvider;

        public GroupOperationController(IMsCrmSettings msCrmSettings,
                                        IAPIOperationsServiceSettings operationsServiceSettings,
                                        IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                        IAPIIdentityServiceSettings identityServiceSettings,
                                        IUserContext userContext,
                                        ITracer tracer,
                                        IGetBaseCurrencyService getBaseCurrencyService,
                                        IReplicationCodeConverter replicationCodeConverter,
                                        IOperationsMetadataProvider operationMetadataProvider)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _replicationCodeConverter = replicationCodeConverter;
            _operationMetadataProvider = operationMetadataProvider;
        }

        public ActionResult Execute(BusinessOperation operation, EntityName entityTypeName)
        {
            var operationName = operation.ToString();
            // TODO {all, 23.07.2013}: реализовать проверку доступности операций не через switch, а через operationMetadataProvider
            // т.е. обобщенным образом + генерацию View можно устроить через конвейер resolvers, без километрового switch

            switch (operation)
            {
                case BusinessOperation.Delete:
                    return View("Delete",
                                new GroupOperationViewModel
                                    {
                                        OperationName = operationName,
                                        EntityTypeName = entityTypeName,
                                    });

                case BusinessOperation.Assign:
                    if (!_operationMetadataProvider.IsSupported<AssignIdentity>(entityTypeName))
                    {
                        throw new NotificationException(BLResources.AssignOperationIsNotSpecifiedForThisEntity);
                    }

                    var assignMetadata = _operationMetadataProvider.GetOperationMetadata<AssignMetadata, AssignIdentity>(entityTypeName);
                    return View("Assign",
                                new AssignViewModel
                                    {
                                        OperationName = operationName,
                                        EntityTypeName = entityTypeName,
                                        PartialAssignSupported = assignMetadata.PartialAssignSupported,
                                        IsCascadeAssignForbidden = assignMetadata.IsCascadeAssignForbidden
                                    });

                case BusinessOperation.Activate:
                    return View("Activate",
                                new GroupOperationViewModel
                                    {
                                        OperationName = operationName,
                                        EntityTypeName = entityTypeName,
                                    });

                case BusinessOperation.Deactivate:
                {
                    if (entityTypeName == EntityName.User)
                    {
                        return View("DeactivateUser",
                                    new OwnerGroupOperationViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    if (entityTypeName == EntityName.Territory)
                    {
                        return View("DeactivateTerritory",
                                    new DeactivateTerritoryViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    return View("Deactivate",
                                new GroupOperationViewModel
                                    {
                                        OperationName = operationName,
                                        EntityTypeName = entityTypeName,
                                    });
                }

                case BusinessOperation.Qualify:
                {
                    if (entityTypeName == EntityName.Firm)
                    {
                        return View("QualifyFirm",
                                    new QualifyFirmViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    if (entityTypeName == EntityName.Client)
                    {
                        return View("QualifyClient",
                                    new OwnerGroupOperationViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    throw new NotificationException(BLResources.QualifyOperationIsNotSpecifiedForThisEntity);
                }

                case BusinessOperation.Disqualify:
                {
                    if (entityTypeName == EntityName.Firm)
                    {
                        return View("DisqualifyFirm",
                                    new GroupOperationViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    if (entityTypeName == EntityName.Client)
                    {
                        return View("DisqualifyClient",
                                    new GroupOperationViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    throw new NotificationException(BLResources.DisqualifyOperationIsNotSpecifiedForThisEntity);
                }

                case BusinessOperation.ChangeTerritory:
                {
                    if (entityTypeName == EntityName.Firm)
                    {
                        return View("ChangeFirmTerritory",
                                    new ChangeTerritoryViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    if (entityTypeName == EntityName.Client)
                    {
                        return View("ChangeClientTerritory",
                                    new ChangeTerritoryViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    throw new NotificationException(BLResources.DisqualifyOperationIsNotSpecifiedForThisEntity);
                }

                case BusinessOperation.ChangeClient:
                {
                    if (entityTypeName == EntityName.Firm)
                    {
                        return View("ChangeFirmClient",
                                    new ChangeClientViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    if (entityTypeName == EntityName.LegalPerson)
                    {
                        return View("ChangeLegalPersonClient",
                                    new ChangeClientViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    if (entityTypeName == EntityName.Deal)
                    {
                        return View("ChangeDealClient",
                                    new ChangeClientViewModel
                                        {
                                            OperationName = operationName,
                                            EntityTypeName = entityTypeName,
                                        });
                    }

                    throw new NotificationException(BLResources.ChangeClientOperationIsNotSpecifiedForThisEntity);
                }

                case BusinessOperation.Cancel:
                    if (!_operationMetadataProvider.IsSupported<CancelIdentity>(entityTypeName))
                    {
                        throw new NotificationException(BLResources.CancelOperationIsNotSpecifiedForThisEntity);
                    }

                    return View("Cancel",
                               new GroupOperationViewModel
                               {
                                   OperationName = operationName,
                                   EntityTypeName = entityTypeName,
                               });

                default:
                    throw new NotificationException(BLResources.OperationIsNotSpecified);
            }
        }

        public JsonNetResult ConvertToEntityIds(EntityName[] entityTypeNames, Guid[] replicationCodes)
        {
            try
            {
                var list = entityTypeNames.Zip(replicationCodes, (k, v) => new CrmEntityInfo { Id = v, EntityName = k }).ToList();
                return new JsonNetResult(_replicationCodeConverter.ConvertToEntityIds(list));
            }
            catch (ArgumentException ex)
            {
                throw new NotificationException(ex.Message, ex);
            }
        }
    }
}