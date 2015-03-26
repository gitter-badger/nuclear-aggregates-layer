using Quartz;

namespace NuClear.Jobs
{
    /// <summary>
    /// ��������� ��������� - ������������ ��� ���������������� DI. 
    /// ��������� ��������� IJob, �.�. ����� Quartz �� ����� ������� ���� job-������� - �������� ������, ��� job ������ ������������� ��������� IJob
    /// </summary>
    public interface ITaskServiceJob : IJob
    {
    }
}