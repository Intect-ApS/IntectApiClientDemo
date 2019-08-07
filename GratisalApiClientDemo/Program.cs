using System;
using System.Linq;
using System.Threading.Tasks;
using static GratisalApiClientDemo.Helpers;

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
                $"{ex.Message}".Log();
            }
        }

        private static async Task DemoCode()
        {
            gratisalapiclientlib.GratisalClient gratisalClient;

            "* Authorizing credentials *".Log();
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

            "Done authorizing credentials\n".Log();

            // You can get a full list of Gratisal methods at TEST: https://api.gratisaltest.dk/swagger/ui/index or LIVE: https://api.gratisal.dk/swagger/ui/index

            // ***********************
            // *** EXAMPLE METHODS ***
            // ***********************

            #region * Initialize data *
            "* Initialize data *".Log();

            "Get available companies".Log();
            var companyInfo = await gratisalClient.Users_GetCompaniesAsync();

            "Get available countries".Log();
            var countries = await gratisalClient.StaticData_GetCountriesAsync();

            "Get available roles".Log();
            var roles = await gratisalClient.StaticData_GetRolesAsync();

            "Get available employment templates".Log();
            var employmentTemplates = await gratisalClient.EmploymentTemplates_GetEmploymentTemplatesAsync();

            "Get available languages".Log();
            var languages = await gratisalClient.Miscellaneous_GetLanguagesAsync();

            "Get available taxcard types".Log();
            var taxcardTypes = await gratisalClient.StaticData_GetTaxCardTypesAsync();

            "Get available timeentrytypes".Log();
            var timeEntryTypes = await gratisalClient.TimeEntryTypes_GetTimeEntryTypeViewsAsync();

            "Get available unittypes".Log();
            var unitTypes = await gratisalClient.StaticData_GetUnitTypesAsync();

            "Get available statuses".Log();
            var statuses = await gratisalClient.StaticData_GetTimeEntryStatusesAsync();

            "Done initializing data\n".Log();

            #endregion

            #region * Create companyuser *
            "* Create companyuser *".Log();

            "Create AddUserToCompanyRequest object".Log();
            var addUserToCompanyRequest = new Gratisal.WebAPI.Client.AddUserToCompanyRequest()
            {
                Details = new Gratisal.WebAPI.Client.CompanyUser()
                {
                    Address = new Gratisal.WebAPI.Client.Address()
                    {
                        CountryId = countries.First(x => x.Key == "DK").Id,
                        Line1 = "MyStreet 2",
                        Line2 = "1. floor to the right",
                        PostalCode = "3344",
                    },
                    PersonalEmail = "js@caribbean.ky",
                    CompanyId = companyInfo.First().CompanyId,
                    FirstName = "Jack",
                    MiddleName = "Teague",
                    LastName = "Sparrow",
                    IsActive = true,
                    RoleId = roles.First(x => x.Key == "Employee").Id,
                    TelFixed = "+345 1234 5678",
                    TelMobile = "+345 8765 4321",
                },
                EmploymentTemplateId = employmentTemplates.First(x => x.Name == "HourlyPaid").Id,
                HireDate = DateTime.Now,
                IdentityNumber = GenerateRandomIdentitynumber(Gender.Male), // CPR number
                LanguageId = languages.First(x => x.Code == "DK").Id,
                TaxCardTypeId = taxcardTypes.First(x => x.Key == "Primary").Id,
                Title = "Employee",
            };

            "Create Company User".Log();
            var addUserToCompanyResponse = await gratisalClient.CompanyUsers_AddUserToCompanyAsync(addUserToCompanyRequest);

            "Done creating CompanyUser\n".Log();

            #endregion

            #region * Change companyuser middle name *
            "* Change companyuser middle name *".Log();

            "Use created user from 'Create companyuser'".Log();
            var companyUser = addUserToCompanyResponse.CompanyUser;

            if (companyUser != null)
            {
                "Change middelname".Log();
                companyUser.MiddleName = "Kirby";

                "Update firstname i gratisal".Log();
                var companyUserUpdateResult = await gratisalClient.CompanyUsers_UpdateCompanyUserAsync(companyUser);
            }
            else
            {
                "Not found".Log();
            }

            "Done changing companyuser middel name\n".Log();

            #endregion

            #region * Add timeregistration to companyuser *
            "* Add timeregistration to companyuser *".Log();

            "Reusing companyUsers from 'Create companyuser' example".Log();
            companyUser = addUserToCompanyResponse.CompanyUser;

            "Create TimeEntryRecord object".Log();
            var timeEntryRecordRequest = new Gratisal.WebAPI.Client.TimeEntryRecord()
            {
                Description = "\"Business\" spoils",
                StartTime = "08:00",
                EndTime = "12:30",
                TimeEntryTypeId = timeEntryTypes.First(x => x.Identifier == "Work").TimeEntryTypeId,
                EntryDate = DateTime.Now,
                UserEmploymentId = companyUser.UserEmployments.First().Id,
                UnitTypeId = unitTypes.First(x => x.Key == "Hours").Id,
                StatusId = statuses.First(x => x.Key == "Open").Id,
            };

            "Create TimeEntryRecord".Log();
            var timeEntryRecordResponse = await gratisalClient.TimeEntry_CreateTimeEntryRecordAsync(timeEntryRecordRequest);

            "Done creating TimeEntryRecord\n".Log();

            #endregion

            #region * Delete companyusers named Jack *
            "* Delete companyusers named Jack *".Log();

            "Get all company users".Log();
            var companyUsers = await gratisalClient.CompanyUsers_GetAllCompanyUsersAsync();

            "Find all users called Jack".Log();
            var selectedCompanyUsers = companyUsers.Where(x => x.FirstName == "Jack");

            $"Delete {selectedCompanyUsers.Count()} companyusers. Only companyusers without payslips can be deleted".Log();
            foreach (var selectedCompanyUser in selectedCompanyUsers)
            {
                $"Deleting ({selectedCompanyUser.UserIdentityNumber}) {selectedCompanyUser.FirstName} {selectedCompanyUser.MiddleName} {selectedCompanyUser.LastName} from CompanyUsers".Log();
                await gratisalClient.CompanyUsers_DeleteCompanyUserAsync(selectedCompanyUser.Id ?? 0);
            }

            "Done deleting companyusers named Jack\n".Log();

            #endregion

            "Terminate the session".Log();
            await gratisalClient.Close();
        }
    }
}
