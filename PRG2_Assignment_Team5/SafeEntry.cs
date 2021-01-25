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
    class SafeEntry
    {
        // properties
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public BusinessLocation Location { get; set; }

        // default constructor
        public SafeEntry() { }
        
        // parameterized constructors
        public SafeEntry(DateTime ci, BusinessLocation loc)
        {
            CheckIn = ci;
            Location = loc;
        }

        // Methods
        public void PerformCheckOut()
        {
            CheckOut = DateTime.Now;
            Location.VisitorsNow -= 1;
        }

        public override string ToString()
        {
            return "Check In: " + CheckIn + "\tCheck Out:" + CheckOut + "\tLocation: " + Location;
        }
    }
}
