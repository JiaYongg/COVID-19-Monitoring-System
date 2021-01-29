//============================================================
// Student Number : S10202579, S10208161
// Student Name : Poh Jia Yong, Choi Shu Yih, Jordan
// Module Group : T01
//============================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace PRG2_Assignment_Team5
{
    abstract class Person
    {
        public string Name { get; set; }

        public List<SafeEntry> SafeEntryList = new List<SafeEntry>();

        public List<TravelEntry> TravelEntryList = new List<TravelEntry>();

        public Person() { }

        public Person(string name)
        {
            Name = name;
        }

        public void AddTravelEntry(TravelEntry travelEntry)
        {
            TravelEntryList.Add(travelEntry);
        }

        public void AddSafeEntry(SafeEntry safeEntry)
        {
            SafeEntryList.Add(safeEntry);
        }

        public abstract double CalculateSHNCharges(TravelEntry te);

        public override string ToString()
        {
            return "Name: " + Name;
        }

    }
}
