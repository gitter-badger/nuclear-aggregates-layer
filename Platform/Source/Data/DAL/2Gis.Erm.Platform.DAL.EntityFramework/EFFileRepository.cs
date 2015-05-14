using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;
using NuClear.Storage;
using NuClear.Storage.Core;
using NuClear.Storage.Specifications;

using File = DoubleGis.Erm.Platform.Model.Entities.Erm.File;
using IConnectionStringSettings = NuClear.Storage.ConnectionStrings.IConnectionStringSettings;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    /// <summary>
    /// Дешевая и сердитая реализация IRepository для файлов - инкапсулирует работу над двумя таблицами: Shared.Files и Shared.FileBinaries.
    /// </summary>
    public class EFFileRepository : IRepository<FileWithContent>, IFileContentFinder
    {
        private const string SelectFileCommand = @"SELECT Data.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT()
                                                   FROM Shared.FileBinaries WHERE Id = @fileId";

        private const string InsertFileCommand =
@"

INSERT INTO Shared.Files (Id, FileName, ContentType, ContentLength, CreatedBy, ModifiedBy, CreatedOn, ModifiedOn, DgppId)
VALUES (@fileId, @fileName, @contentType, @contentLength, @createdBy, @modifiedBy, @createdOn, @modifiedOn, @dgppId)

INSERT INTO Shared.FileBinaries(Id, Data)
OUTPUT inserted.Id, inserted.Data.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() VALUES (@fileId , 0)";

        /// <summary>
        /// CreatedOn и CreatedBy не обновляются при апдейте.
        /// </summary>
        private const string UpdateFileCommand = @"UPDATE Shared.Files SET FileName = @fileName, ContentType = @contentType, ContentLength = @contentLength,
 ModifiedBy = @modifiedBy, ModifiedOn = @modifiedOn WHERE Id = @fileId
SELECT Id, Data.PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM Shared.FileBinaries WHERE Id = @fileId";

        private const string DeleteFileCommand = @"DELETE FROM Shared.FileBinaries WHERE Id = @fileId
DELETE FROM Shared.Files WHERE Id = @fileId";

        private readonly IUserContext _userContext;
        private readonly IPersistenceChangesRegistryProvider _changesRegistryProvider;
        private readonly string _connectionString;
        private readonly IReadDomainContextProvider _readDomainContextProvider;

        public EFFileRepository(IUserContext userContext,
                                IReadDomainContextProvider readDomainContextProvider,
                                IConnectionStringSettings connectionStringSettings,
                                IPersistenceChangesRegistryProvider changesRegistryProvider)
        {
            _userContext = userContext;
            _changesRegistryProvider = changesRegistryProvider;
            _readDomainContextProvider = readDomainContextProvider;

            _connectionString = connectionStringSettings.GetConnectionString(ErmConnectionStringIdentity.Instance);
        }

        private enum CommandType
        {
            Select,
            Insert,
            Update,
            Delete
        }

        private IQueryable<File> RepositoryFileQuery
        {
            get { return _readDomainContextProvider.Get().GetQueryableSource<File>(); }
        }

        public void Add(FileWithContent entity)
        {
            AddOrUpdateInternal(CommandType.Insert, entity);
            _changesRegistryProvider.ChangesRegistry.Added<FileWithContent>(entity.Id);
        }

        public void AddRange(IEnumerable<FileWithContent> entities)
        {
            throw new NotSupportedException();
        }

        public void DeleteRange(IEnumerable<FileWithContent> entities)
        {
            throw new NotSupportedException();
        }

        public int Save()
        {
            // Пока что у репозитория нет отложенного сохранения.
            return 0;
        }

        public void Update(FileWithContent entity)
        {
            AddOrUpdateInternal(CommandType.Update, entity);
            _changesRegistryProvider.ChangesRegistry.Updated<FileWithContent>(entity.Id);
        }

        public void Delete(FileWithContent entity)
        {
            CheckArgumentNull(entity, "entity");

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = new SqlCommand(DeleteFileCommand, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@fileId", entity.Id);
                    sqlCommand.ExecuteNonQuery();
                }
            }

            _changesRegistryProvider.ChangesRegistry.Deleted<FileWithContent>(entity.Id);
        }

        public void Attach(FileWithContent entity)
        {
            CheckArgumentNull(entity, "entity");
        }

        public IQueryable<FileWithContent> Find(IFindSpecification<FileWithContent> findSpecification)
        {
            return FindInternal(findSpecification.Predicate);
        }

        public IQueryable<TOutput> Find<TOutput>(ISelectSpecification<FileWithContent, TOutput> selectSpecification, IFindSpecification<FileWithContent> findSpecification)
        {
            return FindInternal(findSpecification.Predicate).Select(selectSpecification.Selector);
        }

        public IQueryable<FileWithContent> Find(Expression<Func<FileWithContent, bool>> expression)
        {
            return FindInternal(expression);
        }

        private static void CheckArgumentNull<T>(T value, string parameterName) where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        private IQueryable<FileWithContent> FindInternal(Expression<Func<FileWithContent, bool>> findExpression)
        {
            var result = RepositoryFileQuery.Where(findExpression.ReplaceParameterType<FileWithContent, File>()).ToArray();
            return result.Select(OpenContentForFile).AsQueryable();
        }

        private FileWithContent OpenContentForFile(File file)
        {
            var result = new FileWithContent(file);

            using (var transactionScope = new TransactionScope(TransactionScopeOption.Required))
            {
                using (var sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Open();

                    using (var sqlCommand = CreateCommand(CommandType.Select, result, sqlConnection))
                    {
                        using (var sqlDataReader = sqlCommand.ExecuteReader())
                        {
                            if (!sqlDataReader.Read())
                            {
                                throw new InvalidOperationException();
                            }

                            var filePath = sqlDataReader.GetString(0);
                            var transactionToken = sqlDataReader.GetSqlBytes(1).Buffer;
                            if (transactionToken == null)
                            {
                                throw new InvalidOperationException("Работа с FILESTREAM возможна только в контексте транзакции, откройте транзакцию");
                            }

                            using (var sqlFileStream = new SqlFileStream(filePath, transactionToken, FileAccess.Read, FileOptions.SequentialScan, 0))
                            {
                                var ms = new MemoryStream((int)file.ContentLength);
                                sqlFileStream.CopyTo(ms);

                                result.Content = ms;
                                ms.Seek(0, SeekOrigin.Begin);
                            }
                        }
                    }

                    transactionScope.Complete();
                }
            }

            return result;
        }

        private void AddOrUpdateInternal(CommandType commandType, FileWithContent entity)
        {
            CheckArgumentNull(entity, "entity");

            if (entity.Content == null)
            {
                throw new ArgumentException("entity.Content is null");
            }

            SetEntityAuditableInfo(entity, commandType == CommandType.Insert);
            entity.ContentLength = entity.Content.Length;

            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var sqlCommand = CreateCommand(commandType, entity, sqlConnection))
                {
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        if (!sqlDataReader.Read())
                        {
                            throw new InvalidOperationException();
                        }

                        var fileId = sqlDataReader.GetInt64(0);

                        var filePath = sqlDataReader.GetString(1);
                        var transactionToken = sqlDataReader.GetSqlBytes(2).Buffer;
                        if (transactionToken == null)
                        {
                            throw new InvalidOperationException("Работа с FILESTREAM возможна только в контексте транзакции, откройте транзакцию");
                        }

                        if (commandType == CommandType.Insert)
                        {
                            entity.Id = fileId;
                        }

                        using (var sqlFileStream = new SqlFileStream(filePath, transactionToken, FileAccess.Write, FileOptions.SequentialScan, 0))
                        {
                            entity.Content.Seek(0, SeekOrigin.Begin);
                            entity.Content.CopyTo(sqlFileStream);
                        }
                    }
                }
            }
        }

        private static SqlCommand CreateCommand(CommandType commandType, FileWithContent entity, SqlConnection connection)
        {
            string commandText;

            switch (commandType)
            {
                case CommandType.Select:
                    commandText = SelectFileCommand;
                    break;
                case CommandType.Delete:
                    commandText = DeleteFileCommand;
                    break;
                case CommandType.Insert:
                case CommandType.Update:
                    commandText = commandType == CommandType.Insert ? InsertFileCommand : UpdateFileCommand;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var resultCommand = new SqlCommand(commandText, connection);

            resultCommand.Parameters.AddWithValue("@fileId", entity.Id);

            if (commandType == CommandType.Insert || commandType == CommandType.Update)
            {
                resultCommand.Parameters.AddWithValue("@fileName", entity.FileName);
                resultCommand.Parameters.AddWithValue("@contentType", entity.ContentType);
                resultCommand.Parameters.AddWithValue("@contentLength", entity.ContentLength);

                if (commandType == CommandType.Insert)
                {
                    resultCommand.Parameters.AddWithValue("@createdBy", entity.CreatedBy);
                    resultCommand.Parameters.AddWithValue("@createdOn", entity.CreatedOn);
                }

                resultCommand.Parameters.AddWithValue("@modifiedBy", entity.ModifiedBy.HasValue ? (object)entity.ModifiedBy : DBNull.Value);
                resultCommand.Parameters.AddWithValue("@modifiedOn", entity.ModifiedOn.HasValue ? (object)entity.ModifiedOn.Value : DBNull.Value);

                var dgppIdParameter = new SqlParameter("@dgppId", SqlDbType.BigInt)
                    {
                        IsNullable = true,
                        Value = entity.DgppId.HasValue ? (object)entity.DgppId.Value : DBNull.Value
                    };

                resultCommand.Parameters.Add(dgppIdParameter);
            }

            return resultCommand;
        }

        private void SetEntityAuditableInfo(FileWithContent file, bool isEntityCreated)
        {
            var auditableEntity = file as IAuditableEntity;
            if (auditableEntity == null)
            {
                return;
            }

            var now = DateTime.UtcNow;

            if (isEntityCreated)
            {
                auditableEntity.CreatedOn = now;
                auditableEntity.CreatedBy = _userContext.Identity.Code;
            }

            auditableEntity.ModifiedOn = now;
            auditableEntity.ModifiedBy = _userContext.Identity.Code;
        }
    }
}