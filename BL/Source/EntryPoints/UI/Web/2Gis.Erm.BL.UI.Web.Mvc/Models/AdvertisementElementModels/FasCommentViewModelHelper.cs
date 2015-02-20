using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels
{
    public static class FasCommentViewModelHelper
    {
        private static readonly Dictionary<FasComment, Func<string>> FasCommentToDisplayTextMapping = new Dictionary<FasComment, Func<string>>
            {
                { FasComment.Alcohol, () => EnumResources.FasCommentDisplayTextAlcohol },
                { FasComment.Supplements, () => EnumResources.FasCommentDisplayTextSupplements },
                { FasComment.Drugs, () => EnumResources.FasCommentDisplayTextDrugs },
                { FasComment.DrugsAndService, () => EnumResources.FasCommentDisplayTextDrugsAndService },
                { FasComment.NewFasComment, () => EnumResources.FasCommentNone },
                { FasComment.AlcoholAdvertising, () => EnumResources.FasCommentDisplayTextAlcoholAdvertising },

                { FasComment.MedsMultiple, () => EnumResources.FasCommentDisplayTextMedsMultiple },
                { FasComment.MedsSingle, () => EnumResources.FasCommentDisplayTextMedsSingle },
                { FasComment.DietarySupplement, () => EnumResources.FasCommentDisplayTextDietarySupplement },
                { FasComment.SpecialNutrition, () => EnumResources.FasCommentDisplayTextSpecialNutrition },
                { FasComment.ChildNutrition, () => EnumResources.FasCommentDisplayTextChildNutrition },
                { FasComment.FinancilaServices, () => EnumResources.FasCommentDisplayTextFinancilaServices },
                { FasComment.MedsTraditional, () => EnumResources.FasCommentDisplayTextMedsTraditional },
                { FasComment.Biocides, () => EnumResources.FasCommentDisplayTextBiocides },

                { FasComment.UkraineAutotherapy, () => EnumResources.FasCommentDisplayTextUkraineAutotherapy },
                { FasComment.UkraineDrugs, () => EnumResources.FasCommentDisplayTextUkraineDrugs },
                { FasComment.UkraineMedicalDevice, () => EnumResources.FasCommentDisplayTextUkraineMedicalDevice },
                { FasComment.UkraineAlcohol, () => EnumResources.FasCommentDisplayTextUkraineAlcohol },
                { FasComment.UkraineSoundPhonogram, () => EnumResources.FasCommentDisplayTextUkraineSoundPhonogram },
                { FasComment.UkraineSoundLive, () => EnumResources.FasCommentDisplayTextUkraineSoundLive },
                { FasComment.UkraineEmploymentAssistance, () => EnumResources.FasCommentDisplayTextUkraineEmploymentAssistance },

                { FasComment.ChileAlcohol, () => EnumResources.FasCommentDisplayTextChileAlcohol },
                { FasComment.ChileDrugsAndService, () => EnumResources.FasCommentDisplayTextChileDrugsAndService },
                { FasComment.ChileMedicalReceiptDrugs, () => EnumResources.FasCommentDisplayTextChileMedicalReceiptDrugs },
            };

        public static string GetDisplayTextItemsJson()
        {
            return JsonConvert.SerializeObject(FasCommentToDisplayTextMapping.ToDictionary(x => EnumUIUtils.GetEnumName(x.Key, true), x => x.Value()));
        }
    }
}

