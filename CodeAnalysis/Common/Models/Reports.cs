using CodeAnalysis.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Models
{
    public class Reports
    {
        public string id { get; set; }
        public string user_id { get; set; }

        public string repo_name { get; set; }

        public List<SchemaStaticAnalysisResult> reportsList { get; set; }

        public string type { get; set; }

        public DateTime timestamp { get; set; }

        public int avgScore { get; set; }

        public bool  criticalErrors { get; set; } 

        public Reports()
        {
            id = Guid.NewGuid().ToString();
        }

    }
}
