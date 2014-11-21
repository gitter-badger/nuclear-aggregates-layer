using System;

namespace DoubleGis.Erm.Platform.DAL
{
    [Flags]
    public enum SaveOptions
    {
        None = 0,
        AcceptAllChangesAfterSave = 1,
        DetectChangesBeforeSave = 2
    }
}