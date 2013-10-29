using System.Web;

namespace DoubleGis.Erm.Platform.Web.Mvc.Security
{
    /// <summary>
    /// ������� �������� �� ��������� ��� UserContext (�.�. identity � userprofile)
    /// �������� �� ��������� ����������, �� ������� ����������� �������� ������ � ������������ � UserContext - �� ��������������
    /// </summary>
    public interface IDefaultUserContextConfigurator
    {
        void Configure(HttpRequest processingHttpRequest);
    }
}