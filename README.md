# TfL Challenge

A .NET 8 console application that retrieves and displays live road status information from the Transport for London (TfL) API.

---

## Prerequisites

- .NET 8 SDK (or higher)

---

## Build

**Using Visual Studio:**
1. Open the `.sln` file in Visual Studio
2. Press `Ctrl+Shift+B` or right-click solution → Build Solution

## Run

Hit the run button in Visual Studio or press F5. Alternatively, build as release, navigate to the release output folder and open TfLChallenge.exe. Note, if you double-click the exe directly in Windows Explorer, the console window will close immediately after the program finishes. Instead, run from Command Prompt to see the output.

## Using

When prompted, enter a road id. The app prints the status, and exits with:

- 0 for success
- non-zero for errors

## Tests

Tests include unit and integration tests using xUnit, FluentAssertions, NSubstitute and WireMock.Net.

Integration tests use WireMock.Net to simulate the TfL API; they are deterministic and do not call the real TfL API.

Tests can be run from Visual Studio, right click solution -> "Run tests".

## Configuration

appsettings.json (or environment variables) is used for TfL settings. Example structure:

```json
{
  "Tfl": {
    "BaseUrl": "https://api.tfl.gov.uk",
    "Auth": {
      "Id": "<your-id>",
      "Key": "<your-key>"
    }
  }
}
```

Ensure appsettings.json is copied to output.

## Design / assumptions

**Target framework:** .NET 8 

The Refit client (`ITflApi`) is registered via `AddRefitClient` and uses the platform HTTP resilience handler (`AddStandardResilienceHandler`) to handle transient faults and 429 responses.

**RoadStatusService:**
- Fetches data via `ITflApi`
- Handles 404 as NotFound
- Returns a `RoadStatusResult` (a small immutable DTO/record)
- Uses a separate `IRoadStatusFormatter` for presentation

**PlainTextRoadStatusFormatter** returns `string.Empty` for null input (formatter is presentation-only and should not throw)