using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM
{
    // FIXME {all, 01.01.2014}: уже есть типы сподобным функционалом, например, CrmDataContextExtensions, данный тип скорее должен быть таким же набором методов расширений, своего состояния у него фактически нет 
    [Obsolete]
    public interface ICrmTaskFactory
    {
        Guid CreateTask(UserDto owner, HotClientRequestDto hotClient, RegardingObject regardingObject);
    }
}
