using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers.Actions.CanExecute
{
    public static class CanExecuteResult
    {
        public static readonly MessageProcessingResult<bool> True = new MessageProcessingResult<bool>(true);
        public static readonly MessageProcessingResult<bool> False = new MessageProcessingResult<bool>(false);
    }
}
