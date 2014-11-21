using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds
{
    public interface IMetadataKindIdentity : IEquatable<IMetadataKindIdentity>
    {
        /// <summary>
        /// Уникальный идентификатор вида метаданных - уникально идентифицирует вид информации описываемой метаданными: агрегаты, сущности, листинги, гриды, карточки, меню и т.п.
        /// Также данный Id фактически является префиксом для всех элементов метаданных данного вида, 
        /// т.е. если вид метаданных имеет Id erm://metadata/Cards, то все элементы это вида метаданных имеют id с префиксом erm://metadata/Cards/Order
        /// </summary>
        Uri Id { get; }

        /// <summary>
        /// Описание вида метаданных - назначение, в чем суть и т.п.
        /// </summary>
        string Description { get; }
    }
}
