using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Properties;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;
using DoubleGis.Platform.UI.WPF.Infrastructure.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util.FocusableBinding;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public sealed class DynamicCardViewModel : 
        DynamicViewModel,
        ICardViewModel<ICardViewModelIdentity>,
        ITitledViewModel<DynamicCardViewModel>,
        IDynamicPropertiesViewModel<DynamicCardViewModel>, 
        ILocalizedViewModel<DynamicCardViewModel>,
        IValidatableViewModel<DynamicCardViewModel>,
        IActionsBoundViewModel<DynamicCardViewModel>,
        IContextualNavigationViewModel<DynamicCardViewModel>,
        IMessageSource<NotificationMessage>,
        IViewModelWithFeedback,
        IFocusMover,
        INotifyDataErrorInfo 
    {
        private readonly ICardViewModelIdentity _viewModelIdentity;
        private readonly ITitleProvider _titleProvider;
        private readonly ILocalizer _localizer;
        private readonly IValidatorsContainer _validatorsContainer;
        private readonly IActionsContainer _actionsContainer;
        private readonly IContextualNavigationConfig _contextualNavigationConfig;
        private readonly IMessageSink _messageSink;
        private object _referencedItemContext;

        public DynamicCardViewModel(
            ICardViewModelIdentity viewModelIdentity,
            ITitleProvider titleProvider,
            IPropertiesContainer propertiesContainer, 
            ILocalizer localizer, 
            IValidatorsContainer validatorsContainer, 
            IActionsContainer actionsContainer, 
            IContextualNavigationConfig contextualNavigationConfig,
            IMessageSink messageSink)
        {
            _viewModelIdentity = viewModelIdentity;
            _titleProvider = titleProvider;
            _localizer = localizer;
            _validatorsContainer = validatorsContainer;
            _actionsContainer = actionsContainer;
            _contextualNavigationConfig = contextualNavigationConfig;
            _messageSink = messageSink;
            
            AttachProperties(propertiesContainer);
        }

        public event EventHandler<MoveFocusEventArgs> MoveFocus;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get { return _validatorsContainer.Errors.Any(); }
        }

        public override IViewModelIdentity Identity
        {
            get { return _viewModelIdentity; }
        }
        
        public ICardViewModelIdentity ConcreteIdentity
        {
            get
            {
                return _viewModelIdentity;
            }
        }

        public string DocumentName
        {
            get
            {
                return _titleProvider.Title;
            }
        }

        Guid IDocument.Id
        {
            get
            {
                return _viewModelIdentity.Id;
            }
        }

        public DynamicResourceDictionary Localizer
        {
            get
            {
                return _localizer.Localized;
            }
        }

        public string Title
        {
            get
            {
                return _titleProvider.Title;
            }
        }

        IReadOnlyDictionary<string, INavigationItem> IContextualNavigationViewModel.Parts
        {
            get
            {
                return _contextualNavigationConfig.Parts;
            }
        }

        object IContextualNavigationViewModel.ReferencedItemContext
        {
            get
            {
                return _referencedItemContext;
            }

            set
            {
                if (_referencedItemContext != value)
                {
                    _referencedItemContext = value;
                    OnPropertyChanged();
                }
            }
        }

        DataTemplateSelector IContextualNavigationViewModel.ReferencedItemViewSelector
        {
            get
            {
                return _contextualNavigationConfig.ReferecedItemsViewsSelector;
            }
        }

        INavigationItem[] IContextualNavigationViewModel.Items
        {
            get
            {
                return _contextualNavigationConfig.Items;
            }
        }

        ITitleProvider IContextualNavigationViewModel.Title
        {
            get
            {
                return _titleProvider;
            }
        }

        IEnumerable<INavigationItem> IActionsBoundViewModel.Actions
        {
            get
            {
                return _actionsContainer.Enabled && _actionsContainer.Actions != null
                    ? _actionsContainer.Actions
                    : Enumerable.Empty<INavigationItem>();
            }
        }
        
        public void ActivateNotificationSource(INotification notification)
        {
            var contextualNotification = notification as IContextualNotification;
            if (contextualNotification == null)
            {
                return;
            }

            var handler = MoveFocus;
            if (handler != null)
            {
                handler(this, new MoveFocusEventArgs(contextualNotification.PropertyName));
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            ICollection<string> errors;
            return _validatorsContainer.Errors.TryGetValue(propertyName, out errors) ? string.Join(Environment.NewLine, errors) : null;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            Validate();
        }

        private void Validate()
        {
            var validationResult = _validatorsContainer.Validate(this);
            if (validationResult.IsValid)
            {
                return;
            }

            var notifications = validationResult.Errors.Select(x =>
                                                               (INotification)
                                                               new ContextualNotification(_viewModelIdentity.Id, NotificationLevel.Error, x.ErrorMessage)
                                                                   {
                                                                       PropertyName = x.PropertyName
                                                                   }).ToArray();

            foreach (var changedError in _validatorsContainer.ChangedErrors)
            {
                FireErrorsChanged(new DataErrorsChangedEventArgs(changedError));
            }

            _messageSink.Post(new NotificationMessage(notifications));
        }

        private void AttachProperties(IPropertiesContainer propertiesContainer)
        {
            var configurator = (IDynamicPropertiesContainerConfigurator)this;
            foreach (var propertyDescriptor in propertiesContainer.Properties)
            {
                var onlyMetadataProperty = propertyDescriptor as MetadataOnlyViewModelProperty;
                if (onlyMetadataProperty != null)
                {
                    configurator.AddProperty(onlyMetadataProperty.Name, onlyMetadataProperty.PropertyType);
                    continue;
                }

                var metadataAndValueProperty = propertyDescriptor as MetadataAndValueViewModelProperty;
                if (metadataAndValueProperty != null)
                {
                    configurator.AddProperty(metadataAndValueProperty.Name, metadataAndValueProperty.PropertyType, metadataAndValueProperty.Value, Enumerable.Empty<Attribute>());
                    continue;
                }

                throw new NotSupportedException("Unsupported property type " + propertyDescriptor.GetType());
            }

            configurator.Lock();
        }

        private void FireErrorsChanged(DataErrorsChangedEventArgs e)
        {
            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}