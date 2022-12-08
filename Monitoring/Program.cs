using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Monitoring
{
    public class Program
    {
        public static Oltconf conf;
        public static Oltconf confex;

        public static void Main(string[] args)
        {
            #region conf constructor
            if (!File.Exists("Oltconfex.json"))
            {
                confex = new Oltconf();
                confex.login = "login for all olts";
                confex.password = "password for all olts";
                confex.olts = new Olt[]
                {
                    new Olt {Name="olt name", Ip="olt ip"},
                    new Olt {Name="is not configuration file", Ip = "this exemple"}
                };
                confex.Serialize();
            }

            conf = Deserialize();
            #endregion

            Console.WriteLine("Select action: \n 1. ShowMac \n 2. ShowReg \n 3. ShowSignal");
            string acid = Console.ReadLine();
            switch (acid)
            {
                case "1":
                    ShowMac();
                    break;
            }

                    TelnetConnection telnet = Connect(conf.olts[0].Ip);
            while(true)
            {
                string str = Console.ReadLine();

                if(str != string.Empty)
                {
                    Console.WriteLine(telnet.WriteLines(str).Split('\n')[0]);
                }
            }

            Console.ReadLine();
        }

        public static void ShowMac()
        {
            Console.WriteLine("Enter Mac-address:");
            var mac = Console.ReadLine();

            TelnetConnection telnet;

            for(int i = 0; i < conf.olts.Length; i++)
            {
                telnet = Connect(conf.olts[i].Ip);
                telnet.WriteLine("show epon onu-information");
                telnet.WriteLine("wer");
                string s = telnet.Read();
                Console.WriteLine(s);
            }
        }

        public static TelnetConnection Connect(string host)
        {
            TelnetConnection tc = new TelnetConnection(host, 23);
            tc.WriteLine(conf.login);
            tc.WriteLine(conf.password);
            tc.WriteLine("enable");
            tc.Read();

            return tc;
        }

        public static Oltconf Deserialize()
        {
            string json = File.ReadAllText("Oltconf.json");
            return JsonSerializer.Deserialize<Oltconf>(json, new JsonSerializerOptions { WriteIndented = true});
        }
    }
}
