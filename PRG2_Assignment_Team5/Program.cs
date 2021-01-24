﻿//============================================================
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
            InitBusinessLocationData(businessLocationList);

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
                    DisplayQueryDetails(query, searchP);
                }
                else if (selection == "3")
                {
                    //assign or replace TraceTogetherToken
                    Console.Write("Enter Name of Person to Search: ");
                    string searchP = Console.ReadLine();
                    Person person = SearchPerson(personList, searchP);

                    if (person != null && person is Resident)
                    {
                        Resident r = (Resident) person;
                        if (r.token != null && r.token.IsEligibleForReplacement() == true)
                        {
                            ReplaceTraceTogetherToken(r);
                        }
                        else if (r.token == null)
                        {
                            AssignTraceTogetherToken(r);
                        }
                        else
                        {
                            Console.WriteLine("{0} already owns a TraceTogether Token !", r.Name);
                        }
                    }
                    else if (person != null && person is Visitor)
                    {
                        Console.WriteLine("{0} is a visitor and cannot be assigned with a token.", person.Name);
                    }
                    else
                    {
                        Console.WriteLine("Sorry, {0} could not be found.\n", searchP);
                    }
                }
                else if (selection == "4")
                {
                    DisplayBusinessLocations(businessLocationList);
                }
                else if (selection == "5")
                {
                    EditBusinessLocationCapacity(businessLocationList);
                }

                else if (selection == "8")
                {
                    DisplaySHNFacilities(shnFacilities);
                }
                
                else if (selection == "9")
                {
                    CreateVisitor(personList);
                }

                else if (selection == "10")
                {
                    Console.Write("Enter Name of Person to Create Travel Entry Record: ");
                    string searchName = Console.ReadLine();
                    Person p = SearchPerson(personList, searchName);
                    if (p != null)
                    {
                        CreateTravelEntry(p, shnFacilities);
                        DisplayQueryDetails(p, searchName);
                    }
                    else
                    {
                        Console.WriteLine("{0} does not exist.", searchName);
                    }
                    
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
                        rsd.AddTravelEntry(InitTravelEntry(data, shnList));                            // assign TravelEntry object te by calling InitTravelEntry() method and add to Resident's TravelEntryList. 
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
                        vst.AddTravelEntry(InitTravelEntry(data, shnList));
                    }
                }
            }
        }

        static TravelEntry InitTravelEntry(string[] data, List<SHNFacility> shnList)
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
            string[] csvlines = File.ReadAllLines("BusinessLocation.csv");
            for (int i = 1; i < csvlines.Length; i++) // i start from 1 to remove the header
            {
                string[] data = csvlines[i].Split(",");
                string businessName = data[0];
                string branchCode = data[1];
                int maxCapacity = Convert.ToInt32(data[2]);
                BusinessLocation bl = new BusinessLocation(businessName, branchCode, maxCapacity);
                bl.MaximumCapacity = maxCapacity;
                bList.Add(bl);
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
                    
                    foreach(SHNFacility fac in shnFacilityList)
                    {
                        fac.FacilityVacancy = fac.FacilityCapacity;
                    }
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
                Console.WriteLine("{0} does not have travel entry records.", p.Name);
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

        static void AssignTraceTogetherToken(Resident r)
        {
            Console.WriteLine("\nToken not found ! Follow steps below to assign a TraceTogether Token");
            Console.Write("Enter serial number: ");
            string sn = Console.ReadLine();
            Console.Write("Enter collection location: ");
            string colLocation = Console.ReadLine();
            DateTime expiry = DateTime.Now.AddMonths(6);
            r.token = new TraceTogetherToken(sn, colLocation, expiry);
            Console.WriteLine("Token successfully assigned to {0}!", r.Name);
            Console.WriteLine("{0}: {1}", "Serial Number", r.token.SerialNo);
            Console.WriteLine("{0}: {1}", "Collection Location", r.token.CollectionLocation);
        }

        static void ReplaceTraceTogetherToken(Resident r)
        {
            Console.WriteLine("\nToken found but has expired !");
            Console.Write("Enter new serial number: ");
            string sn = Console.ReadLine();
            Console.Write("Enter collection location: ");
            string colLocation = Console.ReadLine();
            r.token.ReplaceToken(sn, colLocation);
            r.token.ExpiryDate = DateTime.Now.AddMonths(6);
            Console.WriteLine("\nToken has been successfully replaced for {0} !", r.Name);
            Console.WriteLine("{0}: {1}", "New Serial Number", r.token.SerialNo);
            Console.WriteLine("{0}: {1}", "Collection Location", r.token.CollectionLocation);
        }

        static void DisplaySHNFacilities(List<SHNFacility> facilityList)
        {
            // Header for SHN Facilities
            Console.WriteLine("{0, -5} {1, -5} {2, 10} {3, 10} {4, 20} {5, 20} {6, 20} {7, 15}", "Facility ID", "Facility Name", "Capacity", "Vacancy", "Distance from Air", "Distance from Sea", "Distance from Land", "Availability");

            // foreach loop to loop through the facilityList

            for (int i = 0; i < facilityList.Count; i++)
            {
                string displayAvail = "No";
                bool available = facilityList[i].IsAvailable();
                if (available)
                {
                    displayAvail = "Yes";
                }
                Console.WriteLine("[{0, -5}] {1, -20} {2, 10} {3, 10} {4, 20} {5, 20} {6, 20} {7, 15}", i+1, facilityList[i].FacilityName, facilityList[i].FacilityCapacity, facilityList[i].FacilityVacancy, facilityList[i].DistFromAirCheckpoint, facilityList[i].DistFromSeaCheckpoint, facilityList[i].DistFromLandCheckpoint, displayAvail);
            }
        }

        static void DisplayBusinessLocations(List<BusinessLocation> bList)
        {
            Console.WriteLine("{0, -20} {1, 15} {2, 15}", "Business Name", "Branch Code", "Max Capacity");

            foreach (BusinessLocation bl in bList)
            {
                Console.WriteLine("{0, -20} {1, 15} {2, 15}", bl.BusinessName, bl.BranchCode, bl.MaximumCapacity);
            }

        }

        static BusinessLocation SearchBusinessLocation(List<BusinessLocation> bList, string bizcode)
        {
            foreach(BusinessLocation bl in bList)
            {
                if (bl.BranchCode == bizcode)
                {
                    return bl;
                }
            }
            return null;
        }

        static void EditBusinessLocationCapacity(List<BusinessLocation> bList)
        {
            try
            {
                Console.Write("Enter Business branch code: ");
                string bizcode = Console.ReadLine();
                BusinessLocation businessLocation = SearchBusinessLocation(bList, bizcode);

                if (businessLocation != null)
                {
                    Console.WriteLine("Business Location found with a Max Capacity of {0} !", businessLocation.MaximumCapacity);
                    Console.Write("Enter the number of Max Capacity you would like to change it to: ");
                    int maxCap = Convert.ToInt32(Console.ReadLine());

                    businessLocation.MaximumCapacity = maxCap;
                    Console.WriteLine("Maximum Capacity of {0} has been updated !", businessLocation.BusinessName);
                }
                else
                {
                    Console.WriteLine("Sorry, branch code of {0} is not found !", bizcode);
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DisplayQueryDetails(Person query, string searchP)
        {
            if (query != null)
            {
                if (query is Resident)
                {
                    Resident r = (Resident)query;
                    Console.WriteLine("\n{0, -10} {1, -20} {2, -20}", "Name", "Address", "Last Left Country Date");
                    Console.WriteLine("{0, -10} {1, -20} {2, -20}", r.Name, r.Address, r.LastLeftCountry.ToString("dd/MM/yyyy"));

                    PrintTravelEntry(r);
                    PrintSafeEntry(r);
                    DisplayTokenDetails(r);
                }
                else
                {
                    Visitor v = (Visitor) query;
                    Console.WriteLine("\n{0, -10} {1, -10} {2, -25}", "Name", "Passport No.", "Nationality");
                    Console.WriteLine("{0, -10} {1, -10} {2, -25}", v.Name, v.PassportNo, v.Nationality);
                    PrintTravelEntry(v);
                    PrintSafeEntry(v);
                }
            }
            else
            {
                Console.WriteLine("Sorry, {0} could not be found.\n", searchP);
            }
        }

        static void CreateVisitor(List<Person> pList)
        {
            Console.WriteLine("------------- Create new visitor -------------");
            Console.Write("Visitor Name: ");
            string newVisitorName = Console.ReadLine();
            Console.Write("Visitor Passport No.: ");
            string newVisitorPpn = Console.ReadLine();
            Console.Write("Visitor Nationality: ");
            string newVisitorNationality = Console.ReadLine();

            bool found = false;

            foreach (Person p in pList)
            {
                if (p is Resident)
                {
                    continue;
                }
                else
                {
                    Visitor r = (Visitor) p;
                    if (r.PassportNo == newVisitorPpn)
                    {
                        found = true;
                    }
                }
            }

            if (found)
            {
                Console.WriteLine("Visitor {0} with the passport number {1} already exist.", newVisitorPpn, newVisitorPpn);
            }
            else
            {
                pList.Add(new Visitor(newVisitorName, newVisitorPpn, newVisitorNationality));
                Console.WriteLine("Visitor {0} created.");
                ListVisitors(pList);
            }
        }

        static void CreateTravelEntry(Person p, List<SHNFacility> shnLists)
        {
            // have to do:
            // 1. Validation for Entry Mode input (Air/Land/Sea), 

            Console.Write("Last Country of Embarkation: ");
            string lcoe = Console.ReadLine();
            //string entryMode = "";
            Console.Write("Mode of Entry (Air/Land/Sea): ");
            string entryMode = Console.ReadLine();
            //while (entryMode != "air" || entryMode != "land" || entryMode != "sea")
            //{
            //Console.Write("Mode of Entry (Air/Land/Sea): ");
            //entryMode = Console.ReadLine().ToLower();

            //if (entryMode != "air" || entryMode != "land" || entryMode != "sea")
            //{
            //    Console.WriteLine("Entry mode must be either by 'Air', 'Land' or 'Sea'");
            //}
            //}

        Console.Write("Date of Entry (YYYY/MM/dd HH:mm): ");
            DateTime entryDate = Convert.ToDateTime(Console.ReadLine());

            TravelEntry te = new TravelEntry(lcoe, entryMode, entryDate);
            te.CalculateSHNDuration();

            if (te.ShnEndDate.Subtract(te.EntryDate).Days == 14) 
            {
                DisplaySHNFacilities(shnLists);
                try
                {
                    Console.Write("Enter Facility ID to assign {0}: ", p.Name);
                    int facilityIndex = Convert.ToInt32(Console.ReadLine()) - 1;
                    SHNFacility chosenFac = shnLists[facilityIndex];
                    te.AssignSHNFacility(chosenFac);
                }
                catch(IndexOutOfRangeException)
                {
                    Console.WriteLine("Facility ID inputted does not exist!");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Please input the respective ID of the chosen facility in integer");
                }
            }

            te.IsPaid = false;
            p.AddTravelEntry(te);
            Console.WriteLine("Travel Entry record for {0} has been created.", p.Name);

        }

        // Display Menu - to add
        static void DisplayMenu()
        {
            Console.WriteLine("\nCOVID-19 Monitoring System");
            Console.WriteLine("[1]\tView Visitors");
            Console.WriteLine("[2]\tSearch Person");
            Console.WriteLine("[3]\tAssign/Replace TraceTogether Token");
            Console.WriteLine("[4]\tView Business Locations");
            Console.WriteLine("[5]\tEdit Business Location Capacity");
            Console.WriteLine("[8]\tList SHN Facilities");
            Console.WriteLine("[9]\tCreate New Visitor");
            Console.WriteLine("[10]\tCreate Travel Entry Record");
            Console.WriteLine("[0]\tExit");
            Console.WriteLine("---------------------------");
        }

    }
}