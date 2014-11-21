using System.Windows.Input;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.MVVM
{
    public interface IDelegateCommand : ICommand
    {
        /// <summary>
        /// Raises <see cref="DelegateCommand{T}.CanExecuteChanged"/> on the UI thread so every command invoker
        /// can requery to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="DelegateCommand{T}.CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        void RaiseCanExecuteChanged();
    }
}