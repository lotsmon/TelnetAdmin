using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Monitoring
{
    public class Oltconf
    {
        public string login { get; set; }
        public string password { get; set; }
        public Olt[] olts { get; set; }
    }

    public class Olt
    {
        public string Name { get; set; }
        public string Ip { get; set; }
    }

    public static class Extentions
    {
        public static void Serialize(this Oltconf conf)
        {
            string json = JsonSerializer.Serialize(conf, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("Oltconfex.json", json);
        }
    }
}
