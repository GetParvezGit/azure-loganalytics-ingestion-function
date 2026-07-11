using AzureLogHelper.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using static AzureLogHelper.Utils.Constants;

namespace AzureLoggerFunction;

public class LogIngestionLogger
{
    private readonly ILogIngestionHelper _logIngestionHelper;
    private readonly IConfiguration _configuration;

    public LogIngestionLogger(ILogIngestionHelper logIngestion, IConfiguration configuration)
    {
        _logIngestionHelper = logIngestion;
        _configuration = configuration;
    }

    [Function(FunctionName.LogIngestionLogger)]
    public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
    {
        try
        {

            // LogAnalytics_InstanceName : Pass the log analytics custom table name
            // AZURE_MONITOR_DCR_STREAM : Pass the "Custom-{CustomTableName}_CL"
            // Sample request body: [{ "TimeGenerated": "2026-02-01T14:05:00Z", "InterfaceName": "LoggerFunction", "LogType": "SUCCESS", "LogMessage": "Logging Message", "LogDetails": "This is a test log." }]

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            if (!req.Headers.TryGetValue("AZURE_MONITOR_DCR_STREAM", out var LogTableName))
            {
                log.LogError("Error Executing funct-custom-logger: No AZURE_MONITOR_DCR_STREAM provided.");
                return new ObjectResult("{ Error: No Log Name provided.}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            if (req.Headers.TryGetValue("LogAnalytics_InstanceName", out var instanceName))
            {

                //adding InstanceName
                string azTenantID = $"AZURE_TENANT_ID_{instanceName}";
                string azClientID = $"AZURE_CLIENT_ID_{instanceName}";
                string azClientSecret = $"AZURE_CLIENT_SECRET_{instanceName}";
                string azMonitorDce = $"AZURE_MONITOR_DCE_{instanceName}";
                string azMonitorDcr = $"AZURE_MONITOR_DCR_{instanceName}";

                //Using the value from config
                string _AZURE_TENANT_ID = _configuration[azTenantID]!;
                string _AZURE_CLIENT_ID = _configuration[azClientID]!;
                string _AZURE_CLIENT_SECRET = _configuration[azClientSecret]!;
                string _AZURE_MONITOR_DCE = _configuration[azMonitorDce]!;
                string _AZURE_MONITOR_DCR = _configuration[azMonitorDcr]!;
                string _AZURE_MONITOR_DCR_StreamName = LogTableName!;

                if (string.IsNullOrEmpty(_AZURE_TENANT_ID) || string.IsNullOrWhiteSpace(_AZURE_CLIENT_ID) || string.IsNullOrWhiteSpace(_AZURE_CLIENT_SECRET)
                    || string.IsNullOrWhiteSpace(_AZURE_MONITOR_DCE) || string.IsNullOrWhiteSpace(_AZURE_MONITOR_DCR) || string.IsNullOrWhiteSpace(_AZURE_MONITOR_DCR_StreamName))
                {
                    log.LogError($"Invalid or missing configuration.");
                    return new ObjectResult("{\"Error\": \"Invalid or missing configuration.\"}") { StatusCode = (int)HttpStatusCode.BadRequest };
                }

                string jsonLogString = requestBody;

                // this required when we get from from input 
                _logIngestionHelper.SendLogEntriesAsync(_AZURE_TENANT_ID, _AZURE_CLIENT_ID, _AZURE_CLIENT_SECRET, _AZURE_MONITOR_DCE, _AZURE_MONITOR_DCR, LogTableName!, jsonLogString);

                return new OkObjectResult("Log Ingestion Successful");
            }
            else
            {
                return new ObjectResult("{\"Error\": \"Invalid or missing parameter(s).\"}") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
        }
        catch (Exception ex)
        {
            log.LogError("Error Executing funct-custom-logger: " + ex.Message + ", " + ex.InnerException + ", Trace: " + ex.StackTrace);
            return new ObjectResult("{\"error\": \"" + ex.Message + "\"}") { StatusCode = (int)HttpStatusCode.InternalServerError };
        }
    }
}