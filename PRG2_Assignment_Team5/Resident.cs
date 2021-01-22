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
            for (int i = 0; i < TravelEntryList.Count; i++)                     // assuming that there can only be 1 unpaid shn bill at any one time
            {
                if (TravelEntryList[i].IsPaid == false && TravelEntryList[i].ShnEndDate < DateTime.Now)
                {
                    TravelEntry unpaidTe = TravelEntryList[i];
                    int ShnDuration = unpaidTe.ShnEndDate.Day - unpaidTe.EntryDate.Day;

                    if (unpaidTe.ShnStay != null)                               // SHNFacility object exists in ShnStay
                    {
                        return (200 + 20 + 1000) * 1.07;
                    }
                    else if (ShnDuration == 7)
                    {
                        return (200 + 20) * 1.07;
                    }
                    else
                    {
                        return (200 * 1.07);
                    }
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return base.ToString() + "\tAddress: " + Address + "\tDateTime: " + LastLeftCountry;
        }
    }
}
