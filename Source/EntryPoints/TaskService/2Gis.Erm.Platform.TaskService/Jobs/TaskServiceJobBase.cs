using System;
using System.Collections;
using System.Text;
using System.Threading;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;

using Quartz;

namespace DoubleGis.Erm.Platform.TaskService.Jobs
{
    public abstract class TaskServiceJobBase : ITaskServiceJob
    {
        private readonly ISignInService _signInService;
        private readonly IUserImpersonationService _userImpersonationService;
        private readonly ICommonLog _logger;

        protected TaskServiceJobBase(
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService,
            ICommonLog logger)
        {
            _signInService = signInService;
            _userImpersonationService = userImpersonationService;
            _logger = logger;
        }
        
        /// <summary>
        /// Пользователь ERM, от имени которого будут производится действия.
        /// </summary>
        public string ErmUserImpersonateAs { get; set; }

        protected ICommonLog Logger
        {
            get { return _logger; }
        }

        public void Execute(IJobExecutionContext context)
        {
            var group = context.JobDetail.Key.Group;
            var description = context.JobDetail.Description;

            var stringBuilder = new StringBuilder();
            var dictionary = (IDictionary)context.MergedJobDataMap;
            foreach (DictionaryEntry dictionaryEntry in dictionary)
            {
                stringBuilder
                    .Append('[')
                    .Append(dictionaryEntry.Key)
                    .Append('=').Append(dictionaryEntry.Value)
                    .Append(']');
            }

            var jobDataMap = stringBuilder.ToString();

            var principal = Thread.CurrentPrincipal;

            try
            {
                LogInfo("[{0}][{1}]{2} - старт задачи", group, description, jobDataMap);

                // аутентифицируем текущего пользователя в системе и выполняем logon
                _signInService.SignIn();

                // если указано, то подменяем пользователя указанным 
                if (!string.IsNullOrEmpty(ErmUserImpersonateAs))
                {
                    LogInfo("[{0}][{1}]{2} - используем учетную запись пользователя '{3}'", group, description, jobDataMap, ErmUserImpersonateAs);
                    _userImpersonationService.ImpersonateAsUser(ErmUserImpersonateAs);
                }

                ExecuteInternal(context);

                LogInfo("[{0}][{1}]{2} - окончание задачи", group, description, jobDataMap);
            }
            catch (Exception ex)
            {
                LogError(ex, "[{0}][{1}]{2} - ошибка при выполнении задачи", group, description, jobDataMap);
                throw new JobExecutionException(ex);
            }
            finally
            {
                Thread.CurrentPrincipal = principal;
            }
        }

        // FIXME {all, 11.06.2014}: отказаться от использование этого wrapper метода, перейти к использованию ICommonLog напрямую, логирование на консоль, если оно нужно - реализовать добавив соответствующий appender + фильтры + возможно не для всех сред
        [Obsolete]
        protected void LogInfo(string format, params object[] args)
        {
            Logger.InfoFormatEx(format, args);
            Console.WriteLine(format, args);
        }

        // FIXME {all, 11.06.2014}: отказаться от использование этого wrapper метода, перейти к использованию ICommonLog напрямую, логирование на консоль, если оно нужно - реализовать добавив соответствующий appender + фильтры + возможно не для всех сред
        [Obsolete]
        protected void LogError(Exception e, string format, params object[] args)
        {
            Logger.ErrorFormatEx(e, format, args);
            Console.WriteLine(format, args);
        }

        protected abstract void ExecuteInternal(IJobExecutionContext context);
    }
}
