using System;
using System.Linq;
using System.Threading.Tasks;

namespace GratisalApiClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DemoCode().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task DemoCode()
        {
            gratisalapiclientlib.GratisalClient gratisalClient;

            // Authorizing credentials
            try
            {
                // Credentials. You can create a user to optain credentials at TEST: https://gratisaltest.dk/signup/ or LIVE: https://app.gratisal.dk/signup/
                var credentials = new gratisalapiclientlib.Models.Credentials()
                {
                    Username = "MyUsername",
                    Password = "MySecretPassw0rd",
                };

                // Initialize session
                // The GratisalClient defaults to the testversion of the GratisalApi. Set baseUrl to https://api.gratisal.dk to reach live version
                gratisalClient = new gratisalapiclientlib.GratisalClient(credentials/*,"https://api.gratisal.dk"*/);
            }
            catch (Exception ex)
            {
                throw new Exception("Error authorizing credentials", ex);
            }

            // You can get a full list of Gratisal methods at TEST: https://api.gratisaltest.dk/swagger/ui/index or LIVE: https://api.gratisal.dk/swagger/ui/index

            // ***********************
            // *** EXAMPLE METHODS ***
            // ***********************

            #region * Change companyuser first name *

            // Get list of Company users
            var companyUsers = await gratisalClient.CompanyUsers_GetAllCompanyUsersAsync();

            // Find user with firstname 'Bob'
            var companyUser = companyUsers.FirstOrDefault(x => x.FirstName == "Bob");

            // Change firstname
            companyUser.FirstName = "James";

            // Update firstname i gratisal
            var companyUserUpdateResult = await gratisalClient.CompanyUsers_UpdateCompanyUserAsync(companyUser);

            #endregion

            // Terminate a session
            await gratisalClient.Close();
        }
    }
}
