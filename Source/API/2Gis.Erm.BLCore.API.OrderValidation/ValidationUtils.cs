using System.Text;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public static class ValidationUtils
    {
        public static bool IsMassive(this ValidationType validationType)
        {
            return validationType == ValidationType.PreReleaseBeta ||
                   validationType == ValidationType.PreReleaseFinal ||
                   validationType == ValidationType.ManualReport ||
                   validationType == ValidationType.ManualReportWithAccountsCheck;
        }

        public static string AsDescription(this ValidateOrdersRequest validateOrdersRequest)
        {
            var description = new StringBuilder();

            description.AppendFormat("ValidationType: {0}. Token: {1}. ", validateOrdersRequest.Type, validateOrdersRequest.Token);
            if (validateOrdersRequest.Type.IsMassive())
            {
                description.AppendFormat(
                    "OrganizationUnitId: {0}. Period: {1}. Owner: {2}. IncludeOwnerDescendants: {3}", 
                    validateOrdersRequest.OrganizationUnitId, 
                    validateOrdersRequest.Period, 
                    validateOrdersRequest.OwnerId.HasValue ? validateOrdersRequest.OwnerId.Value.ToString() : "Not used",
                    validateOrdersRequest.OwnerId.HasValue ? "IncludeOwnerDescendants: " + validateOrdersRequest.IncludeOwnerDescendants : string.Empty);
            }
            else
            {
                description.AppendFormat(
                    "OrderId: {0}. CurrentState: {1}. TargetState: {2}",
                    validateOrdersRequest.OrderId,
                    validateOrdersRequest.CurrentOrderState,
                    validateOrdersRequest.NewOrderState);
            }

            return description.ToString();
        }
    }
}