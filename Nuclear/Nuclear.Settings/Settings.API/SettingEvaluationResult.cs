namespace DoubleGis.Erm.Platform.Common.Settings
{
    public sealed class SettingEvaluationResult<TSetting>
    {
        private TSetting _value;

        public SettingEvaluationResult()
        {
            _value = default(TSetting);
        }

        public static SettingEvaluationResult<TSetting> Error
        {
            get
            {
                return new SettingEvaluationResult<TSetting>();
            }
        }

        public bool Successed { get; private set; }
        
        public TSetting Value
        {
            get
            {
                return _value;
            }

            set
            {
                Successed = true;
                _value = value;
            }
        }
    }
}