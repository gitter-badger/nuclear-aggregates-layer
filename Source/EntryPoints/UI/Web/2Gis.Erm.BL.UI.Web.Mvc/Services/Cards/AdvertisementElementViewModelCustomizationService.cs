using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class AdvertisementElementViewModelCustomizationService : IGenericViewModelCustomizationService<AdvertisementElement>
    {
        private static readonly Dictionary<FasComment, Func<string>> FasCommentToDisplayTextMapping = new Dictionary<FasComment, Func<string>>
            {
                { FasComment.Alcohol, () => EnumResources.FasCommentDisplayTextAlcohol },
                { FasComment.Supplements, () => EnumResources.FasCommentDisplayTextSupplements },
                { FasComment.Smoke, () => EnumResources.FasCommentDisplayTextSmoke },
                { FasComment.Drugs, () => EnumResources.FasCommentDisplayTextDrugs },
                { FasComment.DrugsAndService, () => EnumResources.FasCommentDisplayTextDrugsAndService },
                { FasComment.Abortion, () => EnumResources.FasCommentDisplayTextAbortion },
                { FasComment.NewFasComment, () => EnumResources.FasCommentNone },
                { FasComment.AlcoholAdvertising, () => EnumResources.FasCommentDisplayTextAlcoholAdvertising },

                { FasComment.MedsMultiple, () => EnumResources.FasCommentDisplayTextMedsMultiple },
                { FasComment.MedsSingle, () => EnumResources.FasCommentDisplayTextMedsSingle },
                { FasComment.DietarySupplement, () => EnumResources.FasCommentDisplayTextDietarySupplement },
                { FasComment.SpecialNutrition, () => EnumResources.FasCommentDisplayTextSpecialNutrition },
                { FasComment.ChildNutrition, () => EnumResources.FasCommentDisplayTextChildNutrition },
                { FasComment.FinancilaServices, () => EnumResources.FasCommentDisplayTextFinancilaServices },
                { FasComment.MedsTraditional, () => EnumResources.FasCommentDisplayTextMedsTraditional },
                { FasComment.Biocides, () => EnumResources.FasCommentDisplayTextBiocides }
            };

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementElementModel = (AdvertisementElementViewModel)viewModel;
            advertisementElementModel.FasCommentDisplayTextItemsJson = GetDisplayTextItemsJson();
        }
        
        private static string GetDisplayTextItemsJson()
        {
            return JsonConvert.SerializeObject(FasCommentToDisplayTextMapping.ToDictionary(x => EnumUIUtils.GetEnumName(x.Key, true), x => x.Value()));
        }
    }
}