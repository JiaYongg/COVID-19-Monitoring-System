using System;
using System.Collections.Generic;
using System.Text;

namespace PRG2_Assignment_Team5
{
    class BusinessLocation
    {
        public string BusinessName { get; set; }

        public string BranchCode { get; set; }

        public int MaximumCapacity { get; set; }

        public int VisitorsNow { get; set; }

        public BusinessLocation() { }

        public BusinessLocation(string bizname, string bizcode, int visitors)
        {
            BusinessName = bizname;
            BranchCode = bizcode;
            VisitorsNow = visitors;
        }

        public bool IsFull()
        {
            if (VisitorsNow >= MaximumCapacity)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Business Name: " + BusinessName +
                "Branch Code: " + BranchCode +
                "Visitors Now: " + VisitorsNow;
        }
    }
}
