using System;
using System.Collections.Generic;
using System.Text;

namespace PRG2_Assignment_Team5
{
    class Resident : Person
    {
        // properties
        public string Address { get; set; }
        public DateTime LastLeftCountry { get; set; }
        public TraceTogetherToken token { get; set; }

        // constructors
        public Resident(string n, string a, DateTime llc) : base(n)
        {
            Address = a;
            LastLeftCountry = llc;
        }

        // methods
        public override double CalculateSHNCharges()
        {
            int lastItemIndex = TravelEntryList.Count - 1;
            TravelEntry lastTravel = TravelEntryList[lastItemIndex];
            int shnDuration = lastTravel.EntryDate.Day - lastTravel.ShnEndDate.Day;

            if (lastTravel.ShnStay != null)
            {
                return (200 + 20 + 1000) * 1.07;
            }
            else if (shnDuration == 7)
            {
                return (200 + 20) * 1.07;
            }
            else
            {
                return 200 * 1.07; 
            }
            
        }

        public override string ToString()
        {
            return base.ToString() + "\tAddress: " + Address + "\tDateTime: " + LastLeftCountry;
        }
    }
}
