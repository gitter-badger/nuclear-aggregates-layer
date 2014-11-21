using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Common;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(2936, "Наполнение данными таблиц для групп проверок заказов")]
    public class Migration2936 : TransactedMigration
    {
        private const string InsertGroupsStatement = @"INSERT INTO [Billing].[OrderValidationRuleGroups] ([Code]) VALUES ({0})";
        private const string InsertGroupDetailssStatement = @"
INSERT INTO [Billing].[OrderValidationRuleGroupDetails] 
(
	[OrderValidationGroupId]
	,[RuleCode]
)
VALUES 
(
	(SELECT TOP(1) Id FROM [Billing].[OrderValidationRuleGroups] WHERE [Code] = {0}),
	{1}
)";
        protected override void ApplyOverride(IMigrationContext context)
        {
            InsertOrderValidationRuleGroup(context.Connection, 1);  // OrderValidationRuleGroups.Generic
            InsertOrderValidationRuleGroup(context.Connection, 2);  // OrderValidationRuleGroups.AdvertisementMaterialsValidation

            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 1);    // OrderValidationRuleGroups.Generic, OrderValidationRules.BargainOutOfDate,
            InsertOrderValidationRuleGroupDetails(context.Connection, 2, 2);    // OrderValidationRuleGroups.AdvertisementMaterialsValidation, OrderValidationRules.CouponPeriodIsCorrect,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 3);    // OrderValidationRuleGroups.Generic, OrderValidationRules.OrderHasAnAccount,
            InsertOrderValidationRuleGroupDetails(context.Connection, 2, 4);    // OrderValidationRuleGroups.AdvertisementMaterialsValidation, OrderValidationRules.AdvertisementsElementsNotEmpty,
            InsertOrderValidationRuleGroupDetails(context.Connection, 2, 5);    // OrderValidationRuleGroups.AdvertisementMaterialsValidation, OrderValidationRules.AllRequiredAdvertisementsAttached,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 6);    // OrderValidationRuleGroups.Generic, OrderValidationRules.AssociatedAndDeniedPositions,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 7);    // OrderValidationRuleGroups.Generic, OrderValidationRules.BillsSumsAreCorrect,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 8);    // OrderValidationRuleGroups.Generic, OrderValidationRules.CategoriesLinkedToDestOrgUnit,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 9);    // OrderValidationRuleGroups.Generic, OrderValidationRules.DistributionDatesAreCorrect,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 10);   // OrderValidationRuleGroups.Generic, OrderValidationRules.FirmBelongsToOrdersDestOrgUnit,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 11);   // OrderValidationRuleGroups.Generic, OrderValidationRules.FirmsAreValid,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 12);   // OrderValidationRuleGroups.Generic, OrderValidationRules.LinkingObjectsCorrect,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 13);   // OrderValidationRuleGroups.Generic, OrderValidationRules.NoLockExistsForThePeriod,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 14);   // OrderValidationRuleGroups.Generic, OrderValidationRules.OrderHasAtLeastOnePosition,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 15);   // OrderValidationRuleGroups.Generic, OrderValidationRules.OrderPositionsRefereceCurrentPriceList,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 16);   // OrderValidationRuleGroups.Generic, OrderValidationRules.OrdersAndBargainsScansExist,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 17);   // OrderValidationRuleGroups.Generic, OrderValidationRules.NoReleaseExistsForThePeriod,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 18);   // OrderValidationRuleGroups.Generic, OrderValidationRules.RequiredFieldsArentEmpty,
            InsertOrderValidationRuleGroupDetails(context.Connection, 2, 19);   // OrderValidationRuleGroups.AdvertisementMaterialsValidation, OrderValidationRules.RequiredWhiteListedAdvertisementSpecified,
            InsertOrderValidationRuleGroupDetails(context.Connection, 1, 20);   // OrderValidationRuleGroups.Generic, OrderValidationRules.AccountBalanceIsSufficient,
            InsertOrderValidationRuleGroupDetails(context.Connection, 2, 21);   // OrderValidationRuleGroups.AdvertisementMaterialsValidation, OrderValidationRules.SameAdvertisements
        }

        private static void InsertOrderValidationRuleGroup(ServerConnection connection, int groupCode)
        {
            connection.ExecuteNonQuery(string.Format(InsertGroupsStatement, groupCode));
        }

        private static void InsertOrderValidationRuleGroupDetails(ServerConnection connection, int groupCode, int ruleCode)
        {
            connection.ExecuteNonQuery(string.Format(InsertGroupDetailssStatement, groupCode, ruleCode));
        }
    }
}