using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    // TODO: запилить нормальные метаданные
    public static class RelationalMetadata
    {
        private static readonly Dictionary<Tuple<EntityName, EntityName>, string> RelationalMap = new Dictionary<Tuple<EntityName, EntityName>, string>
        {
            { Tuple.Create(EntityName.Account, EntityName.AccountDetail), "AccountId={0}" },
            { Tuple.Create(EntityName.Account, EntityName.Lock), "AccountId={0}" },
            { Tuple.Create(EntityName.Account, EntityName.Limit), "AccountId={0}" },
            { Tuple.Create(EntityName.Account, EntityName.Order), "AccountId={0}" },

            { Tuple.Create(EntityName.AdvertisementTemplate, EntityName.AdsTemplatesAdsElementTemplate), "AdsTemplateId={0}" },

            { Tuple.Create(EntityName.AdvertisementElementTemplate, EntityName.AdsTemplatesAdsElementTemplate), "AdsElementTemplateId={0}" },

            { Tuple.Create(EntityName.AssociatedPositionsGroup, EntityName.AssociatedPosition), "AssociatedPositionsGroupId={0}" },

            { Tuple.Create(EntityName.Bargain, EntityName.BargainFile), "BargainId={0}" },

            { Tuple.Create(EntityName.BranchOffice, EntityName.BranchOfficeOrganizationUnit), "BranchOfficeId={0}" },

            { Tuple.Create(EntityName.BranchOfficeOrganizationUnit, EntityName.PrintFormTemplate), "BranchOfficeOrganizationUnitId={0}" },

            { Tuple.Create(EntityName.Category, EntityName.Category), "ParentId={0}" },
            { Tuple.Create(EntityName.Category, EntityName.CategoryOrganizationUnit), "CategoryId={0}" },

            { Tuple.Create(EntityName.Client, EntityName.Firm), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.Contact), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.Deal), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.LegalPerson), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.Order), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.Limit), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.Bargain), "ClientId={0}" },
            { Tuple.Create(EntityName.Client, EntityName.ActivityInstance), "ClientId={0}" },

            { Tuple.Create(EntityName.Contact, EntityName.ActivityInstance), "ContactId={0}" },

            { Tuple.Create(EntityName.Currency, EntityName.CurrencyRate), "CurrencyId={0}" },
            { Tuple.Create(EntityName.Currency, EntityName.Country), "CurrencyId={0}" },

            { Tuple.Create(EntityName.Deal, EntityName.Order), "DealId={0}" },
            { Tuple.Create(EntityName.Deal, EntityName.ActivityInstance), "DealId={0}" },

            { Tuple.Create(EntityName.Firm, EntityName.FirmAddress), "FirmId={0}" },
            { Tuple.Create(EntityName.Firm, EntityName.CategoryFirmAddress), "FirmId={0}" },
            { Tuple.Create(EntityName.Firm, EntityName.Advertisement), "FirmId={0}" },
            { Tuple.Create(EntityName.Firm, EntityName.Order), "FirmId={0}" },
            { Tuple.Create(EntityName.Firm, EntityName.ActivityInstance), "FirmId={0}" },

            { Tuple.Create(EntityName.FirmAddress, EntityName.FirmContact), "FirmAddressId={0}||(FirmAddressId==null&&CardId!=null)" },
            { Tuple.Create(EntityName.FirmAddress, EntityName.CategoryFirmAddress), "FirmAddressId={0}" },
                
            { Tuple.Create(EntityName.LegalPerson, EntityName.Account), "LegalPersonId={0}" },
            { Tuple.Create(EntityName.LegalPerson, EntityName.Limit), "LegalPersonId={0}" },
            { Tuple.Create(EntityName.LegalPerson, EntityName.Bargain), "CustomerLegalPersonId={0}" },
            { Tuple.Create(EntityName.LegalPerson, EntityName.Order), "LegalPersonId={0}" },

            { Tuple.Create(EntityName.Lock, EntityName.LockDetail), "LockId={0}" },

            { Tuple.Create(EntityName.Order, EntityName.Bill), "OrderId={0}" },
            { Tuple.Create(EntityName.Order, EntityName.Lock), "OrderId={0}" },
            { Tuple.Create(EntityName.Order, EntityName.OrderFile), "OrderId={0}" },
            { Tuple.Create(EntityName.Order, EntityName.OrderProcessingRequest), "RenewedOrderId={0}" },

            { Tuple.Create(EntityName.OrganizationUnit, EntityName.BranchOfficeOrganizationUnit), "OrganizationUnitId={0}" },
            { Tuple.Create(EntityName.OrganizationUnit, EntityName.Price), "OrganizationUnitId={0}" },
            { Tuple.Create(EntityName.OrganizationUnit, EntityName.Project), "OrganizationUnitId={0}" },

            { Tuple.Create(EntityName.Position, EntityName.PositionChildren), "MasterPositionId={0}" },

            { Tuple.Create(EntityName.PricePosition, EntityName.AssociatedPositionsGroup), "PricePositionId={0}" },

            { Tuple.Create(EntityName.Price, EntityName.PricePosition), "PriceId={0}" },

            { Tuple.Create(EntityName.Theme, EntityName.ThemeOrganizationUnit), "ThemeId={0}" },
            { Tuple.Create(EntityName.Theme, EntityName.ThemeCategory), "ThemeId={0}" },

            { Tuple.Create(EntityName.Territory, EntityName.Firm), "TerritoryId={0}" },

            { Tuple.Create(EntityName.User, EntityName.UserRole), "UserId={0}" },
            { Tuple.Create(EntityName.User, EntityName.UserTerritory), "UserId={0}" },
            { Tuple.Create(EntityName.User, EntityName.UserOrganizationUnit), "UserId={0}" },

            //------------------------
            { Tuple.Create(EntityName.Advertisement, EntityName.AdvertisementElement), "AdvertisementId={0}" },
            { Tuple.Create(EntityName.LegalPerson, EntityName.LegalPersonProfile), "LegalPersonId={0}" },
            { Tuple.Create(EntityName.Order, EntityName.OrderPosition), "OrderId={0}" },
            { Tuple.Create(EntityName.OrganizationUnit, EntityName.UserOrganizationUnit), "OrganizationUnitId={0}" },
        };

        public static bool TryGetFilterExpressionFromRelationalMap(EntityName parentEntityName, EntityName entityName, out string filterExpression)
        {
            return RelationalMap.TryGetValue(Tuple.Create(parentEntityName, entityName), out filterExpression);
        }
    }

}