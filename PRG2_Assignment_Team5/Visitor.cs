using System;
using System.Collections.Generic;
using System.Text;

namespace PRG2_Assignment_Team5
{
    class Visitor : Person
    {
        public string PassportNo { get; set; }
        public string Nationality { get; set; }

        public Visitor(string n, string ppn, string nlty) : base(n)
        {
            PassportNo = ppn;
            Nationality = nlty;
        }

        public override double CalculateSHNCharges()
        {
            int lastItemIndex = TravelEntryList.Count;
            TravelEntry lastItem = TravelEntryList[lastItemIndex].
        }   
    }
}
