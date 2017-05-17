using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketSender
{
    public class AqmmMessage : Message
    {
        private int lat, lon, alt;
        private int[] data;
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        public AqmmMessage(int lat, int lon, int alt, int[] data)
        {
            this.lat = lat;
            this.lon = lon;
            this.alt = alt;
            this.data = data;
            dict["latitude"] = lat;
            dict["longitude"] = lon;
            dict["altitude"] = alt;
        }

        public string JSON()
        {
            string json = "";
            json += "{";
            foreach (KeyValuePair<string, object> entry in dict)
            {
                json += @"""" + entry.Key + @""" : " + entry.Value.ToString() + ",";
            }
            json += @"""data : """ + string.Join(", ", data);
            //json = json.Remove(json.Length - 1);
            json += "}";
            return json;
        }
    }
}
