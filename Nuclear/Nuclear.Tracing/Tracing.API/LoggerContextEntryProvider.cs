namespace DoubleGis.Erm.Platform.Common.Logging
{
    /// <summary>
    /// Контейнер для значения свойства одного из контекстов логирования (см. например GlobalContext log4net)
    /// При логировании сообщения, свойства из контекстов логирования становяться составной частью данных LoggingEvent(в терминах log4net), наряду,
    /// например, с сообщением которое было указано при вызове логера
    /// При обработке каждым appender (в терминах log4net) loggingevent конвертируется необходимым образом для отправки в пункт назначения appender, 
    /// вот на этапе конвертации и могут быть использованы значения из контекстов логирования, т.е. в том числе и значение, доступ к которому обеспечивает данный тип.
    /// Если тип свойства контекста CustomObject, при ссылках на значения таких свойств в PatternLayouts, происходит их конвертация в строки через вызов метода ToString
    /// Т.о. в случае log4net никто непосредственно get для свойства Value не вызывает, 
    /// т.к. даже не значет о наличии этого свойства, просто для экземпляра данного типа (его подклассов) вызывается ToString
    /// </summary>
    public abstract class LoggerContextEntryProvider : ILoggerContextEntryProvider
    {
        private readonly string _loggerContextKey;

        protected LoggerContextEntryProvider(string loggerContextKey)
        {
            _loggerContextKey = loggerContextKey;
        }

        public string Key
        {
            get { return _loggerContextKey; }
        }

        public abstract string Value { get; set; }

        public sealed override string ToString()
        {
            return Value ?? "Not found";
        }
    }
}
