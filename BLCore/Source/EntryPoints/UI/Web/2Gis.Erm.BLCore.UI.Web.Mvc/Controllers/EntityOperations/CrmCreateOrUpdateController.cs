using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

using DoubleGis.Erm.BLCore.API.Common.Crosscutting;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.App_Start;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

using Newtonsoft.Json;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;
using DoubleGis.Erm.Platform.DAL.Specifications;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations
{
    public sealed class CrmCreateOrUpdateController<TEntity> : ControllerBase
        where TEntity : class, IEntity, IReplicableEntity, new()
    {
        private readonly IReplicationCodeConverter _replicationCodeConverter;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IPublicService _publicService;
        private readonly ISecureFinder _secureFinder;
        private readonly ISecurityServiceSharings _securityServiceSharings;

        public CrmCreateOrUpdateController(IMsCrmSettings msCrmSettings,
                                           IAPIOperationsServiceSettings operationsServiceSettings,
                                           IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                           IAPIIdentityServiceSettings identityServiceSettings,
                                           IUserContext userContext,
                                           ITracer tracer,
                                           IGetBaseCurrencyService getBaseCurrencyService,
                                           IReplicationCodeConverter replicationCodeConverter,
                                           ISecurityServiceEntityAccess entityAccessService,
                                           IPublicService publicService,
                                           ISecureFinder secureFinder,
                                           ISecurityServiceSharings securityServiceSharings)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _replicationCodeConverter = replicationCodeConverter;
            _entityAccessService = entityAccessService;
            _publicService = publicService;
            _secureFinder = secureFinder;
            _securityServiceSharings = securityServiceSharings;
        }

        public ActionResult Redirect(Guid? crmId)
        {
            var routeValues = new RouteValueDictionary(new { readOnly = false, pId = (long?)null, pType = EntityName.None, extendedInfo = (string)null });
            if (crmId.HasValue)
            {
                routeValues.Add("entityId", _replicationCodeConverter.ConvertToEntityId(typeof(TEntity).AsEntityName(), crmId.Value));
            }

            return RedirectToRoute(RouteConfig.CreateOrUpdateRoute, routeValues);
        }

        public ActionResult EditAccessSharings(long id)
        {
            // existing sharings
            var sharings = _securityServiceSharings.GetAccessSharingsForEntity(typeof(TEntity).AsEntityName(), id);

            // privileges
            var entity = _secureFinder.FindOne<TEntity>(Specs.Find.ById<TEntity>(id));
            var entitySecure = (ICuratedEntity)entity;
            var entityPrivileges = _entityAccessService.RestrictEntityAccess(typeof(TEntity).AsEntityName(),
                                                                             EntityAccessTypes.All,
                                                                             UserContext.Identity.Code,
                                                                             id,
                                                                             entitySecure.OwnerCode,
                                                                             entitySecure.OldOwnerCode);

            var sharingsModel = sharings.Select(x => new AccessSharingRowModel
            {
                UserAccountId = x.UserInfo.Code,
                UserAccountName = x.UserInfo.DisplayName,
                CanCreate = x.AccessTypes.HasFlag(EntityAccessTypes.Create),
                CanRead = x.AccessTypes.HasFlag(EntityAccessTypes.Read),
                CanUpdate = x.AccessTypes.HasFlag(EntityAccessTypes.Update),
                CanDelete = x.AccessTypes.HasFlag(EntityAccessTypes.Delete),
                CanAssign = x.AccessTypes.HasFlag(EntityAccessTypes.Assign),
                CanShare = x.AccessTypes.HasFlag(EntityAccessTypes.Share)
            }).ToArray();

            var jsonData = new
            {
                GridData = new
                {
                    Data = sharingsModel,
                    Rows = sharingsModel.Length
                },
                LayoutData = new
                {
                    NameColumnLocalizedName = BLResources.AccessSharingName,
                    IdColumnLocalizedName = BLResources.AccessSharingId,
                    UserAlreadyInListMessageFormat = BLResources.AccessSharingUserAlreadyInListMessageFormat,
                    AccessRightColumns = new[]
                    {
                        new { Name = "CanRead", LocalizedName = BLResources.AccessSharingRead, ReadOnly = !entityPrivileges.HasFlag(EntityAccessTypes.Read) },
                        new { Name = "CanUpdate", LocalizedName = BLResources.AccessSharingUpdate, ReadOnly = !entityPrivileges.HasFlag(EntityAccessTypes.Update) },
                        new { Name = "CanDelete", LocalizedName = BLResources.AccessSharingDelete, ReadOnly = !entityPrivileges.HasFlag(EntityAccessTypes.Delete) },
                        new { Name = "CanShare", LocalizedName = BLResources.AccessSharingShare, ReadOnly = !entityPrivileges.HasFlag(EntityAccessTypes.Share) },
                        new { Name = "CanAssign", LocalizedName = BLResources.AccessSharingAssign, ReadOnly = !entityPrivileges.HasFlag(EntityAccessTypes.Assign) }
                    }
                }
            };

            var entityReplicable = (IReplicableEntity)entity;

            var model = new AccessSharingModel
            {
                EntityId = id,
                EntityTypeName = typeof(TEntity).AsEntityName(),
                EntityOwnerId = entitySecure.OwnerCode,
                EntityReplicationCode = entityReplicable.ReplicationCode,
                JsonData = JsonConvert.SerializeObject(jsonData),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditAccessSharings(AccessSharingModel model)
        {
            var userIdentities = JsonConvert.DeserializeObject<List<AccessSharingRowModel>>(model.JsonData).Select(x =>
            {
                var result = new SharingDescriptor { UserInfo = new UserInfo(x.UserAccountId, null, x.UserAccountName) };

                if (x.CanCreate)
                {
                    result.AccessTypes |= EntityAccessTypes.Create;
                }

                if (x.CanRead)
                {
                    result.AccessTypes |= EntityAccessTypes.Read;
                }

                if (x.CanUpdate)
                {
                    result.AccessTypes |= EntityAccessTypes.Update;
                }

                if (x.CanDelete)
                {
                    result.AccessTypes |= EntityAccessTypes.Delete;
                }

                if (x.CanAssign)
                {
                    result.AccessTypes |= EntityAccessTypes.Assign;
                }

                if (x.CanShare)
                {
                    result.AccessTypes |= EntityAccessTypes.Share;
                }

                return result;
            });

            _publicService.Handle(new EditAccessSharingRequest
                {
                    EntityType = model.EntityTypeName,
                    EntityId = model.EntityId,
                    EntityOwnerId = model.EntityOwnerId,
                    EntityReplicationCode = model.EntityReplicationCode,
                    AccessSharings = userIdentities,
                });

            return RedirectToAction("EditAccessSharings", model.EntityId);
        }

        private sealed class AccessSharingRowModel
        {
            public long UserAccountId { get; set; }
            public string UserAccountName { get; set; }

            public bool CanRead { get; set; }
            public bool CanUpdate { get; set; }
            public bool CanCreate { get; set; }
            public bool CanDelete { get; set; }
            public bool CanShare { get; set; }
            public bool CanAssign { get; set; }
        }
    }
}
