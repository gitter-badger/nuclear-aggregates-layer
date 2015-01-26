using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Dialogs;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations
{
    // TODO {all, 22.07.2013}: пока конфигуратор операции - это в любом случае диалог, вопрос - хорошо ли это
    public interface IOperationConfiguratorViewModel : IDialogViewModel
    {
        IOperationIdentity ProcessingOperation { get; }
        OperationProcessingStatus Status { get; }
        OperationItemViewModel[] Results { get; }
        
        string OperationResultMessage { get; }
        string OperationTopBarTitle { get; }
        ITitleProvider OperationConfirmMessage { get; }
        string OperationOperationTopBarMessage { get; }

        bool IsGroupMode { get; }
        double Progress { get; }
        ICommonOperationParameter GetCommonParameter();
        IEnumerable<IOperationParameter> GetParameters();
        void UpdateOperationProgress(IOperationResult[] operationResults);
        void Finished(IOperationResult[] finalResults);

        DataTemplateSelector ConfiguratorViewSelector { get; }
    }

    public interface IOperationConfiguratorViewModel<TOperationIdentity, TCommonOperationParameter, TOperationParameter> : IOperationConfiguratorViewModel
        where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        where TCommonOperationParameter : class, ICommonOperationParameter
        where TOperationParameter : class, IOperationParameter
    {
        TCommonOperationParameter CommonParameter { get; }
        IEnumerable<TOperationParameter> Parameters { get; }
    }
}
