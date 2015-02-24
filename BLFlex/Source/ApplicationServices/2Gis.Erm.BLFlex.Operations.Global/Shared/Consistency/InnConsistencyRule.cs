using System;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class InnConsistencyRule<TArg, TValidator> : IConsistencyRule<TArg>
        where TArg : class
        where TValidator : ICheckInnService, new()
    {
        private readonly Func<TArg, string> _field;
        private readonly TValidator _validator;

        public InnConsistencyRule(Func<TArg, string> field)
        {
            _field = field;
            _validator = new TValidator();
        }

        void IConsistencyRule.Apply(object obj)
        {
            Apply((TArg)obj);
        }

        public void Apply(TArg obj)
        {
            var value = _field.Invoke(obj);
            string message;
            if (_validator.TryGetErrorMessage(value, out message))
            {
                throw new NotificationException(message);
            }
        }
    }
}
