using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLogHelper.Models
{
    public class LogIngestionClientConfig
    {
        public string? AZURE_TENANT_ID { get; set; }
        public string? AZURE_CLIENT_ID { get; set; }
        public string? AZURE_CLIENT_SECRET { get; set; }
        public string? AZURE_MONITOR_DCE { get; set; }
        public string? AZURE_MONITOR_DCR { get; set; }
        public string? AZURE_MONITOR_DCR_StreamName { get; set; }
    }
}
