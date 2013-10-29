using Quartz;

namespace DoubleGis.Erm.Platform.TaskService.Jobs
{
    /// <summary>
    /// ��������� ��������� - ������������ ��� ���������������� DI. 
    /// ��������� ��������� IJob, �.�. ����� Quartz �� ����� ������� ���� job-������� - �������� ������, ��� job ������ ������������� ��������� IJob
    /// </summary>
    public interface IDoubleGisJob : IJob
    {
    }
}