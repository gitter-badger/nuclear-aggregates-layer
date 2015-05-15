using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

using NuClear.Metamodeling.Domain.Elements.Aspects.Features.Entities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Domain.Entities;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Metadata.Sources
{
    public sealed partial class StandaloneEntitiesMetadataSource
    {
        private readonly HierarchyMetadata _autonomousMetadataMetadata =
               HierarchyMetadata
                   .Config
                   .Id.Is(NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataEntitiesIdentity>(EntityType.Instance.PerformedBusinessOperation().Description))
                   .WithFeatures(new RelatedEntityFeature(EntityType.Instance.PerformedBusinessOperation()))
                   .Childs(
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Id).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Parent).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Operation).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Descriptor).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Context).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Date).WithFeatures(new HiddenFeature()));
    }
}
