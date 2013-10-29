﻿using System;
using System.Collections;
using System.Text;
using System.Threading;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;

using Quartz;

namespace DoubleGis.Erm.Platform.TaskService.Jobs
{
    public abstract class DoubleGisJobBase : IDoubleGisJob
    {
        private readonly ISignInService _signInService;
        private readonly IUserImpersonationService _userImpersonationService;
        private readonly ICommonLog _logger;

        protected DoubleGisJobBase(ICommonLog logger, ISignInService signInService, IUserImpersonationService userImpersonationService)
        {
            _signInService = signInService;
            _userImpersonationService = userImpersonationService;
            _logger = logger;
        }
        
        /// <summary>
        /// Пользователь ERM, от имени которого будут производится действия.
        /// </summary>
        public string ErmUserImpersonateAs { get; set; }

        public void Execute(IJobExecutionContext context)
        {
            var group = context.JobDetail.Key.Group;
            var description = context.JobDetail.Description;

            var stringBuilder = new StringBuilder();
            var dictionary = (IDictionary)context.MergedJobDataMap;
            foreach (DictionaryEntry dictionaryEntry in dictionary)
            {
                stringBuilder.Append('[').Append(dictionaryEntry.Key).Append('=').Append(dictionaryEntry.Value).Append(']');
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

        protected void LogInfo(string format, params object[] args)
        {
            _logger.InfoFormatEx(format, args);
            Console.WriteLine(format, args);
        }

        protected void LogError(Exception e, string format, params object[] args)
        {
            _logger.ErrorFormatEx(e, format, args);
            Console.WriteLine(format, args);
        }

        protected abstract void ExecuteInternal(IJobExecutionContext context);
    }
}
