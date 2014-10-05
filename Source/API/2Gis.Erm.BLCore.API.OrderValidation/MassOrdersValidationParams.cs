using System.Text;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class MassOrdersValidationParams : ValidationParams
    {
        public MassOrdersValidationParams(ValidationType type) 
            : base(type, new[] { ValidationType.PreReleaseBeta, ValidationType.PreReleaseFinal, ValidationType.ManualReport, ValidationType.ManualReportWithAccountsCheck })
        {
        }

        /// <summary>
        /// ���������� ��������� ��� �������� �� ��������
        /// </summary>
        public long? OwnerId { get; set; }

        /// <summary>
        /// ������� ���������� ��������
        /// </summary>
        public bool IncludeOwnerDescendants { get; set; }

        /// <summary>
        /// ���������� ��������� ��� �������� ������� ������ �� �������
        /// </summary>
        public long OrganizationUnitId { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                .AppendFormat("ValidationType: {0}. Token: {1}. ", Type, Token)
                .AppendFormat(
                              "OrganizationUnitId: {0}. {1}. Owner: {2}. IncludeOwnerDescendants: {3}",
                              OrganizationUnitId,
                              Period,
                              OwnerId.HasValue ? OwnerId.Value.ToString() : "Not used",
                              IncludeOwnerDescendants)
                .ToString();
        }
    }
}