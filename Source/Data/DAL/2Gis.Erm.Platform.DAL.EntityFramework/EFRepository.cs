using System;
using System.Data.Objects;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public abstract class EFRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IModifiableDomainContextProvider _domainContextProvider;
        private EFDomainContext _context;

        protected EFRepository(IModifiableDomainContextProvider domainContextProvider)
        {
            _domainContextProvider = domainContextProvider;
        }

        protected internal EFDomainContext DomainContext
        {
            get 
            {
                if (_context != null)
                {
                    return _context;
                }

                _context = _domainContextProvider.Get<TEntity>() as EFDomainContext;
                if (_context == null)
                {
                    throw new ApplicationException("IObjectContext implementation must inherit from ObjectContext");
                }

                return _context;
            }
        }

        protected void CheckArgumentNull<T>(T value, string parameterName) 
            where T : class
        {
            if (null == value)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        protected T ExecuteStoredProcedure<T>(string procedureName, params Tuple<string, object>[] parameters)
        {
            var result = DomainContext.ExecuteFunction<T>(procedureName, parameters.Select(x => new ObjectParameter(x.Item1, x.Item2)).ToArray());
            return result.FirstOrDefault();
        }
    }
}
