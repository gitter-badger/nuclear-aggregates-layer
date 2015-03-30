namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    /// <summary>
    /// Данные необходимые для расчета стоимости простой позиции + идентификатор этой позиции
    /// </summary>
    public class CalcPositionRequest
    {
        /// <summary>
        /// Прайсовая цена позиции.
        /// </summary>
        public decimal PriceCost { get; set; }

        /// <summary>
        /// Коэфициент группы рубрик, который применяется к данной позиции.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Количество экземпляров позици.
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Количество выпусков размешения.
        /// </summary>
        public int ReleaseCount { get; set; }
        
        /// <summary>
        /// НДС, применяемый к стоимости позиции. Указывается в процентах.
        /// </summary>
        public decimal VatRate { get; set; }
        
        /// <summary>
        /// Признак того, нужно ли показывать НДС пользователю. В определенных случаях мы применяем НДС, но от пользователя это скрываем.
        /// </summary>
        public bool ShowVat { get; set; }

        /// <summary>
        /// Скидка для этой позиции в денежном значении.
        /// </summary>
        public decimal DiscountSum { get; set; }

        /// <summary>
        /// Скидка для этой позиции в процентах.
        /// </summary>
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// Признак того, что нужно применять скидку, указанную в процентах.
        /// </summary>
        public bool CalculateDiscountViaPercent { get; set; }

        /// <summary>
        /// Идентификатор номенклатурной позиции. В конечном расчете не используется. Возвращается в ответе.
        /// </summary>
        public long PositionId { get; set; }

        /// <summary>
        /// Идентификатор прайса. В конечном расчете не используется. Возвращается в ответе.
        /// </summary>
        public long PriceId { get; set; } 
    }
}