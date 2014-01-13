using System;
using System.Linq;
using System.Net.Mime;
using System.ServiceModel.Security;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations
{
    public sealed class CreateOrUpdateController<TEntity, TModel> : ControllerBase
        where TEntity : class, IEntityKey
        where TModel : EntityViewModelBase<TEntity>, new()
    {
        private readonly IUIConfigurationService _uiConfigurationService;
        private readonly IUIServicesManager _uiServicesManager;
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;

        public CreateOrUpdateController(IMsCrmSettings msCrmSettings,
                                        IUserContext userContext,
                                        ICommonLog logger,
                                        IUIConfigurationService uiConfigurationService,
                                        IUIServicesManager uiServicesManager,
                                        IOperationServicesManager operationServicesManager,
                                        ISecurityServiceUserIdentifier userIdentifierService,
                                        ISecurityServiceFunctionalAccess functionalAccessService,
                                        ISecurityServiceEntityAccess entityAccessService,
                                        IAPIOperationsServiceSettings operationsServiceSettings,
                                        IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _uiConfigurationService = uiConfigurationService;
            _uiServicesManager = uiServicesManager;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _entityAccessService = entityAccessService;
        }

        [HttpGet, SetEntityStateToken, UseDependencyFields]
        public ActionResult Entity(long? entityId, bool? readOnly, long? pId, EntityName pType, string extendedInfo)
        {
            var actualEntityId = entityId ?? 0;
            var actualReadOnly = readOnly ?? false;

            var model = GetViewModel(actualEntityId, actualReadOnly, pId, pType, extendedInfo);
            SetViewModelProperties(model, actualReadOnly, pId, pType, extendedInfo);
            CustomizeModelAfterMetadataReady(model);

            ApplyToolbarItemsLock(model);

            var entityTypeName = typeof(TEntity).Name;
            var viewName = GetViewName(model, entityTypeName);
            return View(viewName, model);
        }

        private static string GetViewName(TModel model, string entityTypeName)
        {
            if (model is ICyprusAdapted)
            {
                return string.Format("Cyprus{0}", entityTypeName);
            }

            if (model is ICzechAdapted)
            {
                return string.Format("Czech{0}", entityTypeName);
            }

            return entityTypeName;
        }

        [HttpPost, GetEntityStateToken, UseDependencyFields]
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
                ModelUtils.OnException(this, Logger, model, new NotificationException(ex.Message));
            }
            catch (NotSupportedException ex)
            {
                ModelUtils.OnException(this, Logger, model, new NotificationException(ex.Message));
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Logger, model, ex);
            }
            finally
            {
                SetViewModelProperties(model, model.ViewConfig.ReadOnly, model.ViewConfig.PId, model.ViewConfig.PType, model.ViewConfig.ExtendedInfo);
                CustomizeModelAfterMetadataReady(model);
                ApplyToolbarItemsLock(model);
            }

            UpdateValidationMessages(model);

            var jsonNetResult = new JsonNetResult(model);

            // hack for ExtJs upload fie control
            if (model is FileViewModel<TEntity>)
            {
                jsonNetResult.ContentType = MediaTypeNames.Text.Html;
            }

            return jsonNetResult;
        }

        private TModel CreateOrUpdate(TModel model)
        {
            var domainEntityDto = model.TransformToDomainEntityDto();

            var modifyService = _operationServicesManager.GetModifyDomainEntityService(typeof(TEntity).AsEntityName());
            var entityId = modifyService.Modify(domainEntityDto);
            
            if (model.Id == 0)
            {   
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

        private TModel GetViewModel(long entityId, bool readOnly, long? parentEntityId, EntityName parentEntityName, string extendedInfo)
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

            return model;
        }

        private void SetViewModelProperties(TModel model, bool readOnly, long? parentEntityId, EntityName parentEntityName, string extendedInfo)
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

            var cardSettings = _uiConfigurationService.GetCardSettings(typeof(TEntity).AsEntityName(), UserContext.Profile.UserLocaleInfo.UserCultureInfo);

            // NOTE: Внимание!!!
            // Ниже жесткий копипаст из EntityGridViewServiceBase.SecureViewsToolbars для поддержания работы с тубарами карточки
            // TODO: Учесть текущую реализацию работы с тулбарами и дотюнить ее при реализации EditController. 
            foreach (var toolbarItem in cardSettings.CardToolbar)
            {
                // Для всех сущностей кнопки тулбара блокируются в случае отсутствия соответствующей привилегии,
                // либо, если сущность неактивна/удалена и в настройках флаг LockOnInactive = true
                if (toolbarItem.SecurityPrivelege.HasValue && toolbarItem.SecurityPrivelege.Value != 0)
                {
                    var privilegeMask = toolbarItem.SecurityPrivelege.Value;
                    if (Enum.IsDefined(typeof(FunctionalPrivilegeName), privilegeMask))
                    {
                        toolbarItem.Disabled = !_functionalAccessService.HasFunctionalPrivilegeGranted((FunctionalPrivilegeName)privilegeMask,
                                                                                                      UserContext.Identity.Code);
                    }
                    else
                    {
                        toolbarItem.Disabled = !entityAccess.HasFlag((EntityAccessTypes)privilegeMask);
                    }
                }
            }

            model.ViewConfig.CardSettings = cardSettings.ToCardJson();
        }

        private void CustomizeModelAfterMetadataReady(TModel model)
        {
            var viewModelCustomizationService = _uiServicesManager.GetModelCustomizationService(typeof(TEntity).AsEntityName());
            viewModelCustomizationService.CustomizeViewModel(model, ModelState);
        }

        /// <summary>
        /// Финальный дисейблинг кнопок, должен вызываться после метода CustomizeModelAfterMetadataReady, т.к. основывается на значении model.ViewConfig.ReadOnly.
        /// </summary>
        /// <param name="model"></param>
        private void ApplyToolbarItemsLock(TModel model)
        {
            foreach (var toolbarItem in model.ViewConfig.CardSettings.CardToolbar.Where(t => !t.Disabled))
            {
                // Если кнопка не заблокирована в результате проверки привилегий, блокируем ее на основании EntitySettings
                toolbarItem.Disabled |= (model.ViewConfig.ReadOnly && toolbarItem.LockOnInactive) || (toolbarItem.LockOnNew && model.Id == 0);
            }                
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
