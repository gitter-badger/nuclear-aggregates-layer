using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLFlex.UI.Metadata
{
    public sealed partial class CardMetadataValidator
    {
        private readonly IDictionary<IEntityType, IDictionary<string, IDictionary<string, Tuple<object, object>>>> _cardMetadataCorrections =
            new Dictionary<IEntityType, IDictionary<string, IDictionary<string, Tuple<object, object>>>>
                {
                    #region Order
                    {
                        EntityType.Instance.Order(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                 {
                                    "Order",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("OrderNumber", "Number")
                                            },
                                        }
                                },
                                {
                                    "Save",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "SaveAndClose",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "PrintOrderAction",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "PrintLetterOfGuarantee",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "ChangeDeal",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)(EntityAccessTypes.Create | EntityAccessTypes.Update), (int)(EntityAccessTypes.Create | EntityAccessTypes.Update) | (int)FunctionalPrivilegeName.OrderChangeDealExtended)
                                            },
                                        }
                                },
                                {
                                    "CheckOrder",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "CloseWithDenial",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "CopyOrder",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "SwitchToAccount",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region LegalPerson
                    {
                        EntityType.Instance.LegalPerson(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "ChangeLegalPersonClient",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "ChangeLPRequisites",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Bargain
                    {
                        EntityType.Instance.Bargain(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "ContentTab",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>(null, "en_ico_16_Default.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Bank
                    {
                        EntityType.Instance.Bank(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Bank",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityLocalizedName",
                                                new Tuple<object, object>(ErmConfigLocalization.EnAssociatedPositionsGroup, EnumResources.EntityNameBank)
                                            },
                                            {
                                                "EntityNameLocaleResourceId",
                                                new Tuple<object, object>("EnAssociatedPositionsGroup", "EntityNameBank")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Contact
                    {
                        EntityType.Instance.Contact(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Activities",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_16_Action.gif", "en_ico_16_Activity.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Deal
                    {
                        EntityType.Instance.Deal(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "ReopenDeal",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "ChangeDealClient",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "Activities",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_16_Action.gif", "en_ico_16_Activity.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Client
                    {
                        EntityType.Instance.Client(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "ChangeTerritory",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "Qualify",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "Disqualify",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "Merge",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)FunctionalPrivilegeName.MergeClients)
                                            },
                                        }
                                },
                                {
                                    "Activities",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_16_Action.gif", "en_ico_16_Activity.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region OrderProcessingRequest
                    {
                        EntityType.Instance.OrderProcessingRequest(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "CreateOrder",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.Update, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                                {
                                    "CancelOrderProcessingRequest",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Firm
                    {
                        EntityType.Instance.Firm(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Save",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                                {
                                    "SaveAndClose",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                                {
                                    "Qualify",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "CategoryFirmAddresses",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ExtendedInfo",
                                                new Tuple<object, object>("firmId={Id}", "filterToParent=true")
                                            },
                                        }
                                },
                                {
                                    "ChangeTerritory",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)FunctionalPrivilegeName.ChangeFirmTerritory)
                                            },
                                        }
                                },
                                {
                                    "Activities",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_16_Action.gif", "en_ico_16_Activity.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region FirmAddress
                    {
                        EntityType.Instance.FirmAddress(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Save",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                                {
                                    "SaveAndClose",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region AdvertisementElementStatus
                    {
                        EntityType.Instance.AdvertisementElementStatus(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "AdvertisementElementStatus",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityNameLocaleResourceId",
                                                new Tuple<object, object>("EnAdvertisementElementStatus", "EntityNameAdvertisementElementStatus")
                                            },
                                            {
                                                "EntityLocalizedName",
                                                new Tuple<object, object>("EnAdvertisementElementStatus", EnumResources.EntityNameAdvertisementElementStatus)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region AdvertisementTemplate
                    {
                        EntityType.Instance.AdvertisementTemplate(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "PublishAdvertisementTemplate",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>(null, (int)FunctionalPrivilegeName.PublishAdvertisementTemplate)
                                            },
                                        }
                                },
                                {
                                    "UnpublishAdvertisementTemplate",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>(null, (int)FunctionalPrivilegeName.UnpublishAdvertisementTemplate)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region User
                    {
                        EntityType.Instance.User(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "User",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_lrg_UserAccount.gif", "en_ico_lrg_User.gif")
                                            },
                                        }
                                },
                                {
                                    "ShowUserProfile",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Territory
                    {
                        EntityType.Instance.Territory(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Activate",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "ContentTab",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>(null, "en_ico_16_Default.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Theme
                    {
                        EntityType.Instance.Theme(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "ThemeCategories",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "DisabledExpression",
                                                // Условие будет проверено на сервере
                                                new Tuple<object, object>(@"Ext.getDom(""Id"").value==0||Ext.getDom(""OrganizationUnitCount"").value==0",
                                                                          @"Ext.getDom(""Id"").value==0")
                                            },
                                        }
                                },
                                {
                                    "ContentTab",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>(null, "en_ico_16_Default.gif")
                                            },
                                        }
                                },
                                {
                                    "Actions",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region ReleaseInfo
                    {
                        EntityType.Instance.ReleaseInfo(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "DownloadResults",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                                {
                                    "Actions",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Project
                    {
                        EntityType.Instance.Project(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Project",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_16_Default.gif", "en_ico_lrg_Default.gif")
                                            },
                                        }
                                },
                                {
                                    "Save",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.Update, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                                {
                                    "SaveAndClose",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.Update, (int)(EntityAccessTypes.Update | EntityAccessTypes.Create))
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Currency
                    {
                        EntityType.Instance.Currency(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "CurrencyRates",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "DisabledExpression",

                                                // Часть про базовую валюту будет проверена на сервере
                                                new Tuple<object, object>(@"Ext.getDom(""Id"").value==0 || Ext.getDom(""IsBase"").checked", @"Ext.getDom(""Id"").value==0")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region CurrencyRate
                    {
                        EntityType.Instance.CurrencyRate(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "CurrencyRate",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>(@"Name", @"Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Lock
                    {
                        EntityType.Instance.Lock(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Lock",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>(@"Name", @"Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region LockDetail
                    {
                        EntityType.Instance.LockDetail(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "LockDetail",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>(@"Name", @"Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Role
                    {
                        EntityType.Instance.Role(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Role",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_lrg_UserGroup.gif", "en_ico_lrg_Role.gif")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region RolePrivilege
                    {
                        EntityType.Instance.RolePrivilege(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "RolePrivilege",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_lrg_UserAccountGroup.gif", "en_ico_lrg_RolePrivilege.gif")
                                            },
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("Name", "Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region OrderPosition
                    {
                        EntityType.Instance.OrderPosition(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "OrderPosition",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>(@"Name", @"Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region PricePosition
                    {
                        EntityType.Instance.PricePosition(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "PricePosition",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("PositionName", "PositionName")
                                            },
                                        }
                                },
                                {
                                    "CopyPricePosition",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.ImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "Actions",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                                {
                                    "DeniedPositions",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ExtendedInfo",
                                                // ExtendedInfo будет подготовлен на сервере с конкретными числами
                                                new Tuple<object, object>("PositionId={PositionId}&&PriceId={PriceId}", null)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Position
                    {
                        EntityType.Instance.Position(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Children",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "DisabledExpression",

                                                // Часть про сложная ли позиция будет проверена на сервере
                                                new Tuple<object, object>(@"Ext.getDom(""Id"").value==0 || !Ext.getDom(""IsComposite"").checked", @"Ext.getDom(""Id"").value==0")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region PositionChildren
                    {
                        EntityType.Instance.PositionChildren(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "PositionChildren",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("Name", "Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region DeniedPosition
                    {
                        EntityType.Instance.DeniedPosition(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "DeniedPosition",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("Name", "Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region LocalMessage
                    {
                        EntityType.Instance.LocalMessage(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "LocalMessage",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("Default.gif", "en_ico_lrg_LocalMessage.gif")
                                            },
                                        }
                                },
                                {
                                    "Close",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region AssociatedPosition
                    {
                        EntityType.Instance.AssociatedPosition(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "AssociatedPosition",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("Name", "Id")
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region BranchOfficeOrganizationUnit
                    {
                        EntityType.Instance.BranchOfficeOrganizationUnit(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Actions",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                                {
                                    "SetAsPrimary",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.TextImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                                {
                                    "SetAsPrimaryForRegSales",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "ControlType",
                                                new Tuple<object, object>(ControlType.TextImageButton.ToString(), ControlType.TextButton.ToString())
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region Bill
                    {
                        EntityType.Instance.Bill(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Save",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Create | EntityAccessTypes.Update))
                                            },
                                        }
                                },
                                {
                                    "SaveAndClose",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Create | EntityAccessTypes.Update))
                                            },
                                        }
                                },
                                {
                                    "PrintBillAction",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region AdsTemplatesAdsElementTemplate
                    {
                        EntityType.Instance.AdsTemplatesAdsElementTemplate(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "AdsTemplatesAdsElementTemplate",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("Name", "Id")
                                            },
                                        }
                                },
                                {
                                    "Close",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region WithdrawalInfo
                    {
                        EntityType.Instance.WithdrawalInfo(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "Actions",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnInactive",
                                                new Tuple<object, object>(true, false)
                                            },
                                        }
                                },
                                {
                                    "DownloadResults",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "LockOnNew",
                                                new Tuple<object, object>(false, true)
                                            },
                                        }
                                },
                            }
                    },

                    #endregion

                    #region UserProfile
                    {
                        EntityType.Instance.UserProfile(),
                        new Dictionary<string, IDictionary<string, Tuple<object, object>>>
                            {
                                {
                                    "UserProfile",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "Icon",
                                                new Tuple<object, object>("en_ico_lrg_UserAccount.gif", "en_ico_lrg_User.gif")
                                            },
                                            {
                                                "EntityMainAttribute",
                                                new Tuple<object, object>("TimeZoneInfoId", "Id")
                                            },
                                        }
                                },
                                {
                                    "Save",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Create | EntityAccessTypes.Update))
                                            },
                                        }
                                },
                                {
                                    "SaveAndClose",
                                    new Dictionary<string, Tuple<object, object>>
                                        {
                                            {
                                                "SecurityPrivelege",
                                                new Tuple<object, object>((int)EntityAccessTypes.None, (int)(EntityAccessTypes.Create | EntityAccessTypes.Update))
                                            },
                                        }
                                },
                            }
                    },

                    #endregion
                };
    }
}