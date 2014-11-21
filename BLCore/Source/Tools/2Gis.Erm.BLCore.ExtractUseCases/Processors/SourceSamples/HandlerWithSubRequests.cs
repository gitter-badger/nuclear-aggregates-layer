namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.SourceSamples
{
    internal sealed class RevertReleaseHandler : RequestHandler<RevertReleaseRequest, RevertReleaseResponse>
    {
        private readonly ISubRequestProcessor _subRequestProcessor;
        private readonly IReleaseRepository _releaseAggregateRepository;
        private readonly IAccountRepository _accountAggregateRepository;
        private readonly ICommonLog _logger;

        public RevertReleaseHandler(
            ISubRequestProcessor subRequestProcessor,
            IReleaseRepository releaseAggregateRepository,
            IAccountRepository accountAggregateRepository,
            ICommonLog logger)
        {
            _subRequestProcessor = subRequestProcessor;
            _releaseAggregateRepository = releaseAggregateRepository;
            _accountAggregateRepository = accountAggregateRepository;
            _logger = logger;
        }

        protected override RevertReleaseResponse Handle(RevertReleaseRequest request)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var releaseInfo = _releaseAggregateRepository.GetReleaseInfo(request.ReleaseInfoId);
                if (releaseInfo == null)
                {
                    throw new NotificationException("В журнале сборки не найдена запись с указанным id=" + request.ReleaseInfoId);
                }

                var organizationUnitName = _releaseAggregateRepository.GetOrganizationUnitName(releaseInfo.OrganizationUnitId);
                var releasePeriod = new TimePeriod(releaseInfo.PeriodStartDate, releaseInfo.PeriodEndDate);

                if (_accountAggregateRepository.IsInactiveLocksExists(releaseInfo.OrganizationUnitId, releasePeriod))
                {
                    throw new NotificationException(
                        string.Format(
                            Resources.InactiveLocksExistsForPeriodAndOrganizatonUnit, releasePeriod.Start, releasePeriod.End, organizationUnitName));
                }

                _logger.InfoFormatEx(Resources.RevertReleaseOperationStart, organizationUnitName, releasePeriod.Start, releasePeriod.End);

                BulkActivateLimitResponse bulkActivateLimitResponse = null;
                BulkDeleteLockResponse bulkDeleteLockResponse = null;

                try
                {
                    _subRequestProcessor.HandleSubRequest(new CheckOperationPeriodRequest { Period = releasePeriod }, Context);

                    bulkActivateLimitResponse =
                        (BulkActivateLimitResponse)
                        _subRequestProcessor.HandleSubRequest(
                            new BulkActivateLimitRequest { OrganizationUnitId = releaseInfo.OrganizationUnitId, Period = releasePeriod }, Context);

                    bulkDeleteLockResponse =
                        (BulkDeleteLockResponse)
                        _subRequestProcessor.HandleSubRequest(
                            new BulkDeleteLockRequest { OrganizationUnitId = releaseInfo.OrganizationUnitId, Period = releasePeriod }, Context);
                }
                finally
                {
                    _logger.InfoFormatEx(
                        Resources.RevertReleaseOperationStop,
                        organizationUnitName,
                        releasePeriod.Start,
                        releasePeriod.End,
                        bulkActivateLimitResponse != null ? bulkActivateLimitResponse.LimitsActivated : 0,
                        bulkDeleteLockResponse != null ? bulkDeleteLockResponse.LocksDeleted : 0);
                }

                transaction.Complete();
            }

            return new RevertReleaseResponse { Notification = string.Empty };
        }
    }
}