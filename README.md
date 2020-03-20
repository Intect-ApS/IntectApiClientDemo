# GratisalApiClientDemo
Code showcasing how to work with the Gratisal public REST API

## Usage

We provide a test environment which can be accessed here:

<strong>Create company:</strong> https://gratisaltest.dk/signup<br/>
<strong>Interface:</strong> https://gratisaltest.dk<br/>
<strong>API:</strong> https://api.gratisaltest.dk<br/>

This environment works an isolated “sandbox” where emails, payments and other data are not transferred to the intended receipients.
The only exception is integrations to external systems – these are handled “without question”, meaning that if you set up an integration, you must make sure it does not connect to a production environment in the external system.

All e-mails are dispatched to an internal test mailbox, from which we can retrieve them for you upon request if relevant. When signing up a new company, we recommend that you choose a password directly in the form, which will allow you to log in immediately without the need to receive an e-mail with a password.


### Getting started

<details><summary><strong>Simple</strong> (Using NuGet Package)</summary>
<p>

### Prerequisite
Install NuGet Package
https://www.nuget.org/packages/gratisalapiclientlib

### Credentials
It is needed to have an active account at Gratisal to utilize this Api. The credentials below are only placeholders.

Create a company/user to optain credentials at [TEST_Sign_up](https://gratisaltest.dk/signup/)

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

To get a full list of the available GratisalApi methods, go to [TEST_GratisalDK.WebAPI](https://api.gratisaltest.dk/swagger/ui/index)

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

<details><summary><strong>Advanced</strong> (Accessing the Open Gratisal Web Api directly)</summary>
<p>

Our API documentation can be accessed through Swagger by navigating to the root URL of the API - i.e. https://api.gratisaltest.dk or https://api.gratisal.dk.

You can also fetch the documentation in a more raw format by adding /Documentation.xml to the URL - i.e. https://api.gratisaltest.dk/Documentation.xml for the test environment. 
This file can be used in conjunction with a relevant framework (which one depends on the language you are using) to auto-generate your classes and methods based on the documentation.
This allows you to save a lot of the heavy lifting, and focus on implementing your actual business logic. Our own NuGet package is based on the same idea.

<details><summary><strong>HTTP verbs</strong></summary>
<p>
    
In a slightly modified version of basic REST principles, we utilize the HTTP verbs as follows:
* <strong>GET:</strong> Used for any method that do not require you to post a request body and which does not in any way affect existing data.
* <strong>PATCH:</strong> Used for updating an existing entity, specifying only the relevant properties you want to change. This is the recommended approach for external integrations, although our own client uses PUT.
* <strong>PUT:</strong> Used for updating an existing entity, submitting the full object with all properties. Note that using this verb performs faster than PATCH, but it requires you to continually update your solution as we add new properties. For this reason, PATCH is strongly recommended where supported.
* <strong>DELETE:</strong> Used for deleting or deactivating an existing entity or mapping between entities. 
* <strong>POST:</strong> Used for creating (or in some cases, activating) new entities or mappings between entities.<br/>However, it is also used for methods that do not modify existing data, but requires you to submit a body along with the request. This is because many clients are not able to submit a body along with a GET request.

</p>
</details>

<details><summary><strong>HTTP response codes</strong></summary>
<p>

The API uses the following principles for returning response codes:
* <strong>200 OK:</strong><br/>The normal response indicating the request was processed successfully. For standard PUT or POST requests, the response body will usually contain the updated or created object. 

* <strong>404 Not Found:</strong><br/>This can either mean you are calling an invalid route/URL, or that you have supplied a value (typically an ID) that does not match any relevant entity in our database. 
In the former case, the response will be accompanied by a short explanatory message.

* <strong>401 Unauthorized:</strong><br/>If submitting username/password or other credentials, this simply means that the credentials were invalid. If you call any method that is not publicly available and fail to submit an Authorization header, you will also receive this response.<br/><br/>In other contexts, this response can mean that you are trying to access something that your credentials do not allow you to see or modify. For example, if you attempt to view administrator data while logged in as an employee, or attempt to view data from other companies than the currently active one, you will get this response. (If you have access to the company, it is necessary to first change the company context.)<br/><br/>However the most common reason for this response is simply that the session has expired due to inactivity and a new one should be established. The current session inactivity timeout (at time of writing) is 20 minutes. You may call api/auth/session/isalive to inquire about the status of the current session without extending its duration.

* <strong>400 Bad Request:</strong><br/>This response is encountered quite commonly, and it usually does not imply that the request was technically malformed, but rather that some kind of business logic validation failed, i.e. the request cannot be granted either because you or the end-user did something wrong, or because other circumstances prevent it. The response will always be accompanied by a message explaining the details of the problem.<br/><br/>In general, errors that are expected to occur in normal use (e.g. when the end-user enters a value that is not allowed for some reason) will be translated to the user’s language, whereas errors that are likely the result of a client developer’s mistake (e.g. omitting a required argument from a request) will always be given in English.
The recommended course of action for handling these responses is to show the message to the user. Errors that are caused by the client developer will usually be discovered while testing.

* <strong>500 Internal Server error:</strong><br/>This usually means that something unexpected happened server-side. However it can also mean that you submitted a malformed request body, or in some other way made a request that was so erroneous that the server did not even anticipate the possibility and thus could not respond in a meaningful way.<br/><br/>For this reason, you should first double-check against the documentation to ensure you are calling the method correctly. If this is the case, and you are still getting these responses, please contact us for details.

</p>
</details>


<details><summary><strong>Authentication</strong></summary>
<p>

To authenticate against the API, you submit a POST request to api/auth/login submitting an Authorization header as follows:

```Authorization: Basic [username:password as Base64]```

So if the username is “Gratisal” and the password is “Payroll”, the header would be:

```Authorization: Basic R3JhdGlzYWw6UGF5cm9sbA==```

(Note that we are not actually using the old-fashioned technology known as “Basic Authentication”. The authentication logic is custom-made and designed to be both modern and secure.)<br/>For username, it is possible to use either e-mail or CPR number, just like in the client application.<br/>If you submit valid credentials, the API will return a token which must be stored locally used in all subsequent requests to the API for that session. The token must be submitted in an Authorization header as such:

```Authorization: Token [Token received from the API]```

Please treat the token securely and refrain from e.g. storing it in a text file, submitting it as part of a URL or doing anything else that may expose it to a third-party.<br/><br/>For partner-based solutions we plan to expose an access method that does not require you to enter the actual password of a user. If you are interested in such a solution, please let us know.

</p>
</details>

</p>
</details>

### Technical details and API usage guidelines

Not all of the following is strictly required reading. If you feel like “jumping right in” and don’t mind making some educated guesses about how things work, the section about Authentication is really all you need to know to get started.

<details><summary><strong>Company and language context</strong> (session handling)</summary>
<p>

The authentication token mentioned above refers to a server-side session which contains information about the user, the currently active company and his/her role and access restrictions in this company. It also contains information about the language to use in the session. Most of this information can be viewed by calling GET to ```api/auth/session```.<br/><br/>All API requests are made within the context of the currently active company. This means it is not necessary to specify in each request which company you are operating in. In most scenarios, this means you can simply act as if there is only one company in the database.<br/><br/>To change the company context, call POST to ```api/auth/company/{companyId}```. After this method returns 200 OK, all subsequent calls will be made in the context of this company (including permissions etc.) until you change the company context again or log out. You will of course receive a 401 if the user does not have access to the selected company.<br/><br/>Similarly, to change the language for the session (if the company has enabled access to this feature), you call POST to ```api/auth/language/{languageId}```. This will affect the language of static data, server error messages, generated payslips etc.

</p>
</details>


<details><summary><strong>Fundamental data structure</strong></summary>
<p>

Here is a brief explanation of the data structure employed by Gratisal:
* A <strong>User</strong> is a global entity corresponding to one physical person. They are uniquely identified by their <strong>IdentityNumber</strong>, i.e. the Danish CPR number.
* A <strong>Company</strong> is a company in the Gratisal database. Since one physical person can access multiple companies, we need the concept of a 
* <strong>CompanyUser</strong>, which is the “mapping” of a User to a specific company. This, rather than User, is what you should normally consider your “user” object, unless you are developing solutions that span multiple companies.
* A <strong>UserEmployment</strong> is a work contract meaning that a CompanyUser is actually employed in the company. This is not necessarily the case, as administrators are often not technically employees.
* <strong>Note:</strong> Since it’s also possible for one person to have multiple contracts within the same company, one CompanyUser can have multiple UserEmployments.<br/>Put a bit simply, the CompanyUser is what you see in the General tab in the client application, and the UserEmployment is what you see in the Employment tab.

</p>
</details>


<details><summary><strong>Handling data graphs</strong> (child objects and relationships)</summary>
<p>
    
Relations between objects are always handled through numerical Id columns in accordance with basic principles for relational data. The convention is that primary keys are always named “Id”, whereas foreign keys are named “xxxId” where xxx is the name of the foreign entity.<br/><br/>Although the API will often expose child objects or related objects in a response to a GET request, these objects cannot be returned in a PUT or POST request in order to update the relation. There are few exceptions to this principle, but they are clearly stated in the documentation.<br/>To update a relation between to entities, you instead need to update the relevant numerical foreign key value (i.e. “xxxId”). If you also submit a child object along with the PUT or POST request, it will be ignored.<br/><br/>So to use a fictional example, let’s say you issue a GET request to api/books and receive a Book object containing an ID and an an OwnerId, and also containing a child object named Owner with an Id matching the OwnerId of the parent object.<br/>The correct way to change the ownership of this book would then be to update OwnerId to a new value, which must of course match another Owner object (that you probably retrieved by calling api/owners). You should then submit the Book object back in a PUT request with the Owner object set to null (although if you do submit any value for the Owner object it will usually just be ignored). You cannot submit a modified Owner object back as a child object of the Book, unless the documentation clearly states otherwise.   
    
</p>
</details>


<details><summary><strong>Other conventions</strong></summary>
<p>
    
Other conventions are used in the API with an aim for consistency and predictability. This should become clear from studying the documentation of specific methods, but here are a few examples:
* API routes are case-sensitive, but by convention lowercase-only (although an exception is mentioned below) .

* Entities with the suffix “View” are generally read-only and cannot be updated. They are usually aggregations or joins between different types of entities, i.e. corresponding to a database view.

* Routes that include an entity name in a plural form indicate they are handled in a traditional RESTful manner. For example, ```api/books/{bookId}/chapters``` would give you the chapters belonging to the book with the specified ID.
However, requests that take the form “get X by Y” can sometimes use the singular form for Y. For example, a request to ```api/books/owner/{ownerId}``` would yield the books belonging to the owner with the specified ID (i.e. “Get books by owner”). An alternative route for the same request would be ```api/owners/{ownerId}/books```, but this implies the method is owner-centric where in this case it is clearly book-centric, and should thus be grouped with other book-related methods.

* The name arguments to the routes that start with ```api/staticdata```, e.g. ```api/staticdata/PensionProvider```, break no less than two conventions that are used in the rest of the API. Firstly they must be submitted in PascalCase and not lower-case, and secondly you must use the singular and not the plural form. This is due to technical details in the back-end implementation of these entities.

</p>
</details>
