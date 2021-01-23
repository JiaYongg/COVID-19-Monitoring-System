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

            while (true)
            {
                DisplayMenu();
                Console.Write("Enter your selection: ");
                string selection = Console.ReadLine();
                
                if (selection == "1")
                {
                    ListVisitors(personList);
                }
            }
        }

        static void InitPersonData(List<Person> pList, List<SHNFacility> shnList)         // reads Person csv and create a Person object, can be either visitor or resident
        {
            string[] csvlines = File.ReadAllLines("Person.csv");

            for (int i = 1; i < csvlines.Length; i++) // i starts from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");

                // data conversion and preparation
                string type = data[0];

                // generic data - all people category (residents & visitor) requires the following.
                string name = data[1];

                if (type == "resident")
                {
                    // residents data - only resident require the following
                    string address = data[2];

                    // to retrieve respective date parts
                    string[] leftDateArray = data[3].Split('/');
                    int leftYear = Convert.ToInt32(leftDateArray[2]);
                    int leftMonth = Convert.ToInt32(leftDateArray[1]);
                    int leftDay = Convert.ToInt32(leftDateArray[0]);
                    DateTime lastLeftDate = new DateTime(leftYear, leftMonth, leftDay);

                    // Create new Resident object
                    Resident rsd = new Resident(name, address, lastLeftDate);
                    pList.Add(rsd);

                    // if travel records exist
                    if (data[9] != "")
                    {                           
                        rsd.AddTravelEntry(CreateTravelEntry(data, shnList));                            // assign TravelEntry object te by calling CreateTravelEntry() method and add to Resident's TravelEntryList. 
                    }

                    // if resident has TraceTogetherToken              
                    if (data[6] != "")                                                          
                    {
                        string tokenSn = data[6];
                        string tokenColLocation = data[7];
                        DateTime tokenExpiryDate = Convert.ToDateTime(data[8]);
                        rsd.token = new TraceTogetherToken(tokenSn, tokenColLocation, tokenExpiryDate);
                    }
                }
                else
                {
                    // visitors data - only visitor require the following
                    string passportNo = data[4];
                    string nationality = data[5];

                    Visitor vst = new Visitor(name, passportNo, nationality);
                    pList.Add(vst);                                                 // should be no problems
                    if (data[9] != "")
                    {
                        vst.AddTravelEntry(CreateTravelEntry(data, shnList));
                    }
                }
            }
        }

        static TravelEntry CreateTravelEntry(string[] data, List<SHNFacility> shnList)
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

            string[] exitDate = data[11].Split('/', ' ', ':');
            int exitYear = Convert.ToInt32(exitDate[2]);
            int exitMonth = Convert.ToInt32(exitDate[1]);
            int exitDay = Convert.ToInt32(exitDate[0]);
            int exitHour = Convert.ToInt32(exitDate[3]);
            int exitMin = Convert.ToInt32(exitDate[4]);

            DateTime travelShnEndDate = new DateTime(exitYear, exitMonth, exitDay, exitHour, exitMin, 0);

            string travelIsPaid = data[13];
            TravelEntry personTravelEntry = new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate);
            personTravelEntry.EntryDate = travelEntryDate;
            personTravelEntry.ShnEndDate = travelShnEndDate;
            personTravelEntry.IsPaid = Convert.ToBoolean(travelIsPaid);

            // if resident stayed in a shnFacility
            if (facilityName != "")
            {
                //bool found = false;
                foreach (SHNFacility facility in shnList)
                {
                    if (facilityName == facility.FacilityName)
                    {
                        //found = true;
                        personTravelEntry.AssignSHNFacility(facility);
                        break;
                    }
                }
                //if (found)
                //{
                //    Console.WriteLine("{0} successfully assigned to {1}", data[1], personTravelEntry.ShnStay.FacilityName);
                //}
                //else
                //{
                //    Console.WriteLine("Facility not found!");
                //}
            }

            return personTravelEntry;
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
                }
            }
        }

        static void ListVisitors(List<Person> pList)
        {
            Console.WriteLine("\n{0, -10} {1, -15} {2, -15}", "Name", "Passport No.", "Nationality");
            foreach (Person p in pList)
            {
                if (p is Visitor)
                {
                    Visitor v = (Visitor)p;
                    Console.WriteLine("{0, -10} {1, -15} {2, -15}", p.Name, v.PassportNo, v.Nationality);
                }
            }
            Console.WriteLine("\n");
        }

        // Display Menu - to add
        static void DisplayMenu()
        {
            Console.WriteLine("COVID-19 Monitoring System");
            Console.WriteLine("[1]\tView Visitors");
            Console.WriteLine("[2]\tSearch Person");
            Console.WriteLine("---------------------------");
        }

    }
}


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