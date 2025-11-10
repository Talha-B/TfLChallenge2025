\# TfL Challenge



A .NET 8 console application that retrieves and displays live road status information from the Transport for London (TfL) API.



---



\## Prerequisites



\- .NET 8 SDK

\- Internet access for restoring NuGet packages



---



\## Build



```bash

\# from repository root

dotnet restore

dotnet build





\## Run



\# run the console app project

dotnet run --project src/TfLChallenge



When prompted, enter a road id (for example A2). The app prints the status, and exits with:



0 for success



non-zero for errors





\## Tests



Tests include unit and integration tests using xUnit, FluentAssertions, NSubstitute and WireMock.Net.



\# run all tests

dotnet test



Integration tests use WireMock.Net to simulate the TfL API; they are deterministic and do not call the real TfL API.





\## Configuration



appsettings.json (or environment variables) is used for TfL settings. Example structure:



{

&nbsp; "Tfl": {

&nbsp;   "BaseUrl": "https://api.tfl.gov.uk",

&nbsp;   "Auth": {

&nbsp;     "Id": "<your-id>",

&nbsp;     "Key": "<your-key>"

&nbsp;   }

&nbsp; }

}



Ensure appsettings.json is copied to output.





\## Design / assumptions



Target framework: .NET 8



The Refit client (ITflApi) is registered via AddRefitClient and uses the platform HTTP resilience handler (AddStandardResilienceHandler) to handle transient faults and 429 responses.



RoadStatusService:



Fetches data via ITflApi



Handles 404 as NotFound



Returns a RoadStatusResult (a small immutable DTO/record)



Uses a separate IRoadStatusFormatter for presentation



PlainTextRoadStatusFormatter returns string.Empty for null input (formatter is presentation-only and should not throw)



Logging uses ILogger<T>



DI lifetimes:



Refit client / HttpClient: provided by IHttpClientFactory (singleton-safe)



RoadStatusService: singleton (stateless)



Formatters: transient (stateless, cheap to create)

