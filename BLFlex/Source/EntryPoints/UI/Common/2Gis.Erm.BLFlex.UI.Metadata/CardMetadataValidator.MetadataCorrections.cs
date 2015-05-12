using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Metadata
{
    public sealed partial class CardMetadataValidator
    {
        private readonly IDictionary<EntityName, IDictionary<string, IDictionary<string, Tuple<object, object>>>> _cardMetadataCorrections =
            new Dictionary<EntityName, IDictionary<string, IDictionary<string, Tuple<object, object>>>>
                {
                    #region Order
                    {
                        EntityName.Order,
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
                        EntityName.LegalPerson,
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
                        EntityName.Bargain,
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
                        EntityName.Bank,
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
                        EntityName.Contact,
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
                        EntityName.Deal,
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
                        EntityName.Client,
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
                        EntityName.OrderProcessingRequest,
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
                        EntityName.Firm,
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
                                                new Tuple<object, object>("firmId={Id}", null)
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
                        EntityName.FirmAddress,
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
                        EntityName.AdvertisementElementStatus,
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
                        EntityName.AdvertisementTemplate,
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
                        EntityName.User,
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
                        EntityName.Territory,
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
                        EntityName.Theme,
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
                        EntityName.ReleaseInfo,
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
                        EntityName.Project,
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
                        EntityName.Currency,
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
                        EntityName.CurrencyRate,
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
                        EntityName.Lock,
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
                        EntityName.LockDetail,
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
                        EntityName.Role,
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
                        EntityName.RolePrivilege,
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
                        EntityName.OrderPosition,
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
                        EntityName.PricePosition,
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
                        EntityName.Position,
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
                        EntityName.PositionChildren,
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
                        EntityName.DeniedPosition,
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
                        EntityName.LocalMessage,
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
                        EntityName.AssociatedPosition,
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
                        EntityName.BranchOfficeOrganizationUnit,
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
                        EntityName.Bill,
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
                        EntityName.AdsTemplatesAdsElementTemplate,
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
                        EntityName.WithdrawalInfo,
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
                        EntityName.UserProfile,
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