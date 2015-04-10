using System;
using System.Linq;
using System.ServiceModel.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Metadata;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations
{
    public sealed class CreateOrUpdateController<TEntity, TModel> : ControllerBase
        where TEntity : class, IEntityKey, IEntity
        where TModel : EntityViewModelBase<TEntity>, new()
    {
        private readonly IEntityViewNameProvider _entityViewNameProvider;
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly ICardSettingsProvider _cardSettingsProvider;
        private readonly IViewModelCustomizationService _viewModelCustomizationService;

        public CreateOrUpdateController(IMsCrmSettings msCrmSettings,
                                        IUserContext userContext,
                                        ITracer tracer,
                                        IOperationServicesManager operationServicesManager,
                                        ISecurityServiceUserIdentifier userIdentifierService,
                                        ISecurityServiceEntityAccess entityAccessService,
                                        IAPIOperationsServiceSettings operationsServiceSettings,
                                        IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                        IGetBaseCurrencyService getBaseCurrencyService,
                                        IEntityViewNameProvider entityViewNameProvider,
                                        ICardSettingsProvider cardSettingsProvider,
                                        IViewModelCustomizationService viewModelCustomizationService,
                                        IAPIIdentityServiceSettings identityServiceSettings)
            : base(msCrmSettings,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   identityServiceSettings,
                   userContext,
                   tracer,
                   getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _userIdentifierService = userIdentifierService;
            _entityAccessService = entityAccessService;
            _entityViewNameProvider = entityViewNameProvider;
            _cardSettingsProvider = cardSettingsProvider;
            _viewModelCustomizationService = viewModelCustomizationService;
        }

        [HttpGet]
        [SetEntityStateToken]
        [UseDependencyFields]
        public ActionResult Entity(long? entityId, bool? readOnly, long? pId, IEntityType pType, string extendedInfo)
        {
            var actualEntityId = entityId ?? 0;
            var actualReadOnly = readOnly ?? false;

            var model = GetViewModel(actualEntityId, actualReadOnly, pId, pType, extendedInfo);
            SetViewModelProperties(model, actualReadOnly, pId, pType, extendedInfo);
            _viewModelCustomizationService.CustomizeViewModel<TModel, TEntity>(model, ModelState);
            var viewName = _entityViewNameProvider.GetView<TModel, TEntity>();
            return View(viewName, model);
        }

        [HttpPost]
        [GetEntityStateToken]
        [UseDependencyFields]
        public ActionResult Entity(TModel model)
        {
            try
            {
                if (ModelUtils.CheckIsModelValid(this, model))
                {
                    model = CreateOrUpdate(model);
                }
            }
            catch (ArgumentException ex)
            {
                ModelUtils.OnException(this, Tracer, model, new NotificationException(ex.Message));
            }
            catch (NotSupportedException ex)
            {
                ModelUtils.OnException(this, Tracer, model, new NotificationException(ex.Message));
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Tracer, model, ex);
            }
            finally
            {
                SetViewModelProperties(model, model.ViewConfig.ReadOnly, model.ViewConfig.PId, model.ViewConfig.PType, model.ViewConfig.ExtendedInfo);
                _viewModelCustomizationService.CustomizeViewModel<TModel, TEntity>(model, ModelState);
            }

            UpdateValidationMessages(model);

            var jsonNetResult = new JsonNetResult(model);

            return jsonNetResult;
        }

        private TModel CreateOrUpdate(TModel model)
        {
            var domainEntityDto = model.TransformToDomainEntityDto();

            var modifyService = _operationServicesManager.GetModifyDomainEntityService(typeof(TEntity).AsEntityName());
            var entityId = modifyService.Modify(domainEntityDto);
            
            if (model.Id == 0)
            {   
                // Здесь фактически нанооптимизация под конкретного клиента - имеющегося в данный момент (01.2015) extjs webclient. 
                // Суть оптимизации - не тратим время на процессинг viewmodel, так как знаем, что в нашем конкретном клиенте все что нужно из этой view model - это факт её прихода от сервера, и значение id в нем,
                // далее в любом случае view будет перезагружена через подмену window.location => пойдем по части GET update usecase и в любом случае будем процессить viewmodel. 
                // Однако в случае разных клиентов, разделения на create и update, трансформации web приложения в restapi могут возникнуть вопросы.
                model.Id = entityId;
            }
            else
            {
                var tmpViewConfig = model.ViewConfig;
                model = GetViewModel(entityId, model.ViewConfig.ReadOnly, model.ViewConfig.PId, model.ViewConfig.PType, model.ViewConfig.ExtendedInfo);
                model.ViewConfig = tmpViewConfig;
                model.SetEntityStateToken();
            }

            return model;
        }

        private TModel GetViewModel(long entityId, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var model = new TModel();

            var service = _operationServicesManager.GetDomainEntityDtoService(typeof(TEntity).AsEntityName());
            var domainEntityDto = service.GetDomainEntityDto(entityId, readOnly, parentEntityId, parentEntityName, extendedInfo);

            model.LoadDomainEntityDto(domainEntityDto);

            if (model.IsAuditable)
            {
                var createdBy = domainEntityDto.GetPropertyValue<IDomainEntityDto, EntityReference>("CreatedByRef").Id;
                if (createdBy > 0)
                {
                    model.CreatedBy = new LookupField
                        {
                            Key = createdBy,
                            Value = _userIdentifierService.GetUserInfo(createdBy).DisplayName
                        };
                }

                model.CreatedOn = domainEntityDto.GetPropertyValue<IDomainEntityDto, IAuditableEntity, DateTime>(x => x.CreatedOn);

                var modifiedBy = domainEntityDto.GetPropertyValue<IDomainEntityDto, EntityReference>("ModifiedByRef").Id;
                if (modifiedBy.HasValue && modifiedBy.Value != 0)
                {
                    model.ModifiedBy = new LookupField
                        {
                            Key = modifiedBy,
                            Value = _userIdentifierService.GetUserInfo(modifiedBy.Value).DisplayName
                        };
                }

                model.ModifiedOn = domainEntityDto.GetPropertyValue<IDomainEntityDto, IAuditableEntity, DateTime?>(x => x.ModifiedOn);
            }

            if (model.IsCurated)
            {
                var ownerCode = domainEntityDto.GetPropertyValue<IDomainEntityDto, EntityReference>("OwnerRef").Id;
                if (ownerCode > 0)
                {
                    model.Owner = new LookupField
                        {
                            Key = ownerCode,
                            Value = _userIdentifierService.GetUserInfo(ownerCode).DisplayName
                        };
                }
            }

            if (model.IsDeactivatable)
            {
                model.IsActive = domainEntityDto.GetPropertyValue<IDomainEntityDto, IDeactivatableEntity, bool>(x => x.IsActive);
            }

            if (model.IsDeletable)
            {
                model.IsDeleted = domainEntityDto.GetPropertyValue<IDomainEntityDto, IDeletableEntity, bool>(x => x.IsDeleted);
            }

            if (model.IsStateTracking)
            {
                model.Timestamp = domainEntityDto.GetPropertyValue<IDomainEntityDto, IStateTrackingEntity, byte[]>(x => x.Timestamp);
            }

            return model;
        }

        private void SetViewModelProperties(TModel model, bool readOnly, long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            model.ViewConfig.Id = model.Id;
            model.ViewConfig.EntityName = typeof(TEntity).AsEntityName();
            model.ViewConfig.PId = parentEntityId;
            model.ViewConfig.PType = parentEntityName;
            model.ViewConfig.ExtendedInfo = extendedInfo;

            // TODO {d.ivanov, 22.03.2013}: Далее написан код, который регулирует возможности создания/редактирования, основывясь на правах пользователя
            //                              -> При реализации WPF-клиента решить должен ли этот код переехать в GetDomainEntityDtoServiceBase
            var ownerCode = UserContext.Identity.Code;
            var oldOwnerCode = (long?)null;

            if (model.IsCurated && model.Owner != null)
                {
                    ownerCode = model.Owner.Key.Value;
                    oldOwnerCode = model.OldOwnerCode;
                }

            // check security access
            var entityAccess = _entityAccessService.RestrictEntityAccess(typeof(TEntity).AsEntityName(),
                                                                        EntityAccessTypes.All,
                                                                        UserContext.Identity.Code,
                                                                        model.Id,
                                                                        ownerCode,
                                                                        oldOwnerCode);
            if (!entityAccess.HasFlag(EntityAccessTypes.Create) && model.IsNew)
            {
                throw new SecurityAccessDeniedException(BLResources.SecurityAccess_HasNoCreatePlivilege);
            }

            if (!entityAccess.HasFlag(EntityAccessTypes.Read))
            {
                throw new SecurityAccessDeniedException(BLResources.SecurityAccess_HasNoReadPlivilege);
            }

            // Решение о запрете редактирования карточки принимается на основе 3х значений:
            // 1) на основе разрешений на редактирование.
            bool securityReadonlyMode = !entityAccess.HasFlag(EntityAccessTypes.Update) ||
                                        (model.IsDeletable && model.IsDeleted) ||
                                        (model.IsDeactivatable && !model.IsActive);

            // 2) значение, заданное в конкретном контроллере в реализации метода GetViewModel.
            bool baseReadonlyMode = model.ViewConfig.ReadOnly;

            // 3) значение, запрошенное в query string.
            model.ViewConfig.ReadOnly = securityReadonlyMode || baseReadonlyMode || readOnly;

            model.ViewConfig.CardSettings = _cardSettingsProvider.GetCardSettings<TEntity>(UserContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        private void UpdateValidationMessages(TModel model)
        {
            model.ValidationMessages = ModelState
                .Where(keyValuePair => keyValuePair.Value.Errors.Any())
                .Select(keyValuePair => new ValidationMessage { For = keyValuePair.Key, Message = keyValuePair.Value.Errors.First().ErrorMessage })
                .ToList();
        }
    }
}
