using System;
using System.Collections.Generic;
using System.Text;

namespace PRG2_Assignment_Team5
{
    class TravelEntry
    {
        // Properties
        public string LastCountryOfEmbarkation { get; set; }
        public string EntryMode { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ShnEndDate { get; set; }
        public SHNFacility ShnStay { get; set; }
        public bool IsPaid { get; set; }
    
        // Default Constructor
        public TravelEntry() { }

        // Parameterized Constructor
        public TravelEntry(string lcoe, string em, DateTime ed)
        {
            LastCountryOfEmbarkation = lcoe;
            EntryMode = em;
            EntryDate = ed;
        }

        // Methods
        public void AssignSHNFacility(SHNFacility ShnFac)
        {
            ShnStay = ShnFac;
        }
        // to add
        // 1) AssignSHNFacility(SHNFacility)
        // 2) CalculateSHNDuration()
        // 3) ToString():string
    }
}
