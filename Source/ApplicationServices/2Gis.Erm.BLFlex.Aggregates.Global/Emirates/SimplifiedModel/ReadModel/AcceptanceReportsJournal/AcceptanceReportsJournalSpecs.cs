using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Emirates.SimplifiedModel.ReadModel.AcceptanceReportsJournal
{
    public static class AcceptanceReportsJournalSpecs
    {
        public static class Find
        {
            public static IFindSpecification<DictionaryEntityInstance> OnlyAcceptanceReportsJournalRecords
            {
                get
                {
                    return new FindSpecification<DictionaryEntityInstance>(
                        entity =>
                        entity.DictionaryEntityPropertyInstances.Any(property => property.PropertyId == EntityTypeNameIdentity.Instance.Id &&
                                                                                 property.NumericValue == (int)EntityName.AcceptanceReportsJournalRecord));
                }
            }
        }

        public static class Select
        {
            public static ISelectSpecification<DictionaryEntityInstance, AcceptanceReportsJournalRecord> AcceptanceReportsJournalRecords
            {
                get
                {
                    return new SelectSpecification<DictionaryEntityInstance, AcceptanceReportsJournalRecord>(
                        entity => new AcceptanceReportsJournalRecord
                            {
                                Id = entity.Id,
                                CreatedBy = entity.CreatedBy,
                                CreatedOn = entity.CreatedOn,
                                ModifiedBy = entity.ModifiedBy,
                                ModifiedOn = entity.ModifiedOn,
                                IsActive = entity.IsActive,
                                IsDeleted = entity.IsDeleted,
                                Timestamp = entity.Timestamp,

                                OrganizationUnitId = (long)
                                                     entity.DictionaryEntityPropertyInstances.FirstOrDefault(
                                                         instance => instance.PropertyId == OrganizationUnitIdIdentity.Instance.Id).NumericValue,
                                EndDistributionDate = entity.DictionaryEntityPropertyInstances.FirstOrDefault(
                                    instance => instance.PropertyId == EndDistributionDateIdentity.Instance.Id).DateTimeValue.Value,
                                DocumentsAmount = (int)entity.DictionaryEntityPropertyInstances.FirstOrDefault(
                                    instance => instance.PropertyId == DocumentsAmountIdentity.Instance.Id).NumericValue,
                                AuthorId = (long)entity.DictionaryEntityPropertyInstances.FirstOrDefault(
                                    instance => instance.PropertyId == AuthorIdIdentity.Instance.Id).NumericValue,
                            });
                }
            }
        }
    }
}