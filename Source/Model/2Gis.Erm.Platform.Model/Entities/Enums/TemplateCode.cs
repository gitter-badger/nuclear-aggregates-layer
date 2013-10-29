namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum TemplateCode
    {
        None = 0,

        // �������
        BargainLegalPerson = 10,
        BargainBusinessman = 11,
        BargainNaturalPerson = 12,

        // ���. ����������
        AdditionalAgreementLegalPerson = 20,
        AdditionalAgreementBusinessman = 21,
        AdditionalAgreementNaturalPerson = 22,
        BargainAdditionalAgreementLegalPerson = 23,
        BargainAdditionalAgreementBusinessman = 24,

        // ���� �� ������
        BillLegalPerson = 30,
        BillBusinessman = 31,
        BillNaturalPerson = 32,

        // ������ ���� �� ������
        JointBillLegalPerson = 40,
        JointBillBusinessman = 41,
        JointBillNaturalPerson = 42,

        // ����� ������
        OrderWithoutVatWithDiscount = 58,
        OrderWithVatWithDiscount = 59,
        OrderWithoutVatWithoutDiscount = 60,
        OrderWithVatWithoutDiscount = 61,

        // ����������� � �����������
        TerminationNoticeLegalPerson = 70,
        TerminationNoticeBusinessman = 71,
        TerminationNoticeNaturalPerson = 72,
        TerminationNoticeWithoutReasonLegalPerson = 73,
        TerminationNoticeWithoutReasonBusinessman = 74,
        TerminationNoticeBargainLegalPerson = 75,
        TerminationNoticeBargainBusinessman = 76,
        TerminationNoticeBargainWithoutReasonLegalPerson = 77,
        TerminationNoticeBargainWithoutReasonBusinessman = 78,

        // ������������ ����������� � �����������
        RegionalTerminationNoticeBranch2Branch = 80,
        RegionalTerminationNoticeNotBranch2Branch = 81,

        // ������ �� �����
        LimitRequest = 90,

        // ���������� ����������
        ReferenceInformation = 100,

        // ����������� ������
        LetterOfGuarantee = 110,
        LetterOfGuaranteeAdvMaterial = 111
    }
}