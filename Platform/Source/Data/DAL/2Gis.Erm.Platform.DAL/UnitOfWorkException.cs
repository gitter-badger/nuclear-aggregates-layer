using System;

namespace DoubleGis.Erm.Platform.DAL
{
    public class UnitOfWorkException : Exception
    {
        public UnitOfWorkException() { }
        public UnitOfWorkException(string message) : base(message) { }
    }
}