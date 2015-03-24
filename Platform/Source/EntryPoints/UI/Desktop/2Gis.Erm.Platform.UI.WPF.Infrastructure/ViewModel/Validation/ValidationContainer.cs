using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;

using FluentValidation;
using FluentValidation.Results;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public class ValidationContainer : IValidatorsContainer
    {
        private readonly IEnumerable<IValidator> _validators;
        private readonly IDictionary<string, ICollection<string>> _errors = new Dictionary<string, ICollection<string>>();
        private IEnumerable<string> _changedErrors = Enumerable.Empty<string>();

        public ValidationContainer(IEnumerable<IValidator> validators)
        {
            _validators = validators; 
        }

        bool IViewModelAspect.Enabled
        {
            get
            {
                return true;
            }
        }

        public IDictionary<string, ICollection<string>> Errors
        {
            get { return _errors; }
        }

        public IEnumerable<string> ChangedErrors
        {
            get { return _changedErrors; }
        }

        public ValidationResult Validate(IViewModel model)
        {
            var viewModelFailures = _validators.SelectMany(v => v.Validate(model).Errors).Distinct(ValidationFailureComparer.Instance).ToArray();
            
            var localizableModel = model as ILocalizedViewModel;
            if (localizableModel == null)
            {
                return new ValidationResult(viewModelFailures);
            }

            Func<string, string> localizeFunc = x => localizableModel.Localizer.GetString(x);

            viewModelFailures = viewModelFailures.Select(failure =>
            {
                // Описание ошибки передаем в поле CustomState 
                var error = failure.CustomState as ErrorDescription;
                if (error == null)
                {
                    return failure;
                }

                // Обрабатываются сообщеня вида "Поле {0} должно быть строкой максимальной длины {1}"
                // Первый аргумент - имя валидируемого свойства (PropertyNameResourceKey), остальные приходят в свойстве FormatArgs
                var args = new object[] { localizeFunc(error.PropertyNameResourceKey) }.Concat(error.FormatArgs).ToArray();

                return new ValidationFailure(failure.PropertyName,
                    string.Format(localizeFunc(error.MessageResourceKey), args),
                    failure.AttemptedValue);
            }).ToArray();


            var errorsProperties = Errors.Select(x => x.Key);
            var failureProperties = viewModelFailures.Select(x => x.PropertyName);

            _changedErrors = errorsProperties.Except(failureProperties).Concat(failureProperties.Except(errorsProperties)).ToArray();

            Errors.Clear(); 
            foreach (var failure in viewModelFailures)
            {
                if (Errors.ContainsKey(failure.PropertyName))
                {
                    Errors[failure.PropertyName].Add(failure.ErrorMessage);
                }
                else
                {
                    Errors.Add(failure.PropertyName, new List<string> { failure.ErrorMessage });
                }
            }

            return new ValidationResult(viewModelFailures);
        }

        #region Comparer 

        private class ValidationFailureComparer : IEqualityComparer<ValidationFailure>
        {
            private static readonly ValidationFailureComparer _instance =
                new ValidationFailureComparer();

            public static ValidationFailureComparer Instance
            {
                get { return _instance; }
            }

            public bool Equals(ValidationFailure x, ValidationFailure y)
            {
                return string.Equals(x.PropertyName, y.PropertyName) && string.Equals(x.ErrorMessage, y.ErrorMessage);
            }

            public int GetHashCode(ValidationFailure obj)
            {
                return obj.PropertyName.GetHashCode() ^ obj.ErrorMessage.GetHashCode();
            }
        }

        #endregion
    }
}
