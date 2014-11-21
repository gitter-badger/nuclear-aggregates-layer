using System;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Processings
{
    public class ProcessingDescriptor<TProcessingDetail> : IProcessingDescriptor
        where TProcessingDetail : class
    {
        private readonly Guid _id;
        private readonly TProcessingDetail _processingDetail;

        public ProcessingDescriptor(Guid processingId, TProcessingDetail processingDetail)
        {
            _id = processingId;
            _processingDetail = processingDetail;
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public TProcessingDetail Detail
        {
            get
            {
                return _processingDetail;
            }
        }

        public override string ToString()
        {
            return string.Format("Processing. Id:{0}. Detail:{1}", _id, _processingDetail);
        }
    }
}