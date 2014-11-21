using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Metadata.Sources
{
    public sealed partial class StandaloneEntitiesMetadataSource
    {
        private readonly HierarchyMetadata _autonomousMetadataMetadata =
               HierarchyMetadata
                   .Config
                   .Id.Is(IdBuilder.For<MetadataEntitiesIdentity>(EntityName.PerformedBusinessOperation.ToString()))
                   .WithFeatures(new RelatedEntityFeature(EntityName.PerformedBusinessOperation))
                   .Childs(
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Id).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Parent).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Operation).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Descriptor).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Context).WithFeatures(new HiddenFeature()),
                        EntityPropertyMetadata.Create<PerformedBusinessOperationDomainEntityDto>(dto => dto.Date).WithFeatures(new HiddenFeature()));
    }
}
