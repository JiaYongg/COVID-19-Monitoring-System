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
    class SHNFacility
    {
        public string FacilityName { get; set; }

        public int FacilityCapacity { get; set; }

        public int FacilityVacancy { get; set; }

        public double DistFromAirCheckpoint { get; set; }

        public double DistFromSeaCheckpoint { get; set; }

        public double DistFromLandCheckpoint { get; set; }

        public SHNFacility() { }

        public SHNFacility(string name, int capacity, double aircp, double seacp, double landcp)
        {
            FacilityName = name;
            FacilityCapacity = capacity;
            DistFromAirCheckpoint = aircp;
            DistFromSeaCheckpoint = seacp;
            DistFromLandCheckpoint = landcp;
        }

        public double CalculateTravelCost(string entrymode, DateTime entrydate)
        {
            if ((entrydate.Hour >= 6 && entrydate.Hour < 9) || (entrydate.Hour >= 18 && entrydate.Hour < 24))
            {
                double extracharge = 0.25;

                if (entrymode == "Air")
                {
                    double baseFare = 50 + DistFromAirCheckpoint * 0.22; //subjected to change  
                    double totalFare = (baseFare * extracharge) + baseFare;
                    return totalFare;
                }
                else if (entrymode == "Sea")
                {
                    double baseFare = 50 + DistFromSeaCheckpoint * 0.22; //subjected to change  
                    double totalFare = (baseFare * extracharge) + baseFare;
                    return totalFare;
                }
                else if (entrymode == "Land")
                {
                    double baseFare = 50 + DistFromLandCheckpoint * 0.22; //subjected to change  
                    double totalFare = (baseFare * extracharge) + baseFare;
                    return totalFare;
                }
                else
                {
                    return -1;
                }
            }
            else if (entrydate.Hour == 0 && entrydate.Hour < 6)
            {
                double extracharge = 0.5;

                if (entrymode == "Air")
                {
                    double baseFare = 50 + DistFromAirCheckpoint * 0.22; //subjected to change  
                    double totalFare = (baseFare * extracharge) + baseFare;
                    return totalFare;
                }
                else if (entrymode == "Sea")
                {
                    double baseFare = 50 + DistFromSeaCheckpoint * 0.22; //subjected to change  
                    double totalFare = (baseFare * extracharge) + baseFare;
                    return totalFare;
                }
                else if (entrymode == "Land")
                {
                    double baseFare = 50 + DistFromLandCheckpoint * 0.22; //subjected to change  
                    double totalFare = (baseFare * extracharge) + baseFare;
                    return totalFare;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (entrymode == "Air")
                {
                    double baseFare = 50 + DistFromAirCheckpoint * 0.22; //subjected to change  
                    return baseFare;
                }
                else if (entrymode == "Sea")
                {
                    double baseFare = 50 + DistFromSeaCheckpoint * 0.22; //subjected to change  
                    return baseFare;
                }
                else if (entrymode == "Land")
                {
                    double baseFare = 50 + DistFromLandCheckpoint * 0.22; //subjected to change  
                    return baseFare;
                }
                else
                {
                    return -1;
                }
            }
        }

        public bool IsAvailable()
        {
            if (FacilityCapacity <= FacilityVacancy)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Facility Name: " + FacilityName + "\tFacility Capacity: " + FacilityCapacity +
                "DistFromAirCheckpoint: " + DistFromAirCheckpoint +
                "DistFromSeaCheckpoint: " + DistFromSeaCheckpoint +
                "DistFromLandCheckpoint: " + DistFromLandCheckpoint;
        }
    }
}
