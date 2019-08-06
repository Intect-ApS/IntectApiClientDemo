# GratisalApiClientDemo
Code showcasing how to work with the Gratisal public REST API

## Usage
### Prerequisite
Install NuGet Package
https://www.nuget.org/packages/gratisalapiclientlib

### Credentials
It is needed to have an active account at Gratisal to utilize this Api. The credentials below are only placeholders.

Create a company to optain credentials at [TEST](https://gratisaltest.dk/signup/) or [LIVE](https://app.gratisal.dk/signup/)

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

You can get a full list of Gratisal methods here [TEST](https://api.gratisaltest.dk/swagger/ui/index) or [LIVE](https://api.gratisal.dk/swagger/ui/index)

### Example methods
#### Change companyuser first name
```
// Get list of Company users
var companyUsers = await gratisalClient.CompanyUsers_GetAllCompanyUsersAsync();

// Find user with firstname 'Bob'
var companyUser = companyUsers.FirstOrDefault(x => x.FirstName == "Bob");

// Change firstname
companyUser.FirstName = "James";

// Update firstname i gratisal
var companyUserUpdateResult = await gratisalClient.CompanyUsers_UpdateCompanyUserAsync(companyUser);
```
### Logout
```
// Terminate a session
await gratisalClient.Close();
```
