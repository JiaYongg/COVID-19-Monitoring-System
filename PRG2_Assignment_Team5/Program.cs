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
            for (int i = 1; i < csvlines.Length; i++) // i starts from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");

                // generic data
                string type = data[0];
                string name = data[1];
                DateTime travelEntryDate = Convert.ToDateTime(data[11]);
                DateTime travelShnEndDate = Convert.ToDateTime(data[12]);
                bool travelIsPaid = Convert.ToBoolean(data[14]);
                string facilityName = data[15];

                // residents data
                string address = data[2];
                string tokenSn = data[6];
                string tokenColLocation = data[7];
                DateTime tokenExpiryDate = Convert.ToDateTime(data[8]);
                string travelEntryLastCountry = data[9];
                string travelEntryMode = data[10];

                // visitors data
                DateTime lastLeftCountry = Convert.ToDateTime(data[3]);
                string passportNo = data[4];
                string nationality = data[5];

                // add person to pList, have to do conditions

                if (type == "resident")
                {
                    Resident rsd = new Resident(name, address, lastLeftCountry);
                    pList.Add(rsd);                                                 // should be no problems
                    rsd.AddTravelEntry(new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate));
                    // need to add if condition to skip creation of token if is null or ""
                    rsd.token = new TraceTogetherToken(tokenSn, tokenColLocation, tokenExpiryDate);
                }
                else
                {
                    Visitor vst = new Visitor(name, passportNo, nationality);
                    pList.Add(vst);                                                 // should be no problems
                    vst.AddTravelEntry(new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate));
                }
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

        //// Display Menu - to add
        //static void DisplayMenu()
        //{
        //    Console.WriteLine("COVID-19 Monitoring System");

        //}

    }
}
