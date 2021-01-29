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
        public override double CalculateSHNCharges(TravelEntry te)
        {
            int ShnDuration = te.ShnEndDate.Day - te.EntryDate.Day;
            if (te.ShnStay != null)                               // SHNFacility object exists in ShnStay
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

        public override string ToString()
        {
            return base.ToString() + "\tAddress: " + Address + "\tDateTime: " + LastLeftCountry;
        }
    }
}
