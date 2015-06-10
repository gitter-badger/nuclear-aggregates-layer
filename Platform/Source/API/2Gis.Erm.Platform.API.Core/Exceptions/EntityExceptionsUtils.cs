using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.API.Core.Exceptions
{
    public static class EntityExceptionsUtils
    {
        /// <summary>
        /// Более или менее лаконичный способ проверки, вернулось ли что-нибудь из FindOne. 
        /// По-хорошему для partable сущностей нужно запилить поддержку Single и т.п.
        /// </summary>
        public static TEntity EnsureFound<TEntity>(this TEntity entity) where TEntity : class, IEntity
        {
            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TEntity));
            }

            return entity;
        }
    }
}