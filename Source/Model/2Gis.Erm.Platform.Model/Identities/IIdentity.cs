using System;

namespace DoubleGis.Erm.Platform.Model.Identities
{
    public interface IIdentity : IEquatable<IIdentity>
    {
        /// <summary>
        /// Уникальный идентификатор операции - уникально идентифицирует операцию
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Описание операции - назначение, в чем суть и т.п.
        /// </summary>
        string Description { get; }
    }
}