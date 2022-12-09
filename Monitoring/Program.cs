using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                case "2":
                    ShowReg();
                    break;
                case "3":
                    ShowSignal();
                    break;
            }
        }

        public static void ShowSignal()
        {
            var olt =SelectOlt();
            Console.Write("Enter port number (0:0):");
            string num = Console.ReadLine();
            if (num.Split(':').Length != 2)
            {
                ShowError();
                return;
            }
            TelnetConnection telnetConnection = Connect(conf.olts[olt].Ip);
            telnetConnection.WriteLine("show epon optical-transceiver-diagnosis interface epon0/" + num);
            string s = telnetConnection.Read();
            string[] splited = s.Split("\r\n");
            for(int i = 1; i<splited.Length-1;i++)
                Console.WriteLine(i+" " + splited[i]);
        }

        public static void ShowReg()
        {
            var sid = SelectOlt();
            if (sid != -1)
            {
                Console.Write("Select number interface:");
                var input = Console.ReadLine();
                if(input is null) return;
                TelnetConnection telnetConnection = Connect(conf.olts[sid].Ip);
                telnetConnection.WriteLine("show mac address-table interface ePON 0/"+input);
                for(int q=0;q<8;q++) telnetConnection.WriteLine(" ");
                string r = telnetConnection.Read();
                string[] spr =r.Split('\n');
                List<ListOfIpadres> matchCollection1 = new List<ListOfIpadres>();
                foreach (var i in spr)
                {
                    Regex regex = new Regex("[A-Za-z0-9;]{1,4}(\\.[A-Za-z0-9;]{1,4}){2}");
                    Regex regex2 = new Regex("epon0/[0-9]{1,2}\\:[0-9]{1,2}");
                    Match match = regex.Match(i);
                    Match match2 = regex2.Match(i);

                    if (!string.IsNullOrEmpty(match.Value) && !string.IsNullOrEmpty(match2.Value))
                        matchCollection1.Add(new ListOfIpadres(match2.Value, match.Value));
                }
                var ipslp = conf.olts[sid].Ip.Split('.');
                var wp = $"{ipslp[0]}.{ipslp[1]}.{ipslp[2]}.254";
                telnetConnection = null;
                telnetConnection = new TelnetConnection(wp, 23);
                telnetConnection.WriteLine("P@ssw0rd");
                telnetConnection.Read();
                for(int i =0; i < matchCollection1.Count; i++)
                {
                    Console.WriteLine("------------------ (" + matchCollection1[i].Name + ") ------------------");
                    telnetConnection.WriteLine("show arp | include " + matchCollection1[i].Mac);    
                    for(int q=0;q<8;q++) telnetConnection.WriteLine(" ");

                    string s = telnetConnection.Read();
                    string[] ssplit = s.Split("\r\n");
                    foreach(var item in ssplit)
                    {
                        Regex regex = new Regex("[0-9]{1,3}(\\.[0-9]{1,3}){3}");
                        Match match = regex.Match(item);
                        if(!string.IsNullOrEmpty(match.Value))
                            Console.WriteLine(match.Value);
                    }
                    Console.WriteLine("-------------------------------------------------");
                }
            }
            else Console.WriteLine("Input data error");
        }

        public static void ShowMac()
        {
            Console.Write("Enter Mac-address:");
            var mac = Console.ReadLine();
            if (mac.Length != 4) return;
            TelnetConnection telnet;

            for(int i = 0; i < conf.olts.Length; i++)
            {
                Console.WriteLine("-->"+conf.olts[i].Ip+ "<--");
                telnet = Connect(conf.olts[i].Ip);
                telnet.WriteLine("show epon onu-infor");
                for(int q=0;q<8;q++) telnet.WriteLine(" ");
                string s = telnet.Read();
                string[] split = s.Split('\n');
                var mcs = split.FirstOrDefault(x => x.Contains(mac));
                if (String.IsNullOrEmpty(mcs))
                {
                    Console.WriteLine("No mac-address");
                }
                else Console.WriteLine(mcs);
            }
        }

        public static void ShowError()
        {
            Console.WriteLine("Error");
        }

        public static int SelectOlt()
        {
            Console.WriteLine("Select OLT");
            for (int i = 0; i < conf.olts.Length; i++)
            {
                Console.WriteLine(" "+i + ". " + conf.olts[i].Name);
            }
            var olt = Convert.ToInt32(Console.ReadLine());
            if (conf.olts[olt] is null) return -1;
            return olt;
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
    public class ListOfIpadres
    {
        public string Name { get; set; }
        public string Mac { get; set; }
        public string Ip { get; set; }

        public ListOfIpadres(string name, string mac)
        {
            Name = name;
            Mac = mac;
        }
    }
}
