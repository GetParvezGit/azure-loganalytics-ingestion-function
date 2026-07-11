using AzureLogHelper.Models;

namespace AzureLogHelper.Contracts
{
    public interface ILogIngestionHelper
    {

        public void SendLogEntriesAsync(string jsonLogString);
        public void SendLogEntriesAsync(string dcrStreamName, string jsonLogString);
        public void SendLogEntriesAsync(string dataCollectionRuleId, string dcrStreamName, string jsonLogString);
        public void SendLogEntriesAsync(string dataCollectionEndpoint, string dataCollectionRuleId, string dcrStreamName, string jsonLogString);
        public void SendLogEntriesAsync(string azureTenantId, string azureClientId, string azureClientSecret, string dataCollectionEndpoint, string dataCollectionRuleId, string dcrStreamName, string jsonLogString);
        public void SendLogEntriesAsync(LogIngestionClientConfig logIngestionClientConfig, string jsonLogString);

    }
}
