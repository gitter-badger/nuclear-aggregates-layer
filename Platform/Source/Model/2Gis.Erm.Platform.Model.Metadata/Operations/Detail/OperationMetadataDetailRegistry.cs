using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail
{
    public static class OperationMetadataDetailRegistry
    {
        private readonly static Dictionary<Type, Func<IEntityType[], IOperationMetadata>> Identities2MetadataResolverMap =
            new Dictionary<Type, Func<IEntityType[], IOperationMetadata>>();

        static OperationMetadataDetailRegistry()
        {
            Identities2MetadataResolverMap.Add(typeof(AssignIdentity), GetAssignMetadata);
            Identities2MetadataResolverMap.Add(typeof(QualifyIdentity), GetQualifyMetadata);
            Identities2MetadataResolverMap.Add(typeof(DisqualifyIdentity), GetDisqualifyMetadata);
            Identities2MetadataResolverMap.Add(typeof(CheckForDebtsIdentity), GetCheckForDebtsMetadata);
            Identities2MetadataResolverMap.Add(typeof(ChangeTerritoryIdentity), GetChangeTerritoryMetadata);
            Identities2MetadataResolverMap.Add(typeof(ChangeClientIdentity), GetChangeClientMetadata);
            Identities2MetadataResolverMap.Add(typeof(AppendIdentity), GetAppendMetadata);
            Identities2MetadataResolverMap.Add(typeof(ActionHistoryIdentity), GetActionHistoryMetadata);
            Identities2MetadataResolverMap.Add(typeof(DownloadIdentity), GetDownloadFileMetadata);
            Identities2MetadataResolverMap.Add(typeof(UploadIdentity), GetUploadFileMetadata);
            Identities2MetadataResolverMap.Add(typeof(ActivateIdentity), GetActivateMetadata);
            Identities2MetadataResolverMap.Add(typeof(DeactivateIdentity), GetDeactivateMetadata);
            Identities2MetadataResolverMap.Add(typeof(DeleteIdentity), GetDeleteMetadata);
            Identities2MetadataResolverMap.Add(typeof(ModifySimplifiedModelEntityIdentity), GetModifySimplifiedModelEntityMetadata);
            Identities2MetadataResolverMap.Add(typeof(ModifyBusinessModelEntityIdentity), GetModifyBusinessModelEntityMetadata);
            Identities2MetadataResolverMap.Add(typeof(CancelIdentity), GetCancelMetadata);
            Identities2MetadataResolverMap.Add(typeof(CompleteIdentity), GetCompleteMetadata);
            Identities2MetadataResolverMap.Add(typeof(ReopenIdentity), GetReopenMetadata);
        }       

        public static IOperationMetadata GetOperationMetadata<TOperationIdentity>(params IEntityType[] operationProcessingEntities)
            where TOperationIdentity : IOperationIdentity, new()
        {
            return GetOperationMetadata(typeof(TOperationIdentity), operationProcessingEntities);
        }

        public static IOperationMetadata GetOperationMetadata(Type operationIdentityType, params IEntityType[] operationProcessingEntities)
        {
            if (!typeof(IOperationIdentity).IsAssignableFrom(operationIdentityType))
            {
                throw new ArgumentException("Specified type " + operationIdentityType + " is not an operation identity valid type");
            }

            Func<IEntityType[], IOperationMetadata> resolver;
            if (!Identities2MetadataResolverMap.TryGetValue(operationIdentityType, out resolver) || resolver == null)
            {
                return null;
            }

            return resolver(operationProcessingEntities);
        }

        #region Metadata Resolvers
        private static AssignMetadata GetAssignMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (!entityName.IsOwnerable())
            {
                return null;
            }

            if (entityName.Equals(EntityType.Instance.Client()) ||
                entityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return new AssignMetadata { PartialAssignSupported = true };
            }

            return new AssignMetadata();
        }

        private static CancelMetadata GetCancelMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Activity()) ||
                entityName.Equals(EntityType.Instance.Appointment()) ||
                entityName.Equals(EntityType.Instance.Letter()) ||
                entityName.Equals(EntityType.Instance.Phonecall()) ||
                entityName.Equals(EntityType.Instance.Task()))
            {
                    return new CancelMetadata();
            }

            return null;
        }

        private static CompleteMetadata GetCompleteMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Appointment()) ||
                entityName.Equals(EntityType.Instance.Letter()) ||
                entityName.Equals(EntityType.Instance.Phonecall()) ||
                entityName.Equals(EntityType.Instance.Task()))
            {
                    return new CompleteMetadata();
            }

            return null;
        }

        private static ReopenMetadata GetReopenMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Appointment()) ||
                entityName.Equals(EntityType.Instance.Letter()) ||
                entityName.Equals(EntityType.Instance.Phonecall()) ||
                entityName.Equals(EntityType.Instance.Task()))
            {
                    return new ReopenMetadata();
            }

            return null;
        }

        private static QualifyMetadata GetQualifyMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Firm()))
            {
                    return new QualifyMetadata();
            }

            if (entityName.Equals(EntityType.Instance.Client()))
            {
                    return new QualifyMetadata { CheckForDebtsSupported = true };
            }

            return null;
        }

        private static DisqualifyMetadata GetDisqualifyMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Firm()) ||
                entityName.Equals(EntityType.Instance.Client()))
            {
                    return new DisqualifyMetadata();
            }

            return null;
        }

        private static CheckForDebtsMetadata GetCheckForDebtsMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Client()))
            {
                    return new CheckForDebtsMetadata();
            }

            return null;
        }

        private static ChangeTerritoryMetadata GetChangeTerritoryMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Firm()) ||
                entityName.Equals(EntityType.Instance.Client()))
            {
                    return new ChangeTerritoryMetadata();
            }

            return null;
        }

        private static ChangeClientMetadata GetChangeClientMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Firm()) ||
                entityName.Equals(EntityType.Instance.LegalPerson()) ||
                entityName.Equals(EntityType.Instance.Deal()))
            {
                    return new ChangeClientMetadata();
            }

            return null;
        }

        private static AppendMetadata GetAppendMetadata(IEntityType[] entityTypes)
        {
            if (entityTypes == null || entityTypes.Length != 2)
            {
                throw new InvalidOperationException("Can't resolve append operation metadata for invalid entities types list");
            }

            var parentEntityType = entityTypes[1];
            var appendedEntityType = entityTypes[0];
            if (parentEntityType.Equals(EntityType.Instance.Client()) &&
                appendedEntityType.Equals(EntityType.Instance.Client()))
            {
                return new AppendMetadata();
            }

            if (parentEntityType.Equals(EntityType.Instance.Deal()) &&
                (appendedEntityType.Equals(EntityType.Instance.Firm()) ||
                 appendedEntityType.Equals(EntityType.Instance.LegalPerson())))
            {
                return new AppendMetadata();
            }

            if (parentEntityType.Equals(EntityType.Instance.User()) &&
                (appendedEntityType.Equals(EntityType.Instance.OrganizationUnit()) ||
                 appendedEntityType.Equals(EntityType.Instance.Role()) ||
                 appendedEntityType.Equals(EntityType.Instance.Territory())))
            {
                return new AppendMetadata();
            }

            if (parentEntityType.Equals(EntityType.Instance.Theme()) &&
                (appendedEntityType.Equals(EntityType.Instance.Category()) ||
                 appendedEntityType.Equals(EntityType.Instance.OrganizationUnit())))
            {
                return new AppendMetadata();
            }

            return null;
        }

        private static ActionHistoryMetadata GetActionHistoryMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Appointment()) ||
                entityName.Equals(EntityType.Instance.Letter()) ||
                entityName.Equals(EntityType.Instance.Phonecall()) ||
                entityName.Equals(EntityType.Instance.Task()))
            {
                return new ActionHistoryMetadata { Properties = new[] { "OwnerCode", "Status" } };
            }

            if (entityName.Equals(EntityType.Instance.Account()) ||
                entityName.Equals(EntityType.Instance.Client()) ||
                entityName.Equals(EntityType.Instance.Firm()))
            {
                    return new ActionHistoryMetadata { Properties = new[] { "OwnerCode" } };
            }

            if (entityName.Equals(EntityType.Instance.AccountDetail()))
            {
                    return new ActionHistoryMetadata { Properties = new[] { "OwnerCode", "Amount", "Description" } };
            }

            if (entityName.Equals(EntityType.Instance.Deal()))
            {
                    return new ActionHistoryMetadata { Properties = new[] { "DealStage", "OwnerCode" } };
            }

            if (entityName.Equals(EntityType.Instance.Order()))
            {
                    return new ActionHistoryMetadata { Properties = new[] { "WorkflowStepId", "OwnerCode" } };
            }

            if (entityName.Equals(EntityType.Instance.LegalPerson()))
            {
                    return new ActionHistoryMetadata
                        {
                            Properties = new[]
                                {
                                    "LegalName",
                                    "OwnerCode",
                                    "LegalAddress",
                                    "Inn",
                                    "Kpp",
                                    "RegistrationAddress",
                                    "PassportNumber",
                                    "PassportSeries",
                                    "ShortName"
                                }
                        };
            }

            if (entityName.Equals(EntityType.Instance.Limit()))
            {
                    return new ActionHistoryMetadata { Properties = new[] { "StartPeriodDate", "EndPeriodDate", "Amount", "Status" } };
            }

            if (entityName.Equals(EntityType.Instance.AdvertisementElement()))
                        {
                return new ActionHistoryMetadata { Properties = new[] { "ModifiedBy" } };
            }

            if (entityName.Equals(EntityType.Instance.AdvertisementElementStatus()))
                        {
                return new ActionHistoryMetadata { Properties = new[] { "Status" } };
            }

            if (entityName.Equals(EntityType.Instance.Advertisement()))
                        {
                return new ActionHistoryMetadata { Properties = new[] { "ModifiedBy" } };
            }

            if (entityName.Equals(EntityType.Instance.Bargain()))
                    {
                return new ActionHistoryMetadata { Properties = new[] { "BargainEndDate" } };
            }

            return null;
        }

        private static DownloadFileMetadata GetDownloadFileMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (!entityName.IsFileEntity())
            {
                return null;
            }

            return new DownloadFileMetadata();
        }

        private static UploadFileMetadata GetUploadFileMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (!entityName.IsFileEntity())
            {
                return null;
            }

            return new UploadFileMetadata();
        }

        private static ActivateMetadata GetActivateMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (!entityName.IsDeactivatable())
            {
                return null;
            }

            return new ActivateMetadata();
        }

        private static DeactivateMetadata GetDeactivateMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (!entityName.IsDeactivatable())
            {
                return null;
            }

            return new DeactivateMetadata();
        }

        private static DeleteMetadata GetDeleteMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (!entityName.IsDeletable())
            {
                return null;
            }

            return new DeleteMetadata();
        }

        private static ModifySimplifiedModelEntityMetadata GetModifySimplifiedModelEntityMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.Category()) ||
                entityName.Equals(EntityType.Instance.Operation()))
            {
                    return null;
            }

            return new ModifySimplifiedModelEntityMetadata();
        }

        private static ModifyBusinessModelEntityMetadata GetModifyBusinessModelEntityMetadata(IEntityType[] entityTypes)
        {
            var entityName = entityTypes.Single();
            if (entityName.Equals(EntityType.Instance.FirmAddress()) ||
                entityName.Equals(EntityType.Instance.FirmContact()) ||
                entityName.Equals(EntityType.Instance.Lock()) ||
                entityName.Equals(EntityType.Instance.LockDetail()))
            {
                    return null;
            }

            return new ModifyBusinessModelEntityMetadata();
        }

        #endregion
    }
}
