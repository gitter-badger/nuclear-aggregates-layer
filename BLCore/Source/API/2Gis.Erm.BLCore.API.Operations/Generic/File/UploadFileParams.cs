using System.IO;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    /// <summary>
    /// Параметры с указанием типа сущности нужны чтобы аггрегирующие репозитории 
    /// могли реализовывать несколько методов для загрузки файлов для различных сущностей.
    /// </summary>
    public sealed class UploadFileParams<TEntity> : UploadFileParams where TEntity : class, IEntityKey
    {
        public UploadFileParams(UploadFileParams parameters)
        {
            EntityId = parameters.EntityId;
            FileId = parameters.FileId;
            FileName = parameters.FileName;
            ContentType = parameters.ContentType;
            ContentLength = parameters.ContentLength;
            Content = parameters.Content;
        }
    }

    public class UploadFileParams 
    {
        public long EntityId { get; set; }
        public long FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        public Stream Content { get; set; }
    }
}
