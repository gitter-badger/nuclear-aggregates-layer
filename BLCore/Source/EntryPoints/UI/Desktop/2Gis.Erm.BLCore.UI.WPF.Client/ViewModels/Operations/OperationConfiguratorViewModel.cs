using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Dialogs;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations
{
    public abstract class OperationConfiguratorViewModel<TOperationIdentity, TCommonOperationParameter, TOperationParameter, TOperationResult> : OkCancelDialogViewModelBase,
        IOperationConfiguratorViewModel<TOperationIdentity, TCommonOperationParameter, TOperationParameter> 
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new() 
        where TCommonOperationParameter : class, ICommonOperationParameter 
        where TOperationParameter : class, IOperationParameter
        where TOperationResult : class, IOperationResult
    {
        private readonly IMessageSink _messageSink;
        private readonly IUserInfo _userInfo;
        private readonly IEntityType _entityName;
        private readonly long[] _operationProcessingEntities;
        private readonly TOperationIdentity _operationIdentity = new TOperationIdentity();

        private readonly object _sync = new object();
        private readonly OperationItemViewModel[] _results;
        private readonly bool _isGroupMode;
        
        private readonly ITitleProvider _operationResultsTitle;
        private readonly ITitleProvider _operationConfirmMessage;
        private readonly ITitleProvider _operationResultsMessageFormat;
        private readonly ITitleProvider _operationOperationTopBarMessageFormat;
        private readonly ITitleProvider _operationOperationTopBarTitleFormat;

        private OperationProcessingStatus _status;
        private double _progress;

        protected OperationConfiguratorViewModel(
            IEntityType entityName,
            long[] operationProcessingEntities,
            IMessageSink messageSink,
            ITitleProviderFactory titleProviderFactory,
            IUserInfo userInfo)
            : base(titleProviderFactory)
        {
            _messageSink = messageSink;
            _userInfo = userInfo;
            _entityName = entityName;
            _operationProcessingEntities = operationProcessingEntities;
            _results = operationProcessingEntities.Select(i => new OperationItemViewModel(i)).ToArray();
            _isGroupMode = _results.Length > 1;

            Status = OperationProcessingStatus.NotStarted;
            Progress = 0;

            _operationResultsTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => Resources.Client.Properties.Resources.GroupOperationResultsTitle));
            _operationConfirmMessage = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.GroupOperationConfirm));
            _operationResultsMessageFormat = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => Resources.Client.Properties.Resources.GroupOperationResultsMessage));
            _operationOperationTopBarMessageFormat = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.GroupOperationTopBarMessage2));
            _operationOperationTopBarTitleFormat = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.GroupOperationTopBarTitle));
        }

        public override ITitleProvider Title
        {
            get 
            { 
                return Status == OperationProcessingStatus.Finished 
                ? _operationResultsTitle 
                : new StaticTitleProvider(new StaticTitleDescriptor(OperationTopBarTitle)); 
            }
        }

        public string OperationResultMessage
        {
            get
            {
                int successfullyProcessed;
                lock (_sync)
                {
                    successfullyProcessed = _results.Count(r => r.Result != null && r.Result.Succeeded);
                }

                return string.Format(_operationResultsMessageFormat.Title, successfullyProcessed, _operationProcessingEntities.Length - successfullyProcessed);
            }
        }

        public string OperationTopBarTitle 
        {
            get { return string.Format(_operationOperationTopBarTitleFormat.Title, OperationName.Title, EntityNameString); }
        }

        public ITitleProvider OperationConfirmMessage 
        {
            get { return _operationConfirmMessage; }
        }

        public string OperationOperationTopBarMessage 
        {
            get
            {
                return string.Format(_operationOperationTopBarMessageFormat.Title, _operationProcessingEntities.Length, EntityNameString);
            }
        }

        public abstract DataTemplateSelector ConfiguratorViewSelector { get; }
        public abstract TCommonOperationParameter CommonParameter { get; }
        public abstract IEnumerable<TOperationParameter> Parameters { get; }

        public IOperationIdentity ProcessingOperation 
        {
            get { return _operationIdentity; }
        }

        public OperationProcessingStatus Status
        {
            get 
            { 
                lock (_sync)
                {
                    return _status;
                } 
            }
            private set
            {
                if (_status == value)
                {
                    return;
                }

                lock (_sync)
                {
                    _status = value;
                }

                FireCommandCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        public OperationItemViewModel[] Results 
        {
            get 
            { 
                lock (_sync)
                {
                    return _results;
                }
            }
        }

        public bool IsGroupMode 
        {
            get { return _isGroupMode; }
        }

        public double Progress
        {
            get { return _progress; }
            private set
            {
                lock (_sync)
                {
                    var progress = _progress;
                    if (Math.Abs(progress - value) < double.Epsilon)
                    {
                        return;
                    }

                    _progress = value;
                }
                
                RaisePropertyChanged();
            }
        }

        protected abstract ITitleProvider OperationName { get; }

        protected string EntityNameString
        {
            get { return _entityName.ToStringLocalized(EnumResources.ResourceManager, _userInfo.Culture); }
        }

        protected IEntityType EntityName
        {
            get { return _entityName; }
        }

        protected long[] OperationProcessingEntities
        {
            get { return _operationProcessingEntities; }
        }
        
        public ICommonOperationParameter GetCommonParameter()
        {
            return CommonParameter;
        }

        public IEnumerable<IOperationParameter> GetParameters()
        {
            return Parameters;
        }
        
        public void UpdateOperationProgress(IOperationResult[] operationResults)
        {
            lock (_sync)
            {
                if (_status != OperationProcessingStatus.Processing)
                {
                    return;
                }

                UpdateResults(operationResults);
                _progress = Math.Round(_results.Count(result => result.Result != null) * 100 / (double)_results.Length, 0);
            }

            OnUpdateOperationProgress(operationResults);
            FireCommandCanExecuteChanged();
            RaisePropertyChanged(() => Progress);
            RaisePropertyChanged(() => Results);
        }

        public void Finished(IOperationResult[] finalResults)
        {
            lock (_sync)
            {
                UpdateResults(finalResults);
                _status = OperationProcessingStatus.Finished;
                _progress = 100;
            }

            OnFinished(finalResults);
            FireCommandCanExecuteChanged();
            RaisePropertyChanged(() => OperationResultMessage);
            RaisePropertyChanged(() => Title);
            RaisePropertyChanged(() => Progress);
            RaisePropertyChanged(() => Status);
        }

        protected override void OnOkCommand()
        {
            switch (Status)
            {
                case OperationProcessingStatus.NotStarted:
                {
                    Status = OperationProcessingStatus.Processing;
                    _messageSink.Post(
                        new ExecuteActionMessage(new StrictOperationIdentity(new TOperationIdentity(), EntityName.ToEntitySet()), Identity.Id)
                            {
                                NeedConfirmation = true, 
                                Confirmed = true
                            });
                    break;
                }
                case OperationProcessingStatus.Finished:
                {
                    _messageSink.Post(new CloseMessage(Identity.Id));
                    break;
                }
            }
        }

        protected override void OnCancelCommand()
        {
            if (_status != OperationProcessingStatus.NotStarted)
            {
                return;
            }

            Status = OperationProcessingStatus.Canceled;

            _messageSink.Post(new CloseMessage(Identity.Id));
        }
        
        protected virtual void OnUpdateOperationProgress(IEnumerable<IOperationResult> operationResults)
        {
        }

        protected virtual void OnFinished(IEnumerable<IOperationResult> operationResults)
        {
        }

        protected void FireCommandCanExecuteChanged()
        {
            OkCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
        }

        protected override bool CanExecuteOkCommand()
        {
            var status = Status;
            return ConcreteCanExecuteOkCommand() && (status == OperationProcessingStatus.NotStarted || status == OperationProcessingStatus.Finished);
        }

        protected override bool CanExecuteCancelCommand()
        {
            var status = Status;
            return ConcreteCanExecuteCancelCommand() && status == OperationProcessingStatus.NotStarted;
        }

        protected virtual bool ConcreteCanExecuteOkCommand()
        {
            return true;
        }

        protected virtual bool ConcreteCanExecuteCancelCommand()
        {
            return true;
        }

        private void UpdateResults(IEnumerable<IOperationResult> operationResults)
        {
            foreach (var operationResult in operationResults.OfType<TOperationResult>())
            {
                var targetItem = _results.Single(r => r.ItemId == operationResult.EntityId);
                targetItem.Result = operationResult;
            }
        }
    }
}