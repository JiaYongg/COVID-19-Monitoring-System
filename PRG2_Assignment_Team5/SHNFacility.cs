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

        public double CalculateTravelCost(string em, DateTime date)
        {
            
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
