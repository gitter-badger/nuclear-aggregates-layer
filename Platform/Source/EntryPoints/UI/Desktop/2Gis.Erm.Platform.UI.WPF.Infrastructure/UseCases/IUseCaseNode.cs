using System;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases
{
    public interface IUseCaseNode : IDisposable
    {
        Guid Id { get; }
        IUseCaseNode[] Childs { get; }
        IUseCaseNode Parent { get; }
        bool IsDegenerated { get; }
        bool IsRoot { get; }

        object Context { get; set; }

        void Add(IUseCaseNode child);
        bool Remove(Guid childId);
    }
}