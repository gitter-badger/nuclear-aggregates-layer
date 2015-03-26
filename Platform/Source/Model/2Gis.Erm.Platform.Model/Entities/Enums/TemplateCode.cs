
namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum TemplateCode
    {
        None = 0,

        // �������
        BargainProlongationAgreement = 12,
        BargainNewSalesModel = 13,
        ClientBargainAlternativeLanguage = 14,
        ClientBargain = 15,
        AgentBargain = 16,

        // ���. ����������
        AdditionalAgreementLegalPerson = 20,
        AdditionalAgreementBusinessman = 21,
        AdditionalAgreementNaturalPerson = 22,
        BargainAdditionalAgreementLegalPerson = 23,
        BargainAdditionalAgreementBusinessman = 24,
        FirmChangeAgreement = 25,
        BindingChangeAgreement = 26,

        // ���� �� ������
        BillLegalPerson = 30,
        BillBusinessman = 31,
        BillNaturalPerson = 32,

        // ������ ���� �� ������
        JointBill = 40,

        // ����� ������ (58, 60, 61 - ����������)
        Order = 59,
        OrderMultiPlannedProvision = 62,

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

        // ������ �� �����
        LimitRequest = 90,

        // ���������� ����������
        ReferenceInformation = 100,

        // ����������� ������
        LetterOfGuarantee = 110,
        LetterOfGuaranteeAdvMaterial = 111,

        // AcceptanceReport
        AcceptanceReport = 120
    }
}