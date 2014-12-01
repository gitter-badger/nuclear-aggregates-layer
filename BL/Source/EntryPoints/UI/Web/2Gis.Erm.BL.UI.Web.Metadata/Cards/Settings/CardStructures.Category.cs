﻿using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Category =
            CardMetadata.For<Category>()
                        .MainAttribute<Category, ICategoryViewModel>(x => x.Name)                
                        .Actions
                            .Attach(UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(
                                    UiElementMetadata.Config
                                                     .Name.Static("Category")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelCategory)
                                                     .LockOnNew()
                                                     .Icon.Path("en_ico_16_Category.gif")
                                                     .Handler.ShowGridByConvention(EntityName.Category),
                                    UiElementMetadata.Config
                                                     .Name.Static("CategoryOrganizationUnit")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelCategoryOU)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.CategoryOrganizationUnit));
    }
}