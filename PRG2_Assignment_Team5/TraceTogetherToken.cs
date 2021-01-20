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
    class TraceTogetherToken
    {
        public string SerialNo { get; set; }

        public string CollectionLocation { get; set; }

        public DateTime ExpiryDate { get; set; }

        public TraceTogetherToken() { }

        public TraceTogetherToken(string sn, string collocation, DateTime expiry)
        {
            SerialNo = sn;
            CollectionLocation = collocation;
            ExpiryDate = expiry;
        }

        public bool IsEligibleForReplacement()
        {
            DateTime currentDate = DateTime.Now;

            if (ExpiryDate.Month - currentDate.Month <= 1)
            {
                if (ExpiryDate.Day < currentDate.Day)
                {
                    return true;
                }
            }
            return false;
        }

        public void ReplaceToken(string sn, string collocation)
        {
            Console.Write("Enter Serial Number: ");
            sn = Console.ReadLine();
            SerialNo = sn;

            Console.Write("Enter Collection Location: ");
            collocation = Console.ReadLine();
            CollectionLocation = collocation;
        }
    }
}
