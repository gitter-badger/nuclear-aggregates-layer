using System;
using System.Collections;
using System.Text;
using System.Threading;

using DoubleGis.Erm.Platform.API.Security;

using NuClear.Tracing.API;

using Quartz;

namespace NuClear.Jobs
{
    public abstract class TaskServiceJobBase : ITaskServiceJob
    {
        private readonly ISignInService _signInService;
        private readonly IUserImpersonationService _userImpersonationService;
        private readonly ITracer _tracer;

        protected TaskServiceJobBase(
            ISignInService signInService, 
            IUserImpersonationService userImpersonationService,
            ITracer tracer)
        {
            _signInService = signInService;
            _userImpersonationService = userImpersonationService;
            _tracer = tracer;
        }
        
        /// <summary>
        /// Пользователь ERM, от имени которого будут производится действия.
        /// </summary>
        public string ErmUserImpersonateAs { get; set; }

        protected ITracer Tracer
        {
            get { return _tracer; }
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
                Tracer.InfoFormat("[{0}][{1}]{2} - старт задачи", group, description, jobDataMap);

                // аутентифицируем текущего пользователя в системе и выполняем logon
                _signInService.SignIn();

                // если указано, то подменяем пользователя указанным 
                if (!string.IsNullOrEmpty(ErmUserImpersonateAs))
                {
                    Tracer.InfoFormat("[{0}][{1}]{2} - используем учетную запись пользователя '{3}'", group, description, jobDataMap, ErmUserImpersonateAs);
                    _userImpersonationService.ImpersonateAsUser(ErmUserImpersonateAs);
                }

                ExecuteInternal(context);

                Tracer.InfoFormat("[{0}][{1}]{2} - окончание задачи", group, description, jobDataMap);
            }
            catch (Exception ex)
            {
                Tracer.ErrorFormat(ex, "[{0}][{1}]{2} - ошибка при выполнении задачи", group, description, jobDataMap);
                throw new JobExecutionException(ex);
            }
            finally
            {
                Thread.CurrentPrincipal = principal;
            }
        }

        protected abstract void ExecuteInternal(IJobExecutionContext context);
    }
}
