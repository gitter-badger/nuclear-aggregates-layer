﻿using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel
{
    public sealed class ChileLegalPersonReadModel : IChileLegalPersonReadModel
    {
        private readonly IFinder _finder;

        public ChileLegalPersonReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public EntityReference GetCommuneReference(long legalPersonId)
        {
            var legalPerson = _finder.FindOne(Specs.Find.ById<LegalPerson>(legalPersonId));
            var legalPersonPart = legalPerson.Parts.OfType<ChileLegalPersonPart>().SingleOrDefault();

            if (legalPersonPart == null)
            {
                return null;
            }

            var commune = _finder.FindOne(Specs.Find.ById<Commune>(legalPersonPart.CommuneId));
            return new EntityReference(commune.Id, commune.Name);
        }
    }
}
