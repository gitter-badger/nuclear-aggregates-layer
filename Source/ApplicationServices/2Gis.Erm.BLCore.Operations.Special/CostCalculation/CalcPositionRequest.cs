using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;

namespace DoubleGis.Erm.BLCore.Operations.Special.CostCalculation
{
    /// <summary>
    /// ������ ����������� ��� ������� ��������� ������� ������� + ������������� ���� �������
    /// </summary>
    public class CalcPositionRequest
    {
        /// <summary>
        /// ��������� ���� �������.
        /// </summary>
        public decimal PriceCost { get; set; }

        /// <summary>
        /// ���������� ������ ������, ������� ����������� � ������ �������.
        /// </summary>
        public CategoryRate Rate { get; set; }

        /// <summary>
        /// ���������� ����������� ������.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// ���������� �������� ����������.
        /// </summary>
        public int ReleaseCount { get; set; }
        
        /// <summary>
        /// ���, ����������� � ��������� �������. ����������� � ���������.
        /// </summary>
        public decimal VatRate { get; set; }
        
        /// <summary>
        /// ������� ����, ����� �� ���������� ��� ������������. � ������������ ������� �� ��������� ���, �� �� ������������ ��� ��������.
        /// </summary>
        public bool ShowVat { get; set; }

        /// <summary>
        /// ������ ��� ���� ������� � �������� ��������.
        /// </summary>
        public decimal DiscountSum { get; set; }

        /// <summary>
        /// ������ ��� ���� ������� � ���������.
        /// </summary>
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// ������� ����, ��� ����� ��������� ������, ��������� � ���������.
        /// </summary>
        public bool CalculateDiscountViaPercent { get; set; }

        /// <summary>
        /// ������������� �������������� �������. � �������� ������� �� ������������. ������������ � ������.
        /// </summary>
        public long PositionId { get; set; }

        /// <summary>
        /// ������������� ������. � �������� ������� �� ������������. ������������ � ������.
        /// </summary>
        public long PriceId { get; set; } 
    }
}