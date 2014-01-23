using System;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IEnumLocalizer
    {
        string GetLocalizedString(Enum enumId);
    }
}