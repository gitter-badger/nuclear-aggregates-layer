using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers;
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

        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var advertisementElementModel = (AdvertisementElementViewModel)viewModel;
            advertisementElementModel.ViewConfig.ReadOnly |=
                advertisementElementModel.DisableEdit ||
                advertisementElementModel.CanUserChangeStatus ||
                (advertisementElementModel.NeedsValidation && advertisementElementModel.Status != AdvertisementElementStatusValue.Draft);
            
            if (advertisementElementModel.FasComment != null)
            {
                advertisementElementModel.FasComment.FasCommentDisplayTextItemsJson = GetDisplayTextItemsJson();
            }

            if (advertisementElementModel.CanUserChangeStatus || advertisementElementModel.DisableEdit)
            {
                advertisementElementModel.ViewConfig.DisableCardToolbarItem("ResetToDraft");
            }

            advertisementElementModel.ViewConfig.DisableCardToolbarItem(advertisementElementModel.Status == AdvertisementElementStatusValue.Draft
                                                                            ? "ResetToDraft"
                                                                            : "SaveAndVerify");

            var itemsToDelete = advertisementElementModel.NeedsValidation ? new[] { "Save", "SaveAndClose" } : new[] { "ResetToDraft", "SaveAndVerify" };

            advertisementElementModel.ViewConfig.CardSettings.CardToolbar =
                advertisementElementModel.ViewConfig.CardSettings.CardToolbar.Where(x => !itemsToDelete.Contains(x.Name)).ToArray();
        }

        public static string GetDisplayTextItemsJson()
        {
            return JsonConvert.SerializeObject(FasCommentToDisplayTextMapping.ToDictionary(x => EnumUIUtils.GetEnumName(x.Key, true), x => x.Value()));
        }
    }
}