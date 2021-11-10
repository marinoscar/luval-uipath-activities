using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GoogleTextAnalyticsParser
{
    public class EntityParser
    {

        public static DataTable GetEntities(string jsonResult)
        {
            var dt = new DataTable("Entities");
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Score", typeof(double));
            dt.Columns.Add("Type", typeof(string));
            var json = JObject.Parse(jsonResult);
            var jsonEntities = json["entities"].Value<JArray>();
            foreach (var jsonEnt in jsonEntities)
            {
                var row = dt.NewRow();
                row["Name"] = jsonEnt["name"].Value<string>();
                row["Score"] = jsonEnt["salience"].Value<float>();
                row["Type"] = jsonEnt["type"].Value<string>();
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
