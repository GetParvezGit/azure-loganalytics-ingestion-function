# azure-loganalytics-ingestion-function
This solution aims to log the JSON payloads data into the Azure Log Analytics custom table using Data Collection Rules (DCR) and Data Collection Endpoints (DCE).

# Azure Logger Function

A lightweight Azure Functions solution to ingest custom logs into Azure Monitor (Logs) using the Azure Monitor Data Collection Endpoint (DCE) and Data Collection Rule (DCR). The solution consists of an HTTP-triggered Azure Function (`AzureLoggerFunction`) and a reusable helper library (`Azure.LogHelper`) that wraps the Azure Monitor Logs ingestion client.

## Contents

- `AzureLoggerFunction/` - Azure Functions project (HTTP trigger) that accepts log payloads and forwards them to the LogHelper.
- `Azure.LogHelper/` - Class library providing `LogIngestionHelper` to upload logs using `Azure.Monitor.Ingestion`.

## Features

- Receive JSON log arrays via HTTP POST.
- Dynamically select credentials and DCR/DCE configuration via headers or configuration.
- Uses `ClientSecretCredential` for service-to-service authentication to Azure Monitor ingestion endpoint.

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code
- Azure Storage (for function runtime local usage) — Azurite or Access to an Azure Storage account
- Azure AD App Registration (client id / secret) with permission to ingest logs using DCR
- Data Collection Endpoint (DCE) and Data Collection Rule (DCR) configured in the target subscription

## Important local configuration (`local.settings.json`)

Place the following keys in `AzureLoggerFunction/local.settings.json` (or set as Azure Function App settings in production):

- `AZURE_TENANT_ID_<InstanceName>` - Azure AD tenant id for the service principal
- `AZURE_CLIENT_ID_<InstanceName>` - Service principal client id
- `AZURE_CLIENT_SECRET_<InstanceName>` - Service principal client secret
- `AZURE_MONITOR_DCE_<InstanceName>` - Data Collection Endpoint URL (e.g. `https://<name>.ingest.monitor.azure.com`)
- `AZURE_MONITOR_DCR_<InstanceName>` - Data Collection Rule resource id (DCR)
- `AZURE_MONITOR_DCR_StreamName_<InstanceName>` - Stream name (e.g. `Custom-MyCustomTable_CL`)

Example snippet from this repository's `local.settings.json`:

```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "AZURE_TENANT_ID_IntegrationLogger": "<tenant-id>",
    "AZURE_CLIENT_ID_IntegrationLogger": "<client-id>",
    "AZURE_CLIENT_SECRET_IntegrationLogger": "<client-secret>",
    "AZURE_MONITOR_DCE_IntegrationLogger": "https://dce-<name>.ingest.monitor.azure.com",
    "AZURE_MONITOR_DCR_IntegrationLogger": "<dcr-resource-id>",
    "AZURE_MONITOR_DCR_StreamName_IntegrationLogger": "Custom-<TableName>_CL"
  }
}
```

**Note:** In production, store secrets in Azure Key Vault or set them as Function App settings (do not check secrets into source control).

## How the Function Expects Requests

- **HTTP Method:** POST
- **Headers:**
  - `AZURE_MONITOR_DCR_STREAM` — the DCR stream name (e.g. `Custom-IntegrationLogger_CL`)
  - `LogAnalytics_InstanceName` — suffix used to pick the configuration keys from settings (for example, `IntegrationLogger` will cause the function to read `AZURE_TENANT_ID_IntegrationLogger`, etc.)
- **Request Body:** A JSON array of objects representing log records. Example body:

```
[
  {
    "TimeGenerated": "2026-02-01T14:05:00Z",
    "InterfaceName": "LoggerFunction",
    "LogType": "SUCCESS",
    "LogMessage": "Logging Message",
    "LogDetails": "This is a test log."
  }
]
```

### Example curl Command

```
curl -X POST "http://localhost:7071/api/LogIngestionLogger" \
  -H "Content-Type: application/json" \
  -H "AZURE_MONITOR_DCR_STREAM: Custom-IntegrationLogger_CL" \
  -H "LogAnalytics_InstanceName: IntegrationLogger" \
  -d '[{"TimeGenerated":"2026-02-01T14:05:00Z","InterfaceName":"LoggerFunction","LogType":"SUCCESS","LogMessage":"Logging Message","LogDetails":"This is a test log."}]'
```

## Run Locally

1. Start Azurite (or ensure `AzureWebJobsStorage` points to a reachable storage account).
2. From the solution root, open the solution in Visual Studio or run from the command line:

   dotnet build
   dotnet run --project AzureLoggerFunction

3. Test using the curl example above.

## Build & Test

- **Build:** `dotnet build` (solution root)
- **Run function project locally:** `func start` (if using Azure Functions CLI) or `dotnet run --project AzureLoggerFunction`

## Security & Best Practices

- Do not store client secrets in source control. Use Azure Key Vault or platform-managed identities in production.
- Validate and sanitize incoming payloads before ingestion.
- Consider adding authentication to the function (Function-level key or Azure AD) to restrict who can POST log data.
- Use managed identities where possible instead of client secrets.

## Extensibility

- The `Azure.LogHelper` project exposes `ILogIngestionHelper` and `LogIngestionHelper` that can be reused in other services.
- The helper supports several overloads to set credentials and target DCR/DCE programmatically.

## Contributing

- Follow repository coding conventions. Add tests for new behavior and submit PRs with clear descriptions.

## License

This project is provided under the MIT License. See `LICENSE` file for details. 

This revised README maintains the original structure while enhancing clarity and organization, ensuring that users can easily understand and utilize the Azure Logger Function project.
