namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    // TODO {all, 30.10.2014}: � ���� ���� ����� ��� smallint => EF ����� ������� ��� � enum ������ ���� � �������� underlying type ������ short
    public enum LimitStatus : short
    {
        None = 0,
        Opened = 1,
        Approved = 2,
        Rejected = 3,
    }
}