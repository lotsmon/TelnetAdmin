using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnmpSharpNet;

namespace Monitoring
{
    public class Snmp
    {
        // PowerSNMP

        public Snmp Run()
        {
            string host = "92.255.79.101";
            string community = "public";
            SimpleSnmp snmp = new SimpleSnmp(host, community);
            

            if (!snmp.Valid)
            {
                Console.WriteLine("SNMP agent host name/ip address is invalid.");
                return this;
            }
            Dictionary<Oid, AsnType> result = snmp.GetBulk(new string[] { ".1.3.6.1.2", ".1.3.6.1.3" });
            var walk = snmp.Walk(SnmpVersion.Ver2, ".1.3.6.1.2.1.1.1");
            if (result == null)
            {
                Console.WriteLine("No results received.");
                return this;
            }

            foreach (var kvp in result)
            {
                Console.WriteLine("{0}: {1} {2}", kvp.Key.ToString(),
                                      SnmpConstants.GetTypeName(kvp.Value.Type),
                                      kvp.Value.ToString());
            }
            return this;
        }
    }
}
