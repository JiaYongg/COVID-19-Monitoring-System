//============================================================
// Student Number : S10208161, S10202579
// Student Name : Jordan Choi, Poh Jia Yong
// Module Group : T01
//============================================================

using System;

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
            for (int i = 0; i < TravelEntryList.Count; i++)                     // assuming that there can only be 1 unpaid shn bill at any one time
            {
                if (TravelEntryList[i].IsPaid == false && TravelEntryList[i].ShnEndDate < DateTime.Now)
                {xwxw
                    TravelEntry unpaidTe = TravelEntryList[i];
                    if (unpaidTe.ShnStay != null)                               // SHNFacility object exists in ShnStay
                    {
                        double travelCost = unpaidTe.ShnStay.CalculateTravelCost(unpaidTe.EntryMode, unpaidTe.EntryDate);
                        return (200 + travelCost + 2000) * 1.07;
                    }
                    else
                    {
                        return (200 + 80) * 1.07;
                    }
                }
            }
            return -1;
        }

        public override string ToString()
        {
            return base.ToString() + "\tPassport No:" + PassportNo + "\tNationality: " + Nationality + "SHN Charges: " + CalculateSHNCharges();
        }
    }
}
