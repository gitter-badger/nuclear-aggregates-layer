using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail.Concrete;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Detail
{
    public static class OperationMetadataDetailRegistry
    {
        private readonly static Dictionary<Type, Func<EntityName[], IOperationMetadata>> Identities2MetadataResolverMap =
            new Dictionary<Type, Func<EntityName[], IOperationMetadata>>();

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

        public static IOperationMetadata GetOperationMetadata<TOperationIdentity>(params EntityName[] operationProcessingEntities)
            where TOperationIdentity : IOperationIdentity, new()
        {
            return GetOperationMetadata(typeof(TOperationIdentity), operationProcessingEntities);
        }

        public static IOperationMetadata GetOperationMetadata(Type operationIdentityType, params EntityName[] operationProcessingEntities)
        {
            if (!typeof(IOperationIdentity).IsAssignableFrom(operationIdentityType))
            {
                throw new ArgumentException("Specified type " + operationIdentityType + " is not an operation identity valid type");
            }

            Func<EntityName[], IOperationMetadata> resolver;
            if (!Identities2MetadataResolverMap.TryGetValue(operationIdentityType, out resolver) || resolver == null)
            {
                return null;
            }

            return resolver(operationProcessingEntities);
        }

        #region Metadata Resolvers
        private static AssignMetadata GetAssignMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            if (!entityName.IsOwnerable())
            {
                return null;
            }

            var assignMetadata = new AssignMetadata();
            switch (entityName)
            {
                case EntityName.Client:
                case EntityName.LegalPerson:
                    assignMetadata.PartialAssignSupported = true;
                    break;
            }

            return assignMetadata;
        }

        private static CancelMetadata GetCancelMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Activity:
                case EntityName.Appointment:
                case EntityName.Letter:
                case EntityName.Phonecall:
                case EntityName.Task:
                    return new CancelMetadata();
            }
            return null;
        }

        private static CompleteMetadata GetCompleteMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Appointment:
                case EntityName.Letter:
                case EntityName.Phonecall:
                case EntityName.Task:
                    return new CompleteMetadata();
            }

            return null;
        }

        private static ReopenMetadata GetReopenMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Appointment:
                case EntityName.Letter:
                case EntityName.Phonecall:
                case EntityName.Task:
                    return new ReopenMetadata();
            }

            return null;
        }

        private static QualifyMetadata GetQualifyMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Firm:
                    return new QualifyMetadata();
                case EntityName.Client:
                    return new QualifyMetadata { CheckForDebtsSupported = true };
            }

            return null;
        }

        private static DisqualifyMetadata GetDisqualifyMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Firm:
                case EntityName.Client:
                    return new DisqualifyMetadata();
            }

            return null;
        }

        private static CheckForDebtsMetadata GetCheckForDebtsMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Client:
                    return new CheckForDebtsMetadata();
            }

            return null;
        }

        private static ChangeTerritoryMetadata GetChangeTerritoryMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Firm:
                case EntityName.Client:
                    return new ChangeTerritoryMetadata();
            }

            return null;
        }

        private static ChangeClientMetadata GetChangeClientMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Firm:
                case EntityName.LegalPerson:
                case EntityName.Deal:
                    return new ChangeClientMetadata();
            }
            return null;
        }

        private static AppendMetadata GetAppendMetadata(EntityName[] entityNames)
        {
            if (entityNames == null || entityNames.Length != 2)
            {
                throw new InvalidOperationException("Can't resolve append operation metadata for invalid entities types list");
            }

            EntityName parentEntityName = entityNames[1];
            EntityName appendedEntityName = entityNames[0];
            switch (parentEntityName)
            {
                case EntityName.BranchOffice:
                    {
                        switch (appendedEntityName)
                        {
                            case EntityName.User:
                                return new AppendMetadata();
                        }

                        break;
                    }

                case EntityName.Client:
                    {
                        switch (appendedEntityName)
                        {
                            case EntityName.Client:
                                return new AppendMetadata();
                        }

                        break;
                    }

                case EntityName.Deal:
                    {
                        switch (appendedEntityName)
                        {
                            case EntityName.Firm:
                            case EntityName.LegalPerson:
                                return new AppendMetadata();
                        }

                        break;
                    }

                case EntityName.User:
                    {
                        switch (appendedEntityName)
                        {
                            case EntityName.OrganizationUnit:
                            case EntityName.Role:
                            case EntityName.Territory:
                                return new AppendMetadata();
                        }

                        break;
                    }

                case EntityName.Theme:
                    {
                        switch (appendedEntityName)
                        {
                            case EntityName.Category:
                            case EntityName.OrganizationUnit:
                                return new AppendMetadata();
                        }
                        break;
                    }
            }

            return null;
        }

        private static ActionHistoryMetadata GetActionHistoryMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Appointment:                    
                case EntityName.Letter:
                case EntityName.Phonecall:
                case EntityName.Task:
                    return new ActionHistoryMetadata { Properties = new[] { "OwnerCode", "Status" } };
                case EntityName.Account:
                case EntityName.Client:
                case EntityName.Firm:
                    return new ActionHistoryMetadata { Properties = new[] { "OwnerCode" } };
                case EntityName.AccountDetail:
                    return new ActionHistoryMetadata { Properties = new[] { "OwnerCode", "Amount", "Description" } };
                case EntityName.Deal:
                    return new ActionHistoryMetadata { Properties = new[] { "DealStage", "OwnerCode" } };
                case EntityName.Order:
                    return new ActionHistoryMetadata { Properties = new[] { "WorkflowStepId", "OwnerCode" } };
                case EntityName.LegalPerson:
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
                case EntityName.Limit:
                    return new ActionHistoryMetadata { Properties = new[] { "StartPeriodDate", "EndPeriodDate", "Amount", "Status" } };
                case EntityName.AdvertisementElement:
                    return new ActionHistoryMetadata
                        {
                            Properties = new[] { "ModifiedBy" }
                        };
                case EntityName.AdvertisementElementStatus:
                    return new ActionHistoryMetadata
                        {
                            Properties = new[] { "Status" }
                        };
                case EntityName.Advertisement:
                    return new ActionHistoryMetadata
                        {
                            Properties = new[] { "ModifiedBy" }
                        };
                case EntityName.Bargain:
                    return new ActionHistoryMetadata
                    {
                        Properties = new[] { "BargainEndDate" }
                    };
            }

            return null;
        }

        private static DownloadFileMetadata GetDownloadFileMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            if (!entityName.IsFileEntity())
            {
                return null;
            }

            return new DownloadFileMetadata();
        }

        private static UploadFileMetadata GetUploadFileMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            if (!entityName.IsFileEntity())
            {
                return null;
            }

            return new UploadFileMetadata();
        }

        private static ActivateMetadata GetActivateMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            if (!entityName.IsDeactivatable())
            {
                return null;
            }

            return new ActivateMetadata();
        }

        private static DeactivateMetadata GetDeactivateMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            if (!entityName.IsDeactivatable())
            {
                return null;
            }

            return new DeactivateMetadata();
        }

        private static DeleteMetadata GetDeleteMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            if (!entityName.IsDeletable())
            {
                return null;
            }

            return new DeleteMetadata();
        }

        private static ModifySimplifiedModelEntityMetadata GetModifySimplifiedModelEntityMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.Category:
                case EntityName.Operation:
                    return null;
            }

            return new ModifySimplifiedModelEntityMetadata();
        }

        private static ModifyBusinessModelEntityMetadata GetModifyBusinessModelEntityMetadata(EntityName[] entityNames)
        {
            var entityName = entityNames.Single();
            switch (entityName)
            {
                case EntityName.FirmAddress:
                case EntityName.FirmContact:
                case EntityName.Lock:
                case EntityName.LockDetail:
                    return null;
            }
            return new ModifyBusinessModelEntityMetadata();
        }

        #endregion
    
    }
}
