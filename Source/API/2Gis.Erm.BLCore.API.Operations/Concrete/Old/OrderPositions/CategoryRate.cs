using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public class CategoryRate
    {
        public static readonly CategoryRate NeedsCalculation = new CategoryRate(null);
        public static readonly CategoryRate Default = new CategoryRate(1M);

        private readonly decimal? _rate;

        private CategoryRate(decimal? rate)
        {
            _rate = rate;
        }

        public bool MustBeCalculated
        {
            get { return !_rate.HasValue; }
        }

        public decimal Value
        {
            get
            {
                if (_rate == null)
                {
                    throw new InvalidOperationException("Rate needs calculation");
                }

                return _rate.Value;
            }
        }

        public static CategoryRate Known(decimal rate)
        {
            return new CategoryRate(rate);
        }
    }
}
