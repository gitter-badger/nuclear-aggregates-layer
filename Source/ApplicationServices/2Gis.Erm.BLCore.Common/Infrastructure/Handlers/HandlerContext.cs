using System;

namespace DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers
{
    public sealed class HandlerContext
    {
        private readonly Type _handlerType;

        public HandlerContext(Type handlerType)
        {
            _handlerType = handlerType;
        }

        /// <summary>
        /// Тип handler к которому относится данный экземпляр контекста хендлера
        /// </summary>
        internal Type HandlerType
        {
            get
            {
                return _handlerType;
            }
        }
    }
}
