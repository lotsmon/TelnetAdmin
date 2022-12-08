using PrimS.Telnet;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace Monitoring
{
    public class Porgram1
    {

        public static string username = "admin";
        public static string password = "tw3x63bw"; //"2emx6fkm";

        public static void Main(string[]  args)
        {
            Console.WriteLine("Select action: \n 1. ShowMac \n 2. ShowReg \n 3. ShowSignal");
            //int acid = Console.Read();
            ShowMac();



            /*
            Console.WriteLine("Select OLT: ");
            foreach (var item in OltSwitches)
            {
                Console.WriteLine($"{item.Name}, {item.Ip}");
            }
            Console.WriteLine("------------------");
            */

        }
        /*
        public static async void ShowMac()
        {
            string result = "";
            Console.WriteLine("Enter Mac-address:");
            string mac = Console.ReadLine();

            for(int i = 0; i< OltSwitches.Count-1; i++)
            {
                Console.WriteLine(OltSwitches[i].Ip);
                using (Client client = new Client(OltSwitches[i].Ip, 23, new System.Threading.CancellationToken()))
                {
                    await client.TryLoginAsync(username, password, 5000);
                    client.WriteLineAsync("enable");
                    client.WriteLineAsync("show epon onu-information");
                    string response = await client.TerminatedReadAsync(">", TimeSpan.FromMilliseconds(5000));
                    if(response != null)
                    {
                        result = response.Split('\n').FirstOrDefault(x => x.Contains(mac));
                        Console.WriteLine(result);
                    }
                }
                Console.WriteLine("----------------------");
            }
        }

        public static async Task Connect()
        {
            using (Client client = new Client(OltSwitches[0].Ip, 23, new System.Threading.CancellationToken()))
            {
                await client.TryLoginAsync(username, password, 5000);
                client.WriteLineAsync("enable");
                client.WriteLineAsync("show epon onu-information");
                string response = await client.TerminatedReadAsync(">", TimeSpan.FromMilliseconds(5000));
                Console.WriteLine(response);
            }
        }
        */
    }
}
