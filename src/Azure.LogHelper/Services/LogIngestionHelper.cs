using Azure.Core;
using Azure.Identity;
using AzureLogHelper.Contracts;
using AzureLogHelper.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Azure.Monitor.Ingestion;

namespace AzureLogHelper.Services
{
    public class LogIngestionHelper : ILogIngestionHelper
    {
        private string? AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET;
        private string? AZURE_MONITOR_DCE, AZURE_MONITOR_DCR, AZURE_MONITOR_DCR_StreamName;

        public LogIngestionHelper(IConfiguration configuration)
        {
            IConfiguration _configuration = configuration;
            AZURE_TENANT_ID = _configuration["AZURE_TENANT_ID"];
            AZURE_CLIENT_ID = _configuration["AZURE_CLIENT_ID"];
            AZURE_CLIENT_SECRET = _configuration["AZURE_CLIENT_SECRET"];
            AZURE_MONITOR_DCE = _configuration["AZURE_MONITOR_DCE"];
            AZURE_MONITOR_DCR = _configuration["AZURE_MONITOR_DCR"];
            AZURE_MONITOR_DCR_StreamName = _configuration["AZURE_MONITOR_DCR_STREAM"];
        }


        #region async methods

        public async void SendLogEntriesAsync(string jsonLogString)
        {
            PushLogsAsync(jsonLogString);
        }

        public async void SendLogEntriesAsync(string dcrStreamName, string jsonLogString)
        {

            AZURE_MONITOR_DCR_StreamName = dcrStreamName;

            PushLogsAsync(jsonLogString);
        }

        public async void SendLogEntriesAsync(string dataCollectionRuleId, string dcrStreamName, string jsonLogString)
        {

            AZURE_MONITOR_DCR = dataCollectionRuleId;
            AZURE_MONITOR_DCR_StreamName = dcrStreamName;

            PushLogsAsync(jsonLogString);
        }

        public async void SendLogEntriesAsync(string dataCollectionEndpoint, string dataCollectionRuleId, string dcrStreamName, string jsonLogString)
        {

            AZURE_MONITOR_DCE = dataCollectionEndpoint;
            AZURE_MONITOR_DCR = dataCollectionRuleId;
            AZURE_MONITOR_DCR_StreamName = dcrStreamName;
            PushLogsAsync(jsonLogString);
        }

        public async void SendLogEntriesAsync(string azureTenantId, string azureClientId, string azureClientSecret, string dataCollectionEndpoint, string dataCollectionRuleId, string dcrStreamName, string jsonLogString)
        {

            AZURE_TENANT_ID = azureTenantId;
            AZURE_CLIENT_ID = azureClientId;
            AZURE_CLIENT_SECRET = azureClientSecret;
            AZURE_MONITOR_DCE = dataCollectionEndpoint;
            AZURE_MONITOR_DCR = dataCollectionRuleId;
            AZURE_MONITOR_DCR_StreamName = dcrStreamName;
            PushLogsAsync(jsonLogString);
        }

        public async void SendLogEntriesAsync(LogIngestionClientConfig logIngestionClientConfig, string jsonLogString)
        {

            AZURE_TENANT_ID = logIngestionClientConfig.AZURE_TENANT_ID;
            AZURE_CLIENT_ID = logIngestionClientConfig.AZURE_CLIENT_ID;
            AZURE_CLIENT_SECRET = logIngestionClientConfig.AZURE_CLIENT_SECRET;

            AZURE_MONITOR_DCE = logIngestionClientConfig.AZURE_MONITOR_DCE;
            AZURE_MONITOR_DCR = logIngestionClientConfig.AZURE_MONITOR_DCR;
            AZURE_MONITOR_DCR_StreamName = logIngestionClientConfig.AZURE_MONITOR_DCR_StreamName;
            PushLogsAsync(jsonLogString);
        }

        private void PushLogsAsync(string jsonLogString)
        {
            var endpoint = new Uri(AZURE_MONITOR_DCE!);

            var credential = new ClientSecretCredential(AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET);
            LogsIngestionClient client = new LogsIngestionClient(endpoint, credential);

            // Upload logs

            byte[] logBytes = Encoding.UTF8.GetBytes(jsonLogString);
            client.UploadAsync(AZURE_MONITOR_DCR, AZURE_MONITOR_DCR_StreamName, RequestContent.Create(logBytes));
        }

        #endregion


        #region sync methods

        //public Response SendLogEntries(string jsonLogString)
        //{
        //    return PushLogs(jsonLogString);
        //}

        //public Response SendLogEntries(string dcrStreamName, string jsonLogString)
        //{

        //    AZURE_MONITOR_DCR_StreamName = dcrStreamName;

        //    return PushLogs(jsonLogString);
        //}

        //public Response SendLogEntries(string dataCollectionRuleId, string dcrStreamName, string jsonLogString)
        //{

        //    AZURE_MONITOR_DCR = dataCollectionRuleId;
        //    AZURE_MONITOR_DCR_StreamName = dcrStreamName;

        //    return PushLogs(jsonLogString);
        //}

        //public Response SendLogEntries(string dataCollectionEndpoint, string dataCollectionRuleId, string dcrStreamName, string jsonLogString)
        //{

        //    AZURE_MONITOR_DCE = dataCollectionEndpoint;
        //    AZURE_MONITOR_DCR = dataCollectionRuleId;
        //    AZURE_MONITOR_DCR_StreamName = dcrStreamName;

        //    return PushLogs(jsonLogString);
        //}

        //public Response SendLogEntries(string azureTenantId, string azureClientId, string azureClientSecret, string dataCollectionEndpoint, string dataCollectionRuleId, string dcrStreamName, string jsonLogString)
        //{

        //    AZURE_TENANT_ID = azureTenantId;
        //    AZURE_CLIENT_ID = azureClientId;
        //    AZURE_CLIENT_SECRET = azureClientSecret;

        //    AZURE_MONITOR_DCE = dataCollectionEndpoint;
        //    AZURE_MONITOR_DCR = dataCollectionRuleId;
        //    AZURE_MONITOR_DCR_StreamName = dcrStreamName;

        //    return PushLogs(jsonLogString);
        //}

        //public Response SendLogEntries(LogIngestionClientConfig logIngestionClientConfig, string jsonLogString)
        //{

        //    AZURE_TENANT_ID = logIngestionClientConfig.AZURE_TENANT_ID;
        //    AZURE_CLIENT_ID = logIngestionClientConfig.AZURE_CLIENT_ID;
        //    AZURE_CLIENT_SECRET = logIngestionClientConfig.AZURE_CLIENT_SECRET;

        //    AZURE_MONITOR_DCE = logIngestionClientConfig.AZURE_MONITOR_DCE;
        //    AZURE_MONITOR_DCR = logIngestionClientConfig.AZURE_MONITOR_DCR;
        //    AZURE_MONITOR_DCR_StreamName = logIngestionClientConfig.AZURE_MONITOR_DCR_StreamName;

        //   return PushLogs(jsonLogString);
        //}

        //private Response PushLogs(string jsonLogString)
        //{
        //    var endpoint = new Uri(AZURE_MONITOR_DCE);

        //    var credential = new ClientSecretCredential(AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET);
        //    LogsIngestionClient client = new LogsIngestionClient(endpoint, credential);

        //    // Upload logs
        //    byte[] logBytes = Encoding.UTF8.GetBytes(jsonLogString);
        //    var res = client.Upload(AZURE_MONITOR_DCR, AZURE_MONITOR_DCR_StreamName, RequestContent.Create(logBytes));
        //    return res;
        //}
        #endregion
    }
}

