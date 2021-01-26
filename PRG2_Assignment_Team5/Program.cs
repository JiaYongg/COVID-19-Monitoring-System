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
            // Establish List to store respective objects - Person (Residents & Visitors), Business Location and SHN Facilities.
            List<Person> personList = new List<Person>();
            List<BusinessLocation> businessLocationList = new List<BusinessLocation>();
            List<SHNFacility> shnFacilities = new List<SHNFacility>();

            // Initialization of People (Residents & Visitors), Business Locations & SHN Facilities
            shnFacilities = InitSHNFacilityData(shnFacilities);
            InitPersonData(personList, shnFacilities);
            InitBusinessLocationData(businessLocationList);

            // While Loop to loop through menu until user enter exit input.
            while (true)
            {
                // Call DisplayMenu() to print menu texts/items.
                DisplayMenu();

                // Request for user input on his desired functions.
                Console.Write("Enter your selection: ");
                string selection = Console.ReadLine();

                // if, else if and else to handle user inputs for the respective application features or functions.
                if (selection == "1")                                                       // Display All Visitors
                {
                    // Call ListVistors(personList) method to print and display all visitors.
                    ListVisitors(personList);
                }
                else if (selection == "2")                                                  // Search particular Person
                {
                    Console.Write("Enter Name of Person to Search: ");
                    string searchP = Console.ReadLine();
                    Person query = SearchPerson(personList, searchP);                       // Call SearchPerson(), returns Person object when found, or null if not found.
                   
                    DisplayQueryDetails(query, searchP);                                    // Call DisplayQueryDetails() to parse person object and display details.
                }
                else if (selection == "3")                                                  // Assign/Replace TraceTogether Token
                {
                    // !Comments by Jordan - may be able to do validation of person type inside method to make main program cleaner.
                    Console.Write("Enter Name of Person to Search: ");
                    string searchP = Console.ReadLine();
                    Person person = SearchPerson(personList, searchP);

                    if (person is Resident)
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
                            Console.WriteLine("\n{0} has already been assigned with a valid TraceTogether Token.", r.Name);
                        }
                    }

                    else if (person is Visitor)
                    {
                        Console.WriteLine("\n{0} is a visitor and cannot be assigned with a token.\n", person.Name);
                    }

                    else
                    {
                        Console.WriteLine("Sorry, {0} could not be found.\n", searchP);
                    }
                    Console.WriteLine("______________________________________________________________\n");
                }
                else if (selection == "4")                                                  // Display the registered Business Locations
                {
                    DisplayBusinessLocations(businessLocationList);
                }
                else if (selection == "5")
                {
                    EditBusinessLocationCapacity(businessLocationList);
                }
                else if (selection == "6")
                {
                    SafeEntryCheckIn(personList, businessLocationList);
                }
                else if (selection == "7")
                {
                    SafeEntryCheckOut(personList);
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
                else if (selection == "11")
                {
                    // Calculate SHN Charges
                }
                else if (selection == "12")
                {
                    // Generate Contact Tracing Report
                }
                else if (selection == "13")
                {
                    // Generate Active SHN Report
                }
                else if (selection == "14")
                {
                    // Reserved for additional features.
                }
                else if (selection == "0")
                {
                    Console.WriteLine("You have exited the program.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid selection, please enter the respective number on the selected feature.\n");
                }
            }
        }

        /* START OF METHODS - General Methods */
        // Display Menu to Print Application Functions for User's Selection
        static void DisplayMenu()
        {
            Console.WriteLine("COVID-19 Monitoring System\n");
            Console.WriteLine("[1]\tView Visitors");
            Console.WriteLine("[2]\tSearch Person");
            Console.WriteLine("[3]\tAssign/Replace TraceTogether Token");
            Console.WriteLine("[4]\tView All Business Locations");
            Console.WriteLine("[5]\tEdit Business Location Capacity");
            Console.WriteLine("[6]\tSafeEntry Check-In");
            Console.WriteLine("[7]\tSafeEntry Check-Out");
            Console.WriteLine("[8]\tList All SHN Facilities");
            Console.WriteLine("[9]\tCreate New Visitor");
            Console.WriteLine("[10]\tCreate New TravelEntry Record");
            Console.WriteLine("[11]\tCalculate SHN Charges");
            Console.WriteLine("[12]\tGenerate Contact Tracing Report");
            Console.WriteLine("[13]\tSHN Status Reporting");
            Console.WriteLine("[0]\tExit");
            Console.WriteLine("______________________________________________________________");
        }

        /* INITIALIZATION METHODS */
        // Method 01 InitPersonData(personList, shnList) - Initialize 'Person' objects method from CSV File. Can be either Residents or Visitors.
        static void InitPersonData(List<Person> pList, List<SHNFacility> shnList)
        {
            string[] csvlines = File.ReadAllLines("Person.csv");
            for (int i = 1; i < csvlines.Length; i++)                               // for loop to loop through csvLines array and i starts from 1 to remove the header.
            {
                string[] data = csvlines[i].Split(",");                             // Split column data by ','.
                /* data conversion and preparation */
                string type = data[0];
                // generic data - all people category (residents & visitor) requires the following.
                string name = data[1];

                if (type == "resident")                                             // If, else to check if 'Person' is Resident or Visitor. Require different handling & constructors.
                {
                    // residents data - only resident require the following
                    string address = data[2];

                    // Retrieve respective date portions by splitting with "/".
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
                        // assign TravelEntry object te by calling InitTravelEntry() method and add to Resident's TravelEntryList. 
                        rsd.AddTravelEntry(InitTravelEntry(data, shnList));     
                    }

                    // if resident has TraceTogetherToken              
                    if (data[6] != "")                                                          
                    {
                        string tokenSn = data[6];
                        string tokenColLocation = data[7];
                        DateTime tokenExpiryDate = Convert.ToDateTime(data[8]);

                        // Create New TraceTogetherToken object and assign to the TraceTogetherToken property in Resident object.
                        rsd.token = new TraceTogetherToken(tokenSn, tokenColLocation, tokenExpiryDate);
                    }
                }
                else                                                                // if person object is Visitor
                {
                    // visitors data - only visitor require the following
                    string passportNo = data[4];
                    string nationality = data[5];
                       
                    // Create new Visitor object
                    Visitor vst = new Visitor(name, passportNo, nationality);
                    pList.Add(vst);

                    // if travel records exist
                    if (data[9] != "")
                    {
                        vst.AddTravelEntry(InitTravelEntry(data, shnList));
                    }
                }
            }
        }
        // ------- End of InitPersonData() method.

        // Initialize 'TravelEntry' objects method - InitPersonData(string[] data, List<SHNFacility> shnList)
        static TravelEntry InitTravelEntry(string[] data, List<SHNFacility> shnList)
        {
            // Data required to be parsed to construct TravelEntry object
            string travelEntryLastCountry = data[9];
            string travelEntryMode = data[10];
            string facilityName = data[14];
            bool travelIsPaid = Convert.ToBoolean(data[13]);

            // Retrieve respective Entry date portions by splitting with "/", " " and ":" for time.
            string[] entryDate = data[11].Split('/', ' ', ':');
            int entYear = Convert.ToInt32(entryDate[2]);
            int entMonth = Convert.ToInt32(entryDate[1]);
            int entDay = Convert.ToInt32(entryDate[0]);
            int entHour = Convert.ToInt32(entryDate[3]);
            int entMin = Convert.ToInt32(entryDate[4]);
            DateTime travelEntryDate = new DateTime(entYear, entMonth, entDay, entHour, entMin, 0);

            // Retrieve respective Exit date portions by splitting with "/", " " and ":" for time.
            string[] exitDate = data[12].Split('/', ' ', ':');
            int exitYear = Convert.ToInt32(exitDate[2]);
            int exitMonth = Convert.ToInt32(exitDate[1]);
            int exitDay = Convert.ToInt32(exitDate[0]);
            int exitHour = Convert.ToInt32(exitDate[3]);
            int exitMin = Convert.ToInt32(exitDate[4]);
            DateTime travelShnEndDate = new DateTime(exitYear, exitMonth, exitDay, exitHour, exitMin, 0);

            // Create new TravelEntry Object
            TravelEntry personTravelEntry = new TravelEntry(travelEntryLastCountry, travelEntryMode, travelEntryDate);

            //personTravelEntry.EntryDate = travelEntryDate;
            personTravelEntry.ShnEndDate = travelShnEndDate;
            personTravelEntry.IsPaid = travelIsPaid;
            
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
                bl.VisitorsNow = 0;
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
            Console.WriteLine("\nAll Visitors\n");
            Console.WriteLine("{0, -10} {1, -15} {2, -15}", "Name", "Passport No.", "Nationality");
            foreach (Person p in pList)
            {
                if (p is Visitor)
                {
                    Visitor v = (Visitor)p;
                    Console.WriteLine("{0, -10} {1, -15} {2, -15}", p.Name, v.PassportNo, v.Nationality);
                }
            }
            Console.WriteLine("______________________________________________________________\n");
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
                Console.WriteLine("\nTravel Entry Details\n");
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
                    Console.WriteLine("{0, -15} {1, -15} {2, -25} {3, -25} {4, -25} {5, -20}\n", e.LastCountryOfEmbarkation, e.EntryMode, e.EntryDate.ToString("dd/MM/yyyy HH:mm"), e.ShnEndDate.ToString("dd/MM/yyyy HH:mm"), facName, paymentStatus);
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
                Console.WriteLine("Safe Entry Details\n");
                Console.WriteLine("{0, -30} {1, -25} {2, -25}", "Checked in at", "Checked out at", "Business Location");

                foreach (SafeEntry se in p.SafeEntryList)
                {
                    
                    if (se.CheckOut == DateTime.MinValue)
                    {
                        string checkOutTime = "Not checked out";
                        Console.WriteLine("{0, -30} {1, -25} {2, -25}\n", se.CheckIn.ToString("dd/MM/yyyy HH:mm:ss"), checkOutTime, se.Location.BusinessName);
                    }
                    else
                    {
                        string checkOutTime = se.CheckOut.ToString("dd/MM/yyyy HH:mm:ss");
                        Console.WriteLine("{0, -30} {1, -25} {2, -25}\n", se.CheckIn.ToString("dd/MM/yyyy HH:mm:ss"), checkOutTime, se.Location.BusinessName);
                    }
                    
                }
            }
            else
            {
                Console.WriteLine("{0} does not have any Safe Entry records.\n", p.Name);
            }
        }

        static void DisplayTokenDetails(Resident r)
        {
            if(r.token != null)
            {
                Console.WriteLine("TraceTogether Token Details\n");
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
            Console.WriteLine("\nAssignment of TraceTogether Token\n");
            Console.WriteLine("No existing token is found for {0}. Follow steps below to assign a TraceTogether Token:\n", r.Name);
            Console.Write("Serial number: ");
            string sn = Console.ReadLine();
            Console.Write("Collection Location: ");
            string colLocation = Console.ReadLine();
            DateTime expiry = DateTime.Now.AddMonths(6);
            r.token = new TraceTogetherToken(sn, colLocation, expiry);
            Console.WriteLine("Token successfully assigned to {0}.\n", r.Name);
            Console.WriteLine("{0}: {1}", "Serial Number", r.token.SerialNo);
            Console.WriteLine("{0}: {1}", "Collection Location", r.token.CollectionLocation);
        }

        static void ReplaceTraceTogetherToken(Resident r)
        {
            Console.WriteLine("\nReplacement of TraceTogether Token.\n");
            Console.WriteLine("An expired token has been found. Eligible for replacement.\n");
            Console.Write("New Serial Number: ");
            string sn = Console.ReadLine();
            Console.Write("Collection Location: ");
            string colLocation = Console.ReadLine();
            r.token.ReplaceToken(sn, colLocation);
            r.token.ExpiryDate = DateTime.Now.AddMonths(6);
            Console.WriteLine("\nToken has successfully been replaced for {0}.", r.Name);
            Console.WriteLine("{0}: {1}", "New Serial Number", r.token.SerialNo);
            Console.WriteLine("{0}: {1}", "Collection Location", r.token.CollectionLocation);
        }

        static void DisplaySHNFacilities(List<SHNFacility> facilityList)
        {
            Console.WriteLine("\nOfficial SHN Facilities\n");

            // Header for SHN Facilities
            Console.WriteLine("{0, -5} {1, -20} {2, 10} {3, 10} {4, 20} {5, 20} {6, 20} {7, 15}", "ID", "Facility Name", "Capacity", "Vacancy", "Distance from Air", "Distance from Sea", "Distance from Land", "Availability");

            for (int i = 0; i < facilityList.Count; i++)                                                   // foreach loop to loop through the facilityList
            {
                string displayAvail = "No";
                bool available = facilityList[i].IsAvailable();
                if (available)
                {
                    displayAvail = "Yes";
                }
                Console.WriteLine("{0, -5} {1, -20} {2, 10} {3, 10} {4, 20} {5, 20} {6, 20} {7, 15}", i+1, facilityList[i].FacilityName, facilityList[i].FacilityCapacity, facilityList[i].FacilityVacancy, facilityList[i].DistFromAirCheckpoint, facilityList[i].DistFromSeaCheckpoint, facilityList[i].DistFromLandCheckpoint, displayAvail);
            }
            Console.WriteLine("______________________________________________________________\n");
        }

        static void DisplayBusinessLocations(List<BusinessLocation> bList)
        {
            Console.WriteLine("\nRegistered Business Locations\n");
            Console.WriteLine("{0, -20} {1, 15} {2, 15} {3, 15}", "Business Name", "Branch Code", "Visitors Now", "Max Capacity");
            foreach (BusinessLocation bl in bList)
            {
                Console.WriteLine("{0, -20} {1, 15} {2, 15} {3, 15}", bl.BusinessName, bl.BranchCode, bl.VisitorsNow, bl.MaximumCapacity);
            }
            Console.WriteLine("______________________________________________________________\n");
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
                Console.Write("Enter Business Branch Code: ");
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
            Console.WriteLine("\nSearch Person\n");
            if (query != null)
            {
                if (query is Resident)
                {
                    Resident r = (Resident)query;
                    Console.WriteLine("{0, -10} {1, -20} {2, -20}", "Name", "Address", "Last Left Country Date");
                    Console.WriteLine("{0, -10} {1, -20} {2, -20}", r.Name, r.Address, r.LastLeftCountry.ToString("dd/MM/yyyy"));

                    PrintTravelEntry(r);
                    PrintSafeEntry(r);
                    DisplayTokenDetails(r);
                }
                else
                {
                    Visitor v = (Visitor) query;
                    Console.WriteLine("{0, -10} {1, -10} {2, -25}", "Name", "Passport No.", "Nationality");
                    Console.WriteLine("{0, -10} {1, -10} {2, -25}", v.Name, v.PassportNo, v.Nationality);
                    PrintTravelEntry(v);
                    PrintSafeEntry(v);
                }
            }
            else
            {
                Console.WriteLine("Sorry, {0} could not be found.", searchP);
            }
            Console.WriteLine("______________________________________________________________\n");
        }

        static void CreateVisitor(List<Person> pList)
        {
            Console.WriteLine("\nAdd New Visitor\n");
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
                Console.WriteLine("\nCreation of Visitor Failed. There is an existing visitor with the passport number: {0}", newVisitorPpn);
                Console.WriteLine("______________________________________________________________\n");
            }
            else
            {
                Person newV = new Visitor(newVisitorName, newVisitorPpn, newVisitorNationality);
                pList.Add(newV);
                Console.WriteLine("\nVisitor {0} has been successfully created.", newV.Name);
                ListVisitors(pList);
            }
        }

        static void CreateTravelEntry(Person p, List<SHNFacility> shnLists)
        {
            Console.Write("Last Country of Embarkation: ");
            string lcoe = Console.ReadLine();
            string entryMode;

            while (true) // to ensure that user only enters the domain of entry mode - air, land and sea.
            {
                Console.Write("Mode of Entry (Air/Land/Sea): ");
                entryMode = Console.ReadLine();

                if (entryMode.ToLower() == "air" || entryMode.ToLower() == "land" || entryMode.ToLower() == "sea")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Entry mode must be either by 'Air', 'Land' or 'Sea'");
                }
            }

            try
            {
                Console.Write("Enter Year of Entry: ");
                int entryYr = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Month of Entry: ");
                int entryMth = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Day of Entry: ");
                int entryDay = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Hour of Entry: ");
                int entryHr = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Minute of Entry: ");
                int entryMin = Convert.ToInt32(Console.ReadLine());
                DateTime entryDate = new DateTime(entryYr, entryMth, entryDay, entryHr, entryMin, 0);
                TravelEntry te = new TravelEntry(lcoe, entryMode, entryDate);
                te.CalculateSHNDuration();
                te.IsPaid = false;
                p.AddTravelEntry(te);

                if (te.ShnEndDate.Subtract(te.EntryDate).Days == 14)
                {
                    while (true)
                    {
                        try
                        {
                            DisplaySHNFacilities(shnLists);
                            Console.Write("Enter Facility ID to assign {0}: ", p.Name);
                            int facilityIndex = Convert.ToInt32(Console.ReadLine()) - 1;
                            SHNFacility chosenFac = shnLists[facilityIndex];

                            if (chosenFac.FacilityVacancy <= 0)
                            {
                                Console.WriteLine("Facility is full, please select another facility.");
                            }
                            else
                            {
                                te.AssignSHNFacility(chosenFac);
                                Console.WriteLine("Travel Entry record for {0} has been created.", p.Name);
                                break;
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Console.WriteLine("Facility ID inputted does not exist!");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Please input the respective ID of the chosen facility in integer");
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Date has not been inputted correctly.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid date inputs. Please enter the numeric form of all date portions.");
            }
        }
        
        static void SafeEntryCheckIn(List<Person> pList, List<BusinessLocation> bList)
        {
            Console.WriteLine("\nSafeEntry Check-In");
            Console.Write("Enter name of person: ");
            string name = Console.ReadLine();
            Person p = SearchPerson(pList, name);

            if (p != null)
            {
                DisplayBusinessLocations(bList);
                Console.Write("\nEnter name of business location to check in: ");
                string businessLocation = Console.ReadLine();
                bool valid = false;

                foreach (BusinessLocation bl in bList)
                {
                    if (bl.BusinessName.ToLower() == businessLocation.ToLower()) // have yet to validate this portion, where a user enter not a correct business location name
                    {
                        valid = true;
                        if (bl.IsFull() == true)
                        {
                            Console.WriteLine("\nUnable to check in, {0} is currently full.", bl.BusinessName);
                        }
                        else
                        {
                            bool checkedIn = false;
                            foreach (SafeEntry seRecord in p.SafeEntryList)
                            {
                                if (seRecord.Location.BusinessName.ToLower() == businessLocation.ToLower() && seRecord.CheckOut == DateTime.MinValue)
                                {
                                    Console.WriteLine("\n{0} is already checked in to {1}. Please perform a checkout first.", p.Name, businessLocation);
                                    checkedIn = true;
                                    break;
                                }
                            }
                            if (!checkedIn)
                            {
                                SafeEntry se = new SafeEntry(DateTime.Now, bl);
                                bl.VisitorsNow += 1;
                                p.AddSafeEntry(se);
                                Console.WriteLine("{0} has successfully been checked in to {1}.", p.Name, bl.BusinessName);
                            }
                        }
                    }
                }
                if (!valid)
                {
                    Console.WriteLine("{0} is not a business location and could not be found.", businessLocation);
                }
            }
            else
            {
                Console.WriteLine("Sorry, {0} could not be found.", name);
            }
            Console.WriteLine("______________________________________________________________\n");
        }
        static void SafeEntryCheckOut(List<Person> pList)
        {
            Console.Write("Enter name of person: ");
            string name = Console.ReadLine();
            Person p = SearchPerson(pList, name);

            if (p != null && p.SafeEntryList.Count != 0)
            {
                Console.WriteLine("{0, -25} {1, -35}", "Location", "Checked in on");
                foreach (SafeEntry se in p.SafeEntryList)
                {
                    Console.WriteLine("{0, -25} {1, -35}", se.Location.BusinessName, se.CheckIn);
                }
                Console.Write("Enter name of business location to check out: ");
                string businessName = Console.ReadLine();

                foreach (SafeEntry se in p.SafeEntryList)
                {
                    if (se.Location.BusinessName.ToLower() == businessName.ToLower())
                    {
                        se.PerformCheckOut();
                        Console.WriteLine("\n{0} has sucessfully checked out from {1} ! ", p.Name, se.Location.BusinessName);
                    }
                    else
                    {
                        Console.WriteLine("\n{0} is not found in {1}'s Safe Entry list ! ", businessName, p.Name);                    
                    }
                    break;
                }
            }
            else if (p != null && p.SafeEntryList.Count == 0)
            {
                Console.WriteLine("{0} currently does not have a SafeEntry Check-In record.", p.Name);
            }
            else
            {
                Console.WriteLine("{0} does not exist.", name);
            }
        }



    }
}