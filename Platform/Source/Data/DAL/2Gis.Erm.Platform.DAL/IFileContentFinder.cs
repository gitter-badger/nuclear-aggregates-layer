using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IFileContentFinder
    {
        IQueryable<FileWithContent> Find(IFindSpecification<FileWithContent> findSpecification);
        IQueryable<TOutput> Find<TOutput>(ISelectSpecification<FileWithContent, TOutput> selectSpecification, IFindSpecification<FileWithContent> findSpecification);
        IQueryable<FileWithContent> Find(Expression<Func<FileWithContent, bool>> expression);
    }
}