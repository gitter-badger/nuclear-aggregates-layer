using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

namespace DoubleGis.Erm.Qds.Etl.Transform.EF
{
    public interface IDocsSelector
    {
        IEnumerable<IDoc> ModifyDocuments(IEnumerable<IEntityKey> entities);

        // TODO Возможно, что тип документа, можно утащить в docRelation или в IQdsComponent типа: 
        // docRelation.ForDoc<UserDoc>().Modifier<UserDocsSelector>().LinkPart<User>();
        // Идея: 
        //  docRelation.ForDoc<ClientGridDoc>(new UserDocsSelector())
        //          .LinkPart<Client>((e,d)=>e.Id==d.Id)
        //          .LinkPart<User>((e,d)=>e.Id==d.OwnerCode)
        //          .LinkPart<Territory>((e,d)=>e.Id==d.TerritoryId);
        Type SupportedDocType { get; }
    }
}