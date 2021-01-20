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

        public List<Person> SafeEntryList = new List<Person>(); //change list type to travelentry and safeentry respectively

        public List<Person> TravelEntryList = new List<Person>(); //change list type to travelentry and safeentry respectively

        public Person() { }

        public Person(string name)
        {
            Name = name;
        }

        public void AddTravelEntry(TravelEntry)
        {

        }

        public void AddSafeEntry(SafeEntry)
        {

        }

        public abstract double CalculateSHNCharges();

        public override string ToString()
        {
            return "Name: " + Name;
        }

    }
}
