# GratisalApiClientDemo
Code showcasing how to work with the Gratisal public REST API

## Usage

<details><summary><strong>Simple</strong> (Using NuGet Package)</summary>
<p>

### Prerequisite
Install NuGet Package
https://www.nuget.org/packages/gratisalapiclientlib

### Credentials
It is needed to have an active account at Gratisal to utilize this Api. The credentials below are only placeholders.

Create a company/user to optain credentials at [TEST_Sign_up](https://gratisaltest.dk/signup/) or [LIVE_Sign_up](https://app.gratisal.dk/signup/)

Initialize the GratisalClient:
```
var credentials = new gratisalapiclientlib.Models.Credentials()
{
    Username = "MyUsername",
    Password = "MySecretPassw0rd",            
};

// The GratisalClient defaults to the testversion of the GratisalApi. Set baseUrl to https://api.gratisal.dk to reach live version
var gratisalClient = new gratisalapiclientlib.GratisalClient(credentials/*,"https://api.gratisal.dk"*/);
```

To get a full list of the available GratisalApi methods, go to [TEST_GratisalDK.WebAPI](https://api.gratisaltest.dk/swagger/ui/index) or [LIVE_GratisalDK.WebAPI](https://api.gratisal.dk/swagger/ui/index)

### Example methods (More to be found in the code)
#### Change companyuser first name
```
// Get list of Company users
var companyUsers = await gratisalClient.CompanyUsers_GetAllCompanyUsersAsync();

// Find user with firstname 'Bob'
var companyUser = companyUsers.FirstOrDefault(x => x.FirstName == "Bob");

// Change firstname
companyUser.FirstName = "James";

// Update firstname in Gratisal
var companyUserUpdateResult = await gratisalClient.CompanyUsers_UpdateCompanyUserAsync(companyUser);
```
#### Logout
```
// Terminate the session
await gratisalClient.Close();
```

</p>
</details>


