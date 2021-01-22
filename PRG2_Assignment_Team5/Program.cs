using System;
using System.IO;
using System.Collections.Generic;

namespace PRG2_Assignment_Team5
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Person> personList = new List<Person>();
            List<BusinessLocation> businessLocationList = new List<BusinessLocation>();

        }

        static void InitPersonData(List<Person> pList)
        {
            string[] csvlines = File.ReadAllLines("Person.csv");
            for (int i = 1; i < csvlines.Length; i++) // i start from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");
                string type = data[0];
                string name = data[1];
                string address = data[2];
                DateTime lastLeftCountry = Convert.ToDateTime(data[3]);
                string passportNo = data[4];
                string nationality = data[5];
                string tokenSn = data[6];
                string tokenColLocation = data[7];
                DateTime tokenExpiryDate = Convert.ToDateTime(data[8]);
                string travelEntryLastCountry = data[9];
                string travelEntryMode = data[10];
                DateTime travelEntryDate = Convert.ToDateTime(data[11]);
                DateTime travelShnEndDate = Convert.ToDateTime(data[12]);
                bool travelIsPaid = Convert.ToBoolean(data[14]);
                string facilityName = data[15];

                // add person to pList, have to do conditions
            }
        }

        static void InitBusinessLocationData(List<BusinessLocation> bList)
        {
            string[] csvlines = File.ReadAllLines("BusinessLocationcsv");
            for (int i = 1; i < csvlines.Length; i++) // i start from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");
                string businessName = data[0];
                string branchCode = data[1];
                int maxCapacity = Convert.ToInt32(data[2]);

                foreach (BusinessLocation bl in bList)
                {
                    bList.Add(new BusinessLocation(businessName, branchCode, (maxCapacity - bl.VisitorsNow)));
                    break;
                }
               
            }
        }

    }
}
