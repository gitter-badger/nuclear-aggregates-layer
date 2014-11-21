namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation
{
   // public abstract class ValidatedViewModelBase : ViewModelBaseEx, IValidatableDocument, IActionsContainerDocument, INotifyDataErrorInfo, INotifyPropertyChanged, IViewModel
   // {
   //     private readonly ActionContainer _actionContainer;
   //     private readonly ValidationContainer _validationContainer;

   //     protected ValidatedViewModelBase(ValidationContainer validationContainer, ActionContainer actionContainer)
   //     {
   //         _actionContainer = actionContainer;
   //         _validationContainer = validationContainer;

   //         _actionContainer.ErrorsChanged += (sender, args) => RaiseValidated();

   //         _validationContainer.ErrorsChanged += (sender, args) =>
   //             {
   //                 var handler = ErrorsChanged;
   //                 if (handler != null)
   //                 {
   //                     handler.Invoke(this, args);
   //                 }
   //             };

   //         ErrorsChanged += (sender, args) => Debug.WriteLine("ErrorsChanged for " + args.PropertyName);
   //     }

   //     public new event EventHandler<DocumentValidatedEventArgs> Validated;
   //     public new event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
   //     public new event PropertyChangedEventHandler PropertyChanged;

   //     public abstract Guid DocumentId { get; }

   //     public abstract string DocumentName { get; }

   //     public IEnumerable<IToolbarAction> Actions
   //     {
   //         get { return _actionContainer.Actions; }
   //         set { throw new InvalidOperationException(); }
   //     }

   //     public new bool HasErrors
   //     {
   //         get
   //         {
   //             var result = _validationContainer.HasErrors || _actionContainer.ActionMessages.Any();
   //             Debug.WriteLine("HasErrors: " + result);
   //             return result;
   //         }
   //     }

   //     public IEnumerable GetErrors(string propertyName)
   //     {
   //         var result = new List<object>();
   //         result.AddRange(_validationContainer.GetErrors(propertyName).OfType<object>());
   //         ////result.AddRange(_actionContainer.GetErrors(propertyName).OfType<object>()); 

   //         Debug.WriteLine("GetErrors for " + propertyName + ": " + result.Count);
   //         return result;
   //     }

   //     public virtual void ActivateNotificationSource(INotificationsListEntry entry)
   //     {
   //     }

   //     protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
   //     {
   //         base.OnPropertyChanged(propertyName);

   //         _validationContainer.Validate(this);
   //         RaiseValidated();
   //     }

   //     private void RaiseValidated()
   //     {
   //         var errors = new List<ValidationFailure>();
   //         errors.AddRange(_validationContainer.ValidationFailures);
   //         errors.AddRange(_actionContainer.ActionMessages.Select(s => new ValidationFailure(null, s)));
   //         RaiseValidated(errors);
   //     }

   //     private void RaiseValidated(IEnumerable<ValidationFailure> validationFailures)
   //     {
   //         var hander = Validated;
   //         if (hander != null)
   //         {
   //             hander(this, new DocumentValidatedEventArgs
   //                 {
   //                     Notifications = validationFailures
   //                              .Select(x => new NotificationsListEntry
   //                                  {
   //                                      NotificationType = NotificationType.Contextual,
   //                                      NotificationLevel = NotificationLevel.Error,
   //                                      Description = x.ErrorMessage,
   //                                      PropertyName = x.PropertyName
   //                                  })
   //                 });
   //         }
   //     }
   //}
}