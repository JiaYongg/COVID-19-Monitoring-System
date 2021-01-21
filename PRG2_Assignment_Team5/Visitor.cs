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
            int lastItemIndex = TravelEntryList.Count - 1;
            TravelEntry lastTravel = TravelEntryList[lastItemIndex];

            if (lastTravel.ShnStay != null)
            {
                double travelCost = lastTravel.ShnStay.CalculateTravelCost(lastTravel.EntryMode, lastTravel.EntryDate);
                return travelCost + 200 + 2000;
            }
            else
            {
                return 200 + 80;
            }
        }

        public override string ToString()
        {
            return base.ToString() + "\tPassport No:" + PassportNo + "\tNationality: " + Nationality + "SHN Charges: " + CalculateSHNCharges();
        }
    }
}
