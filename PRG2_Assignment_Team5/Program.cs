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
            List<SHNFacility> shnFacilities = new List<SHNFacility>();
            InitSHNFacilityData(shnFacilities);
            InitPersonData(personList, shnFacilities);
            ListVisitors(personList);

        }

        static void InitPersonData(List<Person> pList, List<SHNFacility> shnFacilities) // reads Person csv and create a Person object, can be either visitor or resident
        {
            string[] csvlines = File.ReadAllLines("Person.csv");

            for (int i = 1; i < csvlines.Length; i++) // i starts from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");

                // data conversion and preparation
                // generic data
                string type = data[0];
                string name = data[1];

                // visitors data
                string passportNo = data[4];
                string nationality = data[5];

                // add person to pList, have to do conditions
                if (type == "resident")
                {
                    string address = data[2];
                    string[] leftDateArray = data[3].Split('/');
                    int leftYear = Convert.ToInt32(leftDateArray[2]);
                    int leftMonth = Convert.ToInt32(leftDateArray[1]);
                    int leftDay = Convert.ToInt32(leftDateArray[0]);
                    DateTime lastLeftDate = new DateTime(leftYear, leftMonth, leftDay);
                    Resident rsd = new Resident(name, address, lastLeftDate);
                    pList.Add(rsd);                                                         // should be no problems

                    if (data[9] != "")
                    {
                        rsd.AddTravelEntry(CreateTravelEntry(data));

                        //

                        //if (facilityName != "")
                        //{
                        //    bool found = false;

                        //    foreach (SHNFacility shnFac in shnFacilities)
                        //    {
                        //        if (facilityName == shnFac.FacilityName)
                        //        {
                        //            found = true;
                        //            rsdTravelEntry.AssignSHNFacility(shnFac);
                        //            break;
                        //        }
                        //    }
                            
                        //    if (!found)
                        //    {
                        //        Console.WriteLine("Facility not found.");
                        //    }
                        //}   
                    }
                    
                    // need to add if condition to skip creation of token if is null or ""                   
                    if (data[6] != "")
                    {
                        // for tokens class, partial participation
                        string tokenSn = data[6];
                        string tokenColLocation = data[7];
                        DateTime tokenExpiryDate = Convert.ToDateTime(data[8]);
                        rsd.token = new TraceTogetherToken(tokenSn, tokenColLocation, tokenExpiryDate);
                    }
                }
                else
                {
                    Visitor vst = new Visitor(name, passportNo, nationality);
                    pList.Add(vst);                                                 // should be no problems
                    if (data[9] != "")
                    {
                        vst.AddTravelEntry(CreateTravelEntry(data));
                    }
                }
            }
        }

        static TravelEntry CreateTravelEntry(string[] data)
        {

            // for travel entry class
            string travelEntryLastCountry = data[9];
            string travelEntryMode = data[10];
            string facilityName = data[14];

            string[] entryDate = data[11].Split('/', ' ', ':');
            int entYear = Convert.ToInt32(entryDate[2]);
            int entMonth = Convert.ToInt32(entryDate[1]);
            int entDay = Convert.ToInt32(entryDate[0]);
            int entHour = Convert.ToInt32(entryDate[3]);
            int entMin = Convert.ToInt32(entryDate[4]);
            DateTime travelEntryDate = new DateTime(entYear, entMonth, entDay, entHour, entMin, 0);

            DateTime travelShnEndDate = Convert.ToDateTime(data[12]);
            string travelIsPaid = data[13];
            TravelEntry vstTravelEntry = new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate);
            vstTravelEntry.EntryDate = travelEntryDate;
            vstTravelEntry.ShnEndDate = travelShnEndDate;
            vstTravelEntry.IsPaid = Convert.ToBoolean(travelIsPaid);

            return vstTravelEntry;
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

                    //foreach(SHNFacility shn in shnFacilityList)
                    //{
                    //    Console.WriteLine(shn);
                    //}
                }
            }
        }

        static void ListVisitors(List<Person> pList)
        {
            Console.WriteLine("{0, -10} {1, -10} {2, -10}", "Name", "Passport No", "Nationality");
            foreach(Person p in pList)
            {
                if (p is Visitor)
                {
                    Visitor v = (Visitor) p;
                    Console.WriteLine("{0, -10} {1, -10} {2, -10}", p.Name, v.PassportNo, v.Nationality);
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
