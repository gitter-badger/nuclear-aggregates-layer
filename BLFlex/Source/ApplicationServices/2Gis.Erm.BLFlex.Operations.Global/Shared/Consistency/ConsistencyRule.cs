using System;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public static class ConsistencyRule
    {
        public static ConsistencyRule<LegalPersonProfile, TField, NotificationException> CreateEnumValuesRestriction<TField>(
            Func<LegalPersonProfile, TField> field,
            string message,
            params TField[] allowedValues)
        {
            return Create(field, value => !allowedValues.Contains(value), () => message);
        }

        public static ConsistencyRule<LegalPersonProfile, TField, NotificationException> CreateFormat<TField>(
            Func<LegalPersonProfile, TField> field,
            Func<TField, bool> check,
            string messageFormat,
            params object[] messageArgs)
        {
            return Create(field, check, () => string.Format(messageFormat, messageArgs));
        }

        public static ConsistencyRule<LegalPersonProfile, string, NotificationException> CreateNonEmptyString(
            Func<LegalPersonProfile, string> field,
            string messageFormat,
            params object[] messageArgs)
        {
            return Create(field, string.IsNullOrWhiteSpace, () => string.Format(messageFormat, messageArgs));
        }

        public static ConsistencyRule<LegalPersonProfile, TField, NotificationException> CreateNonNull<TField>(
            Func<LegalPersonProfile, TField> field,
            string messageFormat,
            params object[] messageArgs)
            where TField : class
        {
            return Create(field, fieldValue => fieldValue == null, () => string.Format(messageFormat, messageArgs));
        }

        public static ConsistencyRule<LegalPersonProfile, TField?, NotificationException> CreateNonNull<TField>(
            Func<LegalPersonProfile, TField?> field,
            string messageFormat,
            params object[] messageArgs)
            where TField : struct 
        {
            return Create(field, fieldValue => fieldValue == null, () => string.Format(messageFormat, messageArgs));
        }

        private static ConsistencyRule<LegalPersonProfile, TField, NotificationException> Create<TField>(
            Func<LegalPersonProfile, TField> field, 
            Func<TField, bool> check, 
            Func<string> message)
        {
            Func<NotificationException> exception = () => new NotificationException(message.Invoke());
            return new ConsistencyRule<LegalPersonProfile, TField, NotificationException>(field, check, exception);
        }
    }

    public sealed class ConsistencyRule<TArg, TField, TException> : IConsistencyRule<TArg>
        where TArg : class
        where TException : Exception
    {
        private readonly Func<TArg, TField> _field;
        private readonly Func<TField, bool> _check;
        private readonly Func<TException> _exception;

        public ConsistencyRule(Func<TArg, TField> field, Func<TField, bool> check, Func<TException> exception)
        {
            _field = field;
            _check = check;
            _exception = exception;
        }

        void IConsistencyRule.Apply(object obj)
        {
            Apply((TArg)obj);
        }

        public void Apply(TArg obj)
        {
            var value = _field.Invoke(obj);
            var errorDetected = _check.Invoke(value);

            if (errorDetected)
            {
                var exception = _exception.Invoke();
                throw exception;
            }
        }
    }
}
