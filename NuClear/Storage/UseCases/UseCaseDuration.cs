namespace NuClear.Storage.UseCases
{
    /// <summary>
    /// Вероятная длительность операции в попугаях
    /// Позволяет дополнить информацию об операции знанием о её относительной длительности (длинее, чем обычно; обычная и т.п.)
    /// </summary>
    public enum UseCaseDuration
    {
        None = 0,
        Default = 60,
        BelowNormal = 30,
        Normal = 60,
        AboveNormal = 120,
        Long = 300,
        VeryLong = 600,
        ExtraLong = 1200
    }
}
