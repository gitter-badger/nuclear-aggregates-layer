using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Infrastructure;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public class LookupSearchHandler : UseCaseSyncMessageHandlerBase<LookupSearchMessage>
    {
        private readonly IListNonGenericEntityService _listService;

        public LookupSearchHandler(IListNonGenericEntityService listService)
        {
            _listService = listService;
        }

        protected override IMessageProcessingResult ConcreteHandle(LookupSearchMessage message, IUseCase useCase)
        {
            var settings = message.Property;

            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }

            var rootViewModel = currentElement.Context as IViewModel;
            if (rootViewModel == null)
            {
                return null;
            }

            ICardViewModel hostingViewModel;
            if (!rootViewModel.TryGetElementById(message.SourceId, out hostingViewModel) || hostingViewModel == null)
            {
                return null;
            }

            // TODO пока жесткая привязка к IDynamicPropertiesContainer, по хорошему нужно уметь работать через универсальный интерфейс и со статической, смешанной, чисто динамической 
            var propertiesContainer = hostingViewModel as IDynamicPropertiesContainer;
            if (propertiesContainer == null)
            {
                return null;
            }

            var lookupViewModel = propertiesContainer.GetDynamicPropertyValue(message.Property.TargetPropertyMetadata.Name) as LookupViewModel;
            if (lookupViewModel == null)
            {
                return null;
            }

            ListResult result;

            try
            {
                result = _listService.List(settings.LookupEntity,
                                               new SearchListModel 
                                               {
                                                   ParentEntityName = settings.LookupEntity,
                                                   FilterInput = message.SearchText,
                                                   ExtendedInfo = PrepareExtendedInfo(settings.ExtendedInfo, propertiesContainer),
                                                   Start = 0,
                                                   Limit = 5,
                                               });

            }

            catch (ApiException e)
            {
                MessageBox.Show(e.ApiExceptionDescription.Description, "Can't get list data");
                return new MessageProcessingResult<bool>(true);
            }

            var items = RetrieveItems(result, settings);

            lookupViewModel.Items.Clear();
            foreach (var reference in items)
            {
                lookupViewModel.Items.Add(LookupEntry.FromReference(reference));
            }

            return EmptyResult;
        }

        private static IEnumerable<EntityReference> RetrieveItems(ListResult result, LookupPropertyFeature settings)
        {
            switch (result.ResultType)
            {
                case ListResultType.Dto:
                    return ((IDataListResult)result).Data.Cast<object>().Select(x => new EntityReference(x.GetPropertyValue<object, long?>(settings.KeyAttribute),  
                                                                                                         x.GetPropertyValue<object, string>(settings.ValueAttribute)));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string PrepareExtendedInfo(string expression, IDynamicPropertiesContainer propertiesContainer)
        {
            return expression != null ? Regex.Replace(expression, @"\{([^}]*)\}", match => EvaluateValue(match.Value, propertiesContainer)) : null;
        }

        // Аналог метода в js - LookupField.prepareExtendedInfo
        private string EvaluateValue(string value, IDynamicPropertiesContainer propertiesContainer)
        {
            var propertyPath = value.TrimStart('{').TrimEnd('}');

            var propertyPathParts = propertyPath.Split('.');

            if (propertyPathParts.Length == 1)
            {
                return Convert.ToString(propertiesContainer.GetDynamicPropertyValue(propertyPath));
            }

            // В случае Path вида "Ref.Id", полагаем, что Ref имеет тип EntityReference
            if (propertyPathParts.Length == 2)
            {
                const string ReguiredLastPathSegment = "Id";
                var viewModelPropertyPath = propertyPathParts[0];
                var targetPropertyPath = propertyPathParts[1];
                if (string.CompareOrdinal(targetPropertyPath, ReguiredLastPathSegment) != 0)
                {
                    throw new NotSupportedException(string.Format("Last path part of not flattened property is invalid (!={0}). Full property path: {1}", ReguiredLastPathSegment, propertyPath));
                }

                var viewModelPropertyValue = propertiesContainer.GetDynamicPropertyValue(viewModelPropertyPath);
                if (viewModelPropertyValue == null)
                {
                    return string.Empty;
                }

                var lookupViewModel = viewModelPropertyValue as LookupViewModel;
                if (lookupViewModel == null)
                {
                    throw new NotSupportedException("Not flattened property " + propertyPath + " has unsupported runtime type " + viewModelPropertyValue.GetType());
                }

                if (lookupViewModel.SelectedItem == null)
                {
                    return string.Empty;
                }

                return Convert.ToString(lookupViewModel.SelectedItem.Key);
            }

            return string.Empty;
        }
    }
}