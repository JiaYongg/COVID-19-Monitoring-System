//============================================================
// Student Number : S10208161, S10202579
// Student Name : Jordan Choi, Poh Jia Yong
// Module Group : T01
//============================================================

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
            ShnStay.FacilityVacancy -= 1;
        }

        public void CalculateSHNDuration()
        {
            if (LastCountryOfEmbarkation == "New Zealand" || LastCountryOfEmbarkation == "Vietnam")
            {
                ShnEndDate = EntryDate.AddDays(0);
            }
            else if (LastCountryOfEmbarkation == "Macao SAR")
            {
                ShnEndDate = EntryDate.AddDays(7);
            }
            else
            {
                ShnEndDate = EntryDate.AddDays(14);
            }
        }

        public override string ToString()
        {
            return "Last Country of Embarkation: " + LastCountryOfEmbarkation + "\tEntry Mode: " + EntryMode + "\tEntry Date" + EntryDate;
            // may need to add ShnEndDate, ShnStay, IsPaid
        }
    }
}
