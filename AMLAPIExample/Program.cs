using System;
using System.Text.RegularExpressions;

namespace AMLAPIExample
{
    public class Program
    {
        private static AMLAccount amlUser;
        private static readonly  ConsoleColor init = ConsoleColor.White;

        /// <summary>
        /// Validate the input text for Download, and Approve options
        /// for these two options, the user will need to specify both
        /// the id of the pack and the flag for blockchain notarization
        /// </summary>
        /// <param name="input">input text to validate</param>
        /// <returns>true if the user input is valid</returns>
        public static bool IsValidWallet(string input)
        {
            if (input.Length != 42)
                return false;

            Regex regex = new Regex("^0x[0-9a-f]{40}$");
            if (!regex.IsMatch(input.ToLower()))
                return false;
            return true;
        }

        
        public static void Main()
        {
            Console.WriteLine("AML API Example");
            amlUser = new AMLAccount();
            var authorized = amlUser.RequestAccessToken().Result;
            if (authorized)
            {
                Console.WriteLine("Aml user authorised");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Aml user authorization failed, try again!");
                Console.ForegroundColor = init;
                return;
            }

            Console.WriteLine("Please now provide the information for AML search"
                + Environment.NewLine + "Leave blank if certain information is not present");

            // ask for first name (required)
            Console.WriteLine("Please enter first name (required)");
            var fname = Console.ReadLine().Trim();
            while(fname.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("First name required, try again!");
                Console.ForegroundColor = init;
                fname = Console.ReadLine().Trim();
            }
            string lname, country = null, wallet = null;

            // ask for last name (optional)
            Console.WriteLine("Please enter last name (optional)");
            lname = Console.ReadLine().Trim();
            if(lname.Length != 0)
            {
                // ask for country (optional)
                Console.WriteLine("Please enter country (optional)");
                country = Console.ReadLine().Trim();

                if(country.Length != 0)
                {
                    // ask for wallet (optional)
                    Console.WriteLine("Please enter wallet (optional)");
                    wallet = Console.ReadLine().Trim();
                    while (wallet.Length != 0 && !IsValidWallet(wallet))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid required, try again!");
                        Console.ForegroundColor = init;
                        wallet = Console.ReadLine().Trim();
                    }
                }
            }



            // now do the search
            var result = amlUser.Search(fname, lname, country, wallet).Result;
            if(result.Item1)
            {
                // succeed
                Console.WriteLine($"Search result: {result.Item2}");
            }
            else
            {
                // failed
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Item2);
            }

        }
    }
}
