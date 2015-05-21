using System.Linq;
using System.Web;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Model.Common.Entities;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Specifications;
using NuClear.Tracing.API;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class NoteController : ControllerBase
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;

        public NoteController(IMsCrmSettings msCrmSettings,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IAPIIdentityServiceSettings identityServiceSettings,
                              IUserContext userContext,
                              ITracer tracer,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              ISecurityServiceUserIdentifier userIdentifierService,
                              IFinder finder)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
        }

        public JsonNetResult GetEntityNotes(IEntityType entityType, long entityId)
        {
            var entityTypeId = entityType.Id;
            var data = _finder.Find(new FindSpecification<Note>(note => note.ParentType == entityTypeId && note.ParentId == entityId && note.IsDeleted == false))
                              .Select(note => new
                                  {
                                      note.Id,
                                      note.Title,
                                      note.Text,
                                      note.FileId,
                                      note.File.FileName,
                                      note.CreatedOn,
                                      note.CreatedBy,
                                      note.ModifiedOn,
                                      note.ModifiedBy,
                                      note.OwnerCode
                                  })
                              .OrderByDescending(x => x.Id)
                              .AsEnumerable()
                              .Select(x => new
                                  {
                                      x.Id,
                                      x.Title,
                                      Text = HttpUtility.HtmlEncode(x.Text),
                                      x.FileId,
                                      x.FileName,
                                      x.CreatedOn,
                                      x.ModifiedOn,
                                      CreatedBy = _userIdentifierService.GetUserInfo(x.CreatedBy).DisplayName,
                                      ModifiedBy = _userIdentifierService.GetUserInfo(x.ModifiedBy).DisplayName,
                                      Readonly = x.OwnerCode != UserContext.Identity.Code
                                  })
                              .ToArray();
            return new JsonNetResult(new { items = data });
        }
    }
}
