using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied
{
    public class OrderStateStorage
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public Dictionary<long, int> PositionsIndexes { get; set; }

        public object GetDescription(bool rich)
        {
            return rich ? string.Format("<Order:{0}:{1}>", Number, Id) : Number;
        } 
    }
}