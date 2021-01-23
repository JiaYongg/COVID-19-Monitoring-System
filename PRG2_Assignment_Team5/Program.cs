//============================================================
// Student Number : S10208161, S10202579
// Student Name : Jordan Choi, Poh Jia Yong
// Module Group : T01
//============================================================
using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PRG2_Assignment_Team5
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Person> personList = new List<Person>();
            List<BusinessLocation> businessLocationList = new List<BusinessLocation>();
            List<SHNFacility> sHNFacilities = new List<SHNFacility>();
            InitSHNFacilityData(sHNFacilities);

        }

        static void InitPersonData(List<Person> pList) // reads Person csv and create a Person object, can be either visitor or resident
        {
            string[] csvlines = File.ReadAllLines("Person.csv");
            for (int i = 1; i < csvlines.Length; i++) // i starts from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");

                // generic data
                string type = data[0];
                string name = data[1];
                
                // for travel entry class
                string travelEntryLastCountry = data[9];
                string travelEntryMode = data[10];
                DateTime travelEntryDate = Convert.ToDateTime(data[11]);
                DateTime travelShnEndDate = Convert.ToDateTime(data[12]);
                bool travelIsPaid = Convert.ToBoolean(data[14]);
                string facilityName = data[15];
                
                
                // for tokens class, partial participation
                
                string tokenSn = data[6];
                string tokenColLocation = data[7];
                DateTime tokenExpiryDate = Convert.ToDateTime(data[8]);

                // visitors data
                string passportNo = data[4];
                string nationality = data[5];

                // add person to pList, have to do conditions

                if (type == "resident")
                {
                    if (data[2] != "")
                    {
                        string address = data[2];

                        if (data[3] != "")
                        {
                            DateTime lastLeftCountry = Convert.ToDateTime(data[3]);

                            Resident rsd = new Resident(name, address, lastLeftCountry);
                            pList.Add(rsd);                                                 // should be no problems
                            rsd.AddTravelEntry(new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate));
                            // need to add if condition to skip creation of token if is null or ""                   
                            if (tokenSn != "")
                            {
                                rsd.token = new TraceTogetherToken(tokenSn, tokenColLocation, tokenExpiryDate);
                            }
                        }
                        
                    }
                }
                else
                {
                    Visitor vst = new Visitor(name, passportNo, nationality);
                    pList.Add(vst);                                                 // should be no problems
                    vst.AddTravelEntry(new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate));
                }
            }
        }

        static void InitBusinessLocationData(List<BusinessLocation> bList) // reads BusinessLocation csv and creates a BusinessLocation object
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

        static void InitSHNFacilityData(List<SHNFacility> shnFacilityList) // fetches API response and deserialize it to a list
        {
            using (HttpClient client = new HttpClient())
            {
                //GET request
                client.BaseAddress = new Uri("https://covidmonitoringapiprg2.azurewebsites.net/");
                Task<HttpResponseMessage> responseTask = client.GetAsync("facility");
                responseTask.Wait();
                //retrieve response
                HttpResponseMessage result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    Task<string> readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    string data = readTask.Result;
                    shnFacilityList = JsonConvert.DeserializeObject<List<SHNFacility>>(data);

                    foreach(SHNFacility shn in shnFacilityList)
                    {
                        Console.WriteLine(shn);
                    }
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
