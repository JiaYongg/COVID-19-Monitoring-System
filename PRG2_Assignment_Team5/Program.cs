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
            shnFacilities = InitSHNFacilityData(shnFacilities);
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

                else if (selection == "2")
                {
                    Console.Write("Enter Name of Person to Search: ");
                    string searchP = Console.ReadLine();
                    Person query = SearchPerson(personList, searchP);
                    
                    if (query != null)
                    {
                        if (query is Resident)
                        {
                            Resident r = (Resident) query;
                            Console.WriteLine("\n{0, -10} {1, -20} {2, -20}", "Name", "Address", "Last Left Country Date");
                            Console.WriteLine("{0, -10} {1, -20} {2, -15}", r.Name, r.Address, r.LastLeftCountry.ToString("dd/MM/yyyy"));

                            PrintTravelEntry(r);
                            PrintSafeEntry(r);
                            DisplayTokenDetails(r);
                        }
                        else
                        {
                            Visitor v = (Visitor) query;
                            Console.WriteLine("\n{0, -10} {1, -25} ", "Passport No.", "Nationality");
                            Console.WriteLine("{0, -10} {1, -25} ", v.PassportNo, v.Nationality);

                            PrintTravelEntry(v);
                            PrintSafeEntry(v);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, {0} could not be found.\n", searchP);
                    }
                }
                else if (selection == "3")
                {
                    //assign or replace TraceTogetherToken
                    Console.Write("Enter Name of Person to Search: ");
                    string searchP = Console.ReadLine();
                    Person person = SearchPerson(personList, searchP);

                    if (person != null)
                    {
                        if (person is Resident)
                        {
                            Resident r = (Resident) person;
                            if (r.token.SerialNo != "" && r.token.IsEligibleForReplacement() == true)
                            {
                                Console.WriteLine("\nToken found but has expired!");
                                Console.Write("Enter new serial number: ");
                                string sn = Console.ReadLine();
                                Console.Write("Enter collection location: ");
                                string colLocation = Console.ReadLine();
                                r.token.ReplaceToken(sn, colLocation);
                                Console.WriteLine("\nToken has been sucessfully replaced for {0}!", r.Name);
                                Console.WriteLine("{0}: {1}", "New Serial Number", r.token.SerialNo);
                                Console.WriteLine("{0}: {1}", "Collection Location", r.token.CollectionLocation);

                            }
                            else if (r.token.SerialNo == "")
                            {
                                Console.WriteLine("\nToken not found ! Follow steps below to assign a TraceTogether Token");
                                string sn = Console.ReadLine();
                                Console.Write("Enter collection location: ");
                                string colLocation = Console.ReadLine();
                                r.token.SerialNo = sn;
                                r.token.CollectionLocation = colLocation;
                                Console.WriteLine("Token sucessfully assigned to {0}!", r.Name);
                                Console.WriteLine("{0}: {1}", "Serial Number", r.token.SerialNo);
                                Console.WriteLine("{0}: {1}", "Collection Location", r.token.CollectionLocation);
                            }
                            else
                            {
                                Console.WriteLine("{0} already owns a TraceTogether Token !", r.Name);
                            }
                        }
                        else
                        {
                            Console.WriteLine("{0} is a visitor and cannot be assigned with a token.", person.Name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, {0} could not be found.\n", searchP);
                    }
                }

                else if (selection == "8")
                {
                    DisplaySHNFacilities(shnFacilities);
                }

                else if (selection == "0")
                {
                    Console.WriteLine("You have exited the program.");
                    break;
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

            string[] exitDate = data[12].Split('/', ' ', ':');
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
                foreach (SHNFacility facility in shnList)
                {
                    if (facilityName == facility.FacilityName)
                    {
                        personTravelEntry.AssignSHNFacility(facility);
                        break;
                    }
                }
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

        static List<SHNFacility> InitSHNFacilityData(List<SHNFacility> shnFacilityList) // fetches API response and deserialize it to a list
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
                return shnFacilityList;
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
        }

        static Person SearchPerson(List<Person> pList, string name)
        {
            foreach (Person p in pList)
            {
                if (p.Name.ToLower() == name.ToLower())
                {
                    return p;
                }
            }
            return null;
        }

        static void PrintTravelEntry(Person p)
        {
            if (p.TravelEntryList.Count > 0)
            {
                Console.WriteLine("\n--------------------------------------------------- Travel Entry Details ---------------------------------------------------");
                Console.WriteLine("{0, -15} {1, -15} {2, -25} {3, -25} {4, -25} {5, -20}", "Arrived From", "Entry Mode", "Entry Date", "SHN End Date", "SHN Facility Residence", "Payment Status");

                foreach (TravelEntry e in p.TravelEntryList)
                {
                    string facName = "";
                    string paymentStatus = "Unpaid";
                    if (e.ShnStay != null)
                    {
                        facName = e.ShnStay.FacilityName;
                    }
                    if (e.IsPaid)
                    {
                        paymentStatus = "Paid";
                    }
                    Console.WriteLine("{0, -15} {1, -15} {2, -25} {3, -25} {4, -25} {5, -20}\n", e.LastCountryOfEmbarkation, e.EntryMode, e.EntryDate, e.ShnEndDate, facName, paymentStatus);
                }
            }
            else
            {
                Console.WriteLine("{0} does not have entry records.", p.Name);
            }
        }
        
        static void PrintSafeEntry(Person p)
        {
            if (p.SafeEntryList.Count > 0)
            {
                Console.WriteLine("\n-------------------------------------- Safe Entry Details --------------------------------------");
                Console.WriteLine("{0, -15} {1, -15} {2, -25}", "Arrived From", "Entry Mode", "Entry Date");

                foreach (SafeEntry se in p.SafeEntryList)
                {
                    string checkOutTime = se.CheckOut.ToString("dd/MM/yyyy HH:mm:ss");
                    if (se.CheckOut == null)
                    {
                        checkOutTime = "Not checked out";
                    }
                    Console.WriteLine("{0, -15} {1, -15} {2, -25}\n", se.CheckIn.ToString("dd/MM/yyyy HH:mm:ss"), checkOutTime, se.Location.BusinessName);
                }
            }
            else
            {
                Console.WriteLine("{0} does not have any Safe Entry records.", p.Name);
            }
        }

        static void DisplayTokenDetails(Resident r)
        {
            if(r.token != null)
            {
                Console.WriteLine("\n--------------------------------------------------- TraceTogether Token Details ----------------------------------------------");
                string eligibility = "No";
                if (r.token.IsEligibleForReplacement())
                {
                    eligibility = "Yes";
                }
                Console.WriteLine("{0, -15} {1, -25} {2, -25} {3, -15}", "Serial No.", "Collection Location", "Expiry Date", "Replacement Eligiblity");
                Console.WriteLine("{0, -15} {1, -25} {2, -25} {3, -15}", r.token.SerialNo, r.token.CollectionLocation, r.token.ExpiryDate.ToString(), eligibility);
            }
            else
            {
                Console.WriteLine("{0} does not have a token.", r.Name);
            }
        }

        static void DisplaySHNFacilities(List<SHNFacility> facilityList)
        {
            // Header for SHN Facilities
            Console.WriteLine("{0, -20} {1, 10} {2, 10} {3, 20} {4, 20} {5, 20} {6, 15}", "Facility Name", "Capacity", "Vacancy", "Distance from Air", "Distance from Sea", "Distance from Land", "Availability");

            // foreach loop to loop through the facilityList
            foreach (SHNFacility fac in facilityList)
            {
                string displayAvail = "No";
                bool available = fac.IsAvailable();
                if (available)
                {
                    displayAvail = "Yes";
                }
                Console.WriteLine("{0, -20} {1, 10} {2, 10} {3, 20} {4, 20} {5, 20} {6, 15}", fac.FacilityName, fac.FacilityCapacity, fac.FacilityVacancy, fac.DistFromAirCheckpoint, fac.DistFromSeaCheckpoint, fac.DistFromLandCheckpoint, displayAvail);
            }
        }

        // Display Menu - to add
        static void DisplayMenu()
        {
            Console.WriteLine("\nCOVID-19 Monitoring System");
            Console.WriteLine("[1]\tView Visitors");
            Console.WriteLine("[2]\tSearch Person");
            Console.WriteLine("[3]\tAssign/Replace TraceTogether Token");
            Console.WriteLine("[8]\tList SHN Facilities");
            Console.WriteLine("[0]\tExit");
            Console.WriteLine("---------------------------");
        }

    }
}