using System;
using System.Linq.Expressions;

using FluentValidation;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<TViewModel, TProperty> WithErrorDescription<TViewModel, TProperty>(this IRuleBuilderOptions<TViewModel, TProperty> builder, ErrorDescription description)
        {
            return builder.WithState(_ => description);
        }

        public static IRuleBuilderOptions<TViewModel, TProperty> WithErrorDescription<TViewModel, TProperty>(this IRuleBuilderOptions<TViewModel, TProperty> builder, Expression<Func<string>> messageResourceAccessExpression, Expression<Func<string>> propertyNameResourceAccessExpression, params object[] formatArgs)
        {
            return builder.WithErrorDescription(new ErrorDescription(messageResourceAccessExpression, propertyNameResourceAccessExpression, formatArgs));
        }
    }
}