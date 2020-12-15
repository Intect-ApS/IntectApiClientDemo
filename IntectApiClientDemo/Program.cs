using System;
using System.Linq;
using System.Threading.Tasks;
using static IntectApiClientDemo.Helpers;

namespace IntectApiClientDemo
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
            intectapiclientlib.IntectClient intectClient;

            "* Authorizing credentials *".Log();
            try
            {
                // Credentials. You can create a user to optain credentials at TEST: https://testintect.app/signup/ or LIVE: https://app.intect.app/signup/
                var credentials = new intectapiclientlib.Models.Credentials()
                {
                    Username = "rja@ecitservices.dk",
                    Password = "P4ssw0rd",
                };

                // Initialize session
                // The IntectClient defaults to the testversion of the IntectApi. Set baseUrl to https://api.intect.app to reach live version
                intectClient = new intectapiclientlib.IntectClient(credentials/*,"https://api.intect.app"*/);
            }
            catch (Exception ex)
            {
                throw new Exception("Error authorizing credentials", ex);
            }

            "Done authorizing credentials\n".Log();

            // You can get a full list of Intect methods at TEST: https://api.testintect.app/swagger/ui/index or LIVE: https://api.intect.dk/swagger/ui/index

            // ***********************
            // *** EXAMPLE METHODS ***
            // ***********************
            
            #region * Initialize data *
            "* Initialize data *".Log();

            "Get available companies".Log();
            var companyInfo = await intectClient.Users_GetCompaniesAsync();

            "Get available countries".Log();
            var countries = await intectClient.StaticData_GetCountriesAsync();

            "Get available roles".Log();
            var roles = await intectClient.StaticData_GetRolesAsync();

            "Get available employment templates".Log();
            var employmentTemplates = await intectClient.EmploymentTemplates_GetEmploymentTemplatesAsync();

            "Get available languages".Log();
            var languages = await intectClient.Miscellaneous_GetLanguagesAsync();

            "Get available taxcard types".Log();
            var taxcardTypes = await intectClient.StaticData_GetTaxCardTypesAsync();

            "Get available timeentrytypes".Log();
            var timeEntryTypes = await intectClient.TimeEntryTypes_GetTimeEntryTypeViewsAsync();

            "Get available unittypes".Log();
            var unitTypes = await intectClient.StaticData_GetUnitTypesAsync();

            "Get available statuses".Log();
            var statuses = await intectClient.StaticData_GetTimeEntryStatusesAsync();

            "Done initializing data\n".Log();

            #endregion

            #region * Create companyuser *
            "* Create companyuser *".Log();

            "Create AddUserToCompanyRequest object".Log();
            var addUserToCompanyRequest = new Intect.WebAPI.Client.AddUserToCompanyRequest()
            {
                Details = new Intect.WebAPI.Client.CompanyUser()
                {
                    Address = new Intect.WebAPI.Client.Address()
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
                EmploymentTemplateId = employmentTemplates.First(x => x.Name == "Timelønnet").Id,
                HireDate = DateTime.Now,
                IdentityNumber = GenerateRandomIdentitynumber(Gender.Male), // CPR number
                LanguageId = languages.First(x => x.Code == "DK").Id,
                TaxCardTypeId = taxcardTypes.First(x => x.Key == "Primary").Id,
                Title = "Employee",
            };

            "Create Company User".Log();
            var addUserToCompanyResponse = await intectClient.CompanyUsers_AddUserToCompanyAsync(addUserToCompanyRequest);

            "Done creating CompanyUser\n".Log();

            #endregion

            #region * Change companyuser middle name *
            "* Change companyuser middle name *".Log();

            "Use created user from 'Create companyuser'".Log();
            var companyUser = addUserToCompanyResponse.CompanyUser;

            if (companyUser != null)
            {
                // Try not to use the Update functions to update single or few fields, use patch instead
                // This specific api method doesn't support patch
                 
                "Change middelname".Log();
                companyUser.MiddleName = "Kirby";

                "Update middelname in intect".Log();
                var companyUserUpdateResult = await intectClient.CompanyUsers_UpdateCompanyUserAsync(companyUser);
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
            var timeEntryRecordRequest = new Intect.WebAPI.Client.TimeEntryRecord()
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
            var timeEntryRecordResponse = await intectClient.TimeEntry_CreateTimeEntryRecordAsync(timeEntryRecordRequest);

            "Done creating TimeEntryRecord\n".Log();

            #endregion

            #region * Patch just added timeregistration
            "* Patch timeregistration".Log();

            "Create TimeEntryRecord patch object".Log();
            var timeEntryPatchRequest = new Intect.WebAPI.Client.TimeEntryRecord
            {
                Id = timeEntryRecordResponse.Id,
                StartTime = "",
                EndTime = "",
                Units = new Random().Next(0,1000)/100M,
                Description = "Patching rules"
            };

            "Patch TimeEntryRecord".Log();
            var timeEntryPatchResponse = await intectClient.TimeEntry_PatchTimeEntryRecordAsync(timeEntryPatchRequest);

            "Done patching TimeEntryRecord\n".Log();

            #endregion

            #region * Delete companyusers named Jack *
            "* Delete companyusers named Jack *".Log();

            "Get all company users".Log();
            var companyUsers = await intectClient.CompanyUsers_GetAllCompanyUsersAsync();

            "Find all users called Jack".Log();
            var selectedCompanyUsers = companyUsers.Where(x => x.FirstName == "Jack");

            $"Delete {selectedCompanyUsers.Count()} companyusers. Only companyusers without payslips can be deleted".Log();
            foreach (var selectedCompanyUser in selectedCompanyUsers)
            {
                $"Deleting ({selectedCompanyUser.UserIdentityNumber}) {selectedCompanyUser.FirstName} {selectedCompanyUser.MiddleName} {selectedCompanyUser.LastName} from CompanyUsers".Log();
                await intectClient.CompanyUsers_DeleteCompanyUserAsync(selectedCompanyUser.Id ?? 0);
            }

            "Done deleting companyusers named Jack\n".Log();

            #endregion

            "Terminate the session".Log();
            await intectClient.Close();
        }
    }
}
