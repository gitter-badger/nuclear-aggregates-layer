﻿namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.DTO
{
    public class CheckForDublicatesResultDto
    {
        public bool ActiveDublicateExists { get; set; }
        public bool InactiveDublicateExists { get; set; }
        public bool DeletedDublicateExists { get; set; }
    }
}
