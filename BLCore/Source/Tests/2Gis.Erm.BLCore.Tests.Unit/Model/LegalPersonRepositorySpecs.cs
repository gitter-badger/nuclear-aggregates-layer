using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.Model
{
    public class LegalPersonRepositorySpecs
    {
        public abstract class LegalPersonRepositoryContext
        {
            private Establish context = () => LegalPersonRepository = new LegalPersonRepository(null,
                                                                                                null,
                                                                                                null,
                                                                                                Mock.Of<ISecureRepository<LegalPerson>>(),
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null,
                                                                                                null);

            protected static LegalPersonRepository LegalPersonRepository { get; private set; }   
        }

        [Tags("Model")]
        [Subject(typeof(LegalPersonRepository))]
        class When_changing_legal_requisites : LegalPersonRepositoryContext
        {
            const string Inn = "0276076329";
            const string Kpp = "027501002";
            const string LegalAddress = "Новосибирск";

            static LegalPerson _legalPerson;

            Establish context = () => _legalPerson = new LegalPerson
                {
                    Inn = "0276076328",
                    Kpp = "027501001",
                    LegalAddress = "Чита"
                };

            Because of = () => LegalPersonRepository.ChangeLegalRequisites(_legalPerson, Inn, Kpp, LegalAddress);

            It INN_should_be_changed = () => _legalPerson.Inn.Should().Be(Inn);
            It KPP_should_be_changed = () => _legalPerson.Kpp.Should().Be(Kpp);
            It LegalAddress_should_be_changed = () => _legalPerson.LegalAddress.Should().Be(LegalAddress);
        }

        [Tags("Model")]
        [Subject(typeof(LegalPersonRepository))]
        class When_changing_natural_requisites : LegalPersonRepositoryContext
        {
            const string PassportSeries = "3076";
            const string PassportNumber = "654321";
            const string RegistrationAddress = "Новосибирск";

            static LegalPerson _legalPerson;

            Establish context = () => _legalPerson = new LegalPerson
                {
                    PassportNumber = "123456",
                    PassportSeries = "6703",
                    RegistrationAddress = "Чита"
                };
                

            Because of = () => LegalPersonRepository.ChangeNaturalRequisites(_legalPerson, PassportSeries, PassportNumber, RegistrationAddress);

            It PassportSeries_should_be_changed = () => _legalPerson.PassportSeries.Should().Be(PassportSeries);
            It PassportNumber_should_be_changed = () => _legalPerson.PassportNumber.Should().Be(PassportNumber);
            It RegistrationAddress_should_be_changed = () => _legalPerson.RegistrationAddress.Should().Be(RegistrationAddress);
        }

        [Tags("Model")]
        [Subject(typeof(LegalPersonRepository))]
        class When_synchroninizing_collection_of_objects_with_1C : LegalPersonRepositoryContext
        {
            static LegalPerson _legalPerson1;
            static LegalPerson _legalPerson2;

            Establish context = () =>
                {
                    _legalPerson1 = new LegalPerson { IsInSyncWith1C = false };
                    _legalPerson2 = new LegalPerson { IsInSyncWith1C = true };
                };

            Because of = () => LegalPersonRepository.SyncWith1C(new[] { _legalPerson1, _legalPerson2 });

            It legal_person_1_should_be_synced_with_1C = () => _legalPerson1.IsInSyncWith1C.Should().BeTrue();
            It legal_person_2_should_be_synced_with_1C = () => _legalPerson2.IsInSyncWith1C.Should().BeTrue();
        }
    }
}