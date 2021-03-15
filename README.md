# Manage your education and skills funding - Contracts approver function

## Introduction

Contracts approver is a serverless azure function that handles contract signing by learning providers and updates the underlying contract system. This function is triggered by an azure service bus message.

### Getting Started

This product is a Visual Studio 2019 solution containing several projects (Azure function application, service, and repository layers, with associated unit test and integration test projects).
To run this product locally, you will need to configure the list of dependencies, once configured and the configuration files updated, it should be F5 to run and debug locally.

### Installing

Clone the project and open the solution in Visual Studio 2019.

#### List of dependencies

|Item |Purpose|
|-------|-------|
|Azure Storage Emulator| The Microsoft Azure Storage Emulator is a tool that emulates the Azure Blob, Queue, and Table services for local development purposes. This is required for webjob storage used by azure functions.|
|Azure function development tools | To run and test azure functions locally. |
|Azure service bus | To trigger this function, it cannot be set up locally, you will need an azure subscription to set-up azure service bus. |
|Contracts approval API | Backend API for managing contracts. |
|Audit API | Backend API for managing contracts. |

#### Azure Storage Emulator

The Storage Emulator is available as part of the Microsoft Azure SDK. Azure function requires it for local development.

#### Azure function development tools

You can use your favourite code editor and development tools to create and test functions on your local computer.
We used visual studio and Azure core tools CLI for development and testing. You can find more information for your favourite code editor at <https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-local>.

* Using Visual Studio - To develop functions using visual studio, include the Azure development workload in your Visual Studio installation. More detailed information can be found at <https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs>.
* Azure Functions Core Tools - These tools provide CLI with core runtime and templates for creating functions, which can be used to develop and run functions without visual studio. This can be installed using package managers like `npm` or `chocolately` more detailed information can be found at <https://www.npmjs.com/package/azure-functions-core-tools>.

#### Azure service bus

Microsoft Azure Service Bus is a fully managed enterprise message broker.
Publish-subscribe topics are used by this application to decouple approval processing.
There are no emulators available for azure service bus, hence you will need an azure subscription and set-up a service bus namesapce with a topic created to run this application.
Once you have set-up an azure service bus namespace, you will need to create a shared access policy to set in local configuration settings.

#### Contracts approval API

This is an internal system, there are no specification or test system available for general usage. However you can use any HTTP mock or stub for this, all that is required is to accept a POST, path to post is configurable.
As an example mockserver can be used to accept a POST, more information can be found at <https://github.com/namshi/mockserver>.

For simplicity following are set of instructions to install mockserver globally.

```cmd
npm install -g mockserver
```

Once installed create folder structure `c:\temp\mocks\api\contract\approve` and inside `approve` a valid POST response as a file with extension mock.
e.g `POST.mock`

```txt
HTTP/1.1 200 OK
Content-Type: text/json; charset=utf-8

{
   "Host": "mock-contract-approver",
   "Accept-Charset": "utf-8",
   "Accept": "text/json"
}
```

And then you can run mockserver to accept post to `http://localhost:8080/api/contract/approve` by running the following command

```cmd
mockserver -p 8080 -m mocks c:\temp\mocks
```

#### Audit API

Audit API can be found at <https://github.com/SkillsFundingAgency/pds-shared-audit-api>.

### Local Config Files

Once you have cloned the public repo you need the following configuration files listed below.

| Location | config file |
|-------|-------|
| Pds.Contracts.Approver.Func | local.settings.json |

The following is a sample configuration file

```json
{
  "IsEncrypted": false,
  "version": "2.0",
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",

    "Pds.Contracts.Notifications.Topic": "replace_topic",
    "Pds.Contracts.Approval.Subscription": "replace_subscription",
    "sb-connection-string": "replace_ServiceBusConnectionString",

    "FcsApiClientConfiguration:ApiBaseAddress": "replace_local_fcs_stub",
    "FcsApiClientConfiguration:ShouldSkipAuthentication": "true",

    "FcsApiClientConfiguration:ApiContractApproverEndpoint": "/api/contract/approve",

    "AuditApiConfiguration:ApiBaseAddress": "replace_local_audit_api_or_stub",
    "AuditApiConfiguration:ShouldSkipAuthentication": "true",

    "HttpPolicyOptions:HttpRetryCount": 3,
    "HttpPolicyOptions:HttpRetryBackoffPower": 2,
    "HttpPolicyOptions:CircuitBreakerToleranceCount": 5,
    "HttpPolicyOptions:CircuitBreakerDurationOfBreak": "0.00:00:15"
  }
}
```

The following configurations needs to be replaced with your values.
|Key|Token|Example|
|-|-|-|
|Pds.Contracts.Notifications.Topic|replace_topic|contract-notifications|
|Pds.Contracts.Approval.Subscription|replace_subscription|contract-approval|
|sb-connection-string|replace_ServiceBusConnectionString|A valid azure service bus connection string|
|FcsApiClientConfiguration:ApiBaseAddress|replace_local_fcs_stub|<http://localhost:8080/>|
|AuditApiConfiguration:ApiBaseAddress|replace_local_audit_api_or_stub|<http://localhost:5001/>|

## Build and Test

This API is built using

* Microsoft Visual Studio 2019
* .Net Core 3.1

To build and test locally, you can either use visual studio 2019 or VSCode or simply use dotnet CLI `dotnet build` and `dotnet test` more information in dotnet CLI can be found at <https://docs.microsoft.com/en-us/dotnet/core/tools/>.

## Contribute

To contribute,

* If you are part of the team then create a branch for changes and then submit your changes for review by creating a pull request.
* If you are external to the organisation then fork this repository and make necessary changes and then submit your changes for review by creating a pull request.
