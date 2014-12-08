using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public class CardMainAttributeFeature<TViewModel> : ICardMainAttributeFeature
        where TViewModel : IViewModelAbstract
    {
        public CardMainAttributeFeature(Expression<Func<TViewModel, object>> propertyNameExpression)
        {
            Property = PropertyDescriptor.Create(propertyNameExpression);
            PropertyFunc = propertyNameExpression.Compile();
        }

        // TODO {y.baranihin, 28.11.2014}: Заменить на стандартный PropertyDescriptor
        public IPropertyDescriptor Property { get; private set; }
        public string PropertyName
        {
            get { return Property.PropertyName; }
        }

        private Func<TViewModel, object> PropertyFunc { get; set; }

        public bool TryExecute(IViewModelAbstract viewModel, out object result)
        {
            result = null;
            if (!(viewModel is TViewModel))
            {
                return false;
            }

            result = PropertyFunc.Invoke((TViewModel)viewModel);
            return true;
        }
    }
}