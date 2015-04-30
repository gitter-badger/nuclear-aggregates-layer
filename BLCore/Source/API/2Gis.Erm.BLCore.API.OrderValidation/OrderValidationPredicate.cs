using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public sealed class OrderValidationPredicate
    {
        private Expression<Func<Order, bool>> _combined;

        public OrderValidationPredicate(Expression<Func<Order, bool>> generalPart,
                                        Expression<Func<Order, bool>> orgUnitPart,
                                        Expression<Func<Order, bool>> validationGroupPart,
                                        Expression<Func<Order, bool>> invalidOrdersPart)
        {
            GeneralPart = generalPart;
            OrgUnitPart = orgUnitPart;
            ValidationGroupPart = validationGroupPart;
            InvalidOrdersPart = invalidOrdersPart;
        }

        public OrderValidationPredicate(Expression<Func<Order, bool>> generalPart,
                                        Expression<Func<Order, bool>> orgUnitPart,
                                        Expression<Func<Order, bool>> validationGroupPart)
        {
            GeneralPart = generalPart;
            OrgUnitPart = orgUnitPart;
            ValidationGroupPart = validationGroupPart;
        }

        public Expression<Func<Order, bool>> GeneralPart { get; private set; }
        public Expression<Func<Order, bool>> OrgUnitPart { get; private set; }
        public Expression<Func<Order, bool>> ValidationGroupPart { get; private set; }
        public Expression<Func<Order, bool>> InvalidOrdersPart { get; private set; }

        public Expression<Func<Order, bool>> GetCombinedPredicate()
        {
            if (_combined != null)
            {
                return _combined;
            }

            _combined = GeneralPart != null ? _combined = GeneralPart : x => false;
            
            if (OrgUnitPart != null)
            {
                _combined = _combined.And(OrgUnitPart);
            }

            if (ValidationGroupPart != null)
            {
                _combined = _combined.And(ValidationGroupPart);
            }

            if (InvalidOrdersPart != null)
            {
                _combined = _combined.And(InvalidOrdersPart);
            }

            return _combined;
        }
    }
}
