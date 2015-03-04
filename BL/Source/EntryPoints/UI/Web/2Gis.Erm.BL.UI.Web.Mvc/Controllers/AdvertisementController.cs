using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AdvertisementController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IFinder _finder;

        public AdvertisementController(IMsCrmSettings msCrmSettings,
                                       IAPIOperationsServiceSettings operationsServiceSettings,
                                       IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                       IAPIIdentityServiceSettings identityServiceSettings,
                                       IUserContext userContext,
                                       ICommonLog logger,
                                       IGetBaseCurrencyService getBaseCurrencyService,
                                       IPublicService publicService,
                                       IAdvertisementRepository advertisementRepository,
                                       IFinder finder)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _publicService = publicService;
            _advertisementRepository = advertisementRepository;
            _finder = finder;
        }

        public JsonNetResult GetAdvertisementBag(long id)
        {
            var advertisementBag = _advertisementRepository.GetAdvertisementBag(id);

            var data = advertisementBag.Select(x =>
                {
                    x.RestrictionName = x.RestrictionType.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture);
                    return x;
                })
                                       .OrderBy(x => x.IsValid)
                                       .ThenBy(x => x.Id)
                                       .ToArray();

            return new JsonNetResult(new { data });
        }


        public ActionResult Preview(long advertisementId)
        {
            var advertisement = _finder.Find<Advertisement>(x => x.Id == advertisementId).
                Select(x => new
                {
                    TemplateName = x.AdvertisementTemplate.Name,
                    Firm = x.Firm.Name,
                    Elements = x.AdvertisementElements.Select(e => new { e.AdsTemplatesAdsElementTemplate.ExportCode, e.Text })
                }).Single();


            var builder = new AdvertismentPreviewModelBuilder();
            foreach (var elem in advertisement.Elements)
            {
                builder.SetValueFor(new AdvertisementTypeId(advertisement.TemplateName, elem.ExportCode), elem.Text);
            } 

            AdvertisementElementPreviewModel model = builder.GetModel();
            model.OrgName = advertisement.Firm;


            PlatformEnum platform = builder.GetPlatform(advertisement.TemplateName);
            switch (platform)
            {
                case PlatformEnum.Desktop:
                    return View("PreviewGrym", model);
                case PlatformEnum.Api:
                    return View("PreviewOnline", model);
                default:
                    return View("PreviewIsNotSupported");
            }
        }

        [HttpPost]
        public JsonNetResult SelectWhiteListedAd(long? advertisementId, long? firmId)
        {
            if (!advertisementId.HasValue || !firmId.HasValue)
                return new JsonNetResult();

            _publicService.Handle(new SelectAdvertisementToWhiteListRequest { AdvertisementId = advertisementId.Value, FirmId = firmId.Value });
            return new JsonNetResult(true);
        }
    }

    internal sealed class AdvertismentPreviewModelBuilder
    {
        private readonly IDictionary<AdvertisementTypeId, Action<object>> _actinos;
        private readonly AdvertisementElementPreviewModel _model;

        public AdvertismentPreviewModelBuilder()
        {
            _model = new AdvertisementElementPreviewModel();
            _actinos = new Dictionary<AdvertisementTypeId, Action<object>>
                           {
                               //Desktop
                               {new AdvertisementTypeId("Одинарный комментарий", 1), o => _model.AdComment = (string) o},
                               {new AdvertisementTypeId("Двойной комментарий", 1), o => _model.AdComment = (string) o},
                               {new AdvertisementTypeId("Тройной комментарий", 1), o => _model.AdComment = (string) o},
                               {new AdvertisementTypeId("Комментарий из пакета \"Всё включено\"", 1), o => _model.AdComment = (string) o},
                               {new AdvertisementTypeId("Одинарный комментарий", 2), o => _model.AdCommentWarning = (string) o},
                               {new AdvertisementTypeId("Двойной комментарий", 2), o => _model.AdCommentWarning = (string) o},
                               {new AdvertisementTypeId("Тройной комментарий", 2), o => _model.AdCommentWarning = (string) o},
                               {new AdvertisementTypeId("Комментарий из пакета \"Всё включено\"", 2), o => _model.AdCommentWarning = (string) o},
                               {new AdvertisementTypeId("Микрокомментарий в рубрике", 1), o => _model.Microcomment = (string) o},
                               {new AdvertisementTypeId("Микрокомментарий в рубрике", 2), o => _model.MicrocommentWarning = (string) o},

                               //Online
                               {new AdvertisementTypeId("Абонентская плата в API/Online", 1), o => _model.Microcomment = (string) o},
                               {new AdvertisementTypeId("Абонентская плата в API/Online", 3), o => _model.AdComment = (string) o},
                               {new AdvertisementTypeId("Абонентская плата в API/Online", 8), o => _model.AdCommentWarning = (string) o},

                               //Online
                               {new AdvertisementTypeId("Абонентская плата в API/Online_старый шаблон", 1), o => _model.Microcomment = (string) o},
                               {new AdvertisementTypeId("Абонентская плата в API/Online_старый шаблон", 3), o => _model.AdComment = (string) o},
                               {new AdvertisementTypeId("Абонентская плата в API/Online_старый шаблон", 8), o => _model.AdCommentWarning = (string) o},

                           };
        }

        /// <summary>
        /// Инициализирует свойство модели, соответствующее <paramref name="exportId"/> значением <paramref name="property"/>
        /// </summary>
        public void SetValueFor(AdvertisementTypeId exportId, object property)
        {
            Action<object> action;
            if (_actinos.TryGetValue(exportId, out action))
                action.Invoke(property);
        }

        public AdvertisementElementPreviewModel GetModel()
        {
            FillModelWithFakeData(_model);
            return _model;
        }


        private static void FillModelWithFakeData(AdvertisementElementPreviewModel model)
        {
            model.Rubrics = new object[] { "Компьютеры / Бытовая техника / Офисная техника", "Досуг / Развлечения / Общественное питание", "Коммунальные / бытовые / ритуальные услуги" };
            model.Address = "Карла Маркса площадь, 7";
            model.WorkTime = "Круглосуточно";
            model.ContactGroups = new[]
                                      {
                                          new ContactGroup
                                              {
                                                  Contacts = new[] {"(383) 380-39-17", "383@gmail.com"}
                                              }
                                      };

            model.Payments = new[]
                                 {
                                     new Payment {Name = "payments", Image = "images/Erm/Preview/Grym/Payments.png"}
                                 };
        }
        
        public PlatformEnum GetPlatform(string templateName)
        {
            if (_actinos.Keys.All(x => x.Name != templateName))
            {
                return PlatformEnum.Mobile;
            }

            return templateName == "Абонентская плата в API/Online" || templateName == "Абонентская плата в API/Online_старый шаблон"
                       ? PlatformEnum.Api
                       : PlatformEnum.Desktop;
        }
    }


    /// <summary>
    /// Используется для идентификаци типа элемента рекламного материала.
    /// </summary>
    internal class AdvertisementTypeId : IEquatable<AdvertisementTypeId>
    {
        /// <summary>
        /// Имя шаблона рекламного материала.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Код экспорта элемента рекламного материала.
        /// </summary>
        public int ExportId { get; set; }

        public AdvertisementTypeId(string name, int exportId)
        {
            Name = name;
            ExportId = exportId;
        }

        public bool Equals(AdvertisementTypeId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.ExportId == ExportId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(AdvertisementTypeId)) return false;
            return Equals((AdvertisementTypeId)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ ExportId;
            }
        }

        public static bool operator ==(AdvertisementTypeId left, AdvertisementTypeId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AdvertisementTypeId left, AdvertisementTypeId right)
        {
            return !Equals(left, right);
        }

    }
}
