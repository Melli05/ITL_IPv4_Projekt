using System.Text.RegularExpressions;

namespace KARR_IPv4_24
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ip_dec = string.Empty;
            var snm_dec = string.Empty;
            var snm_bin = string.Empty;
            var input = '\0'; // \0 char-Äquivalent zu string.Empty

            while (true)
            {
                Console.Write("Bitte die IP-Adresse eingeben: ");
                ip_dec = Console.ReadLine();

                Match match = Regex.Match(ip_dec, @"^(((?!25?[6 - 9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
                if (match.Success)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Bitte eine gültige IP-Adresse im Format XXX.XXX.XXX.XXX angeben!\n");
                }
            }

            var ip_bin = dez_to_bin(ip_dec.Split('.'));

            while (true)
            {
                Console.Write("CIDR-Suffix verwenden? (Y/N) ");
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (input == 'Y' || input == 'y' || input == 'n' || input == 'N')
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Bitte entweder mit Y oder N Auswahl treffen.\n");
                }
            }

            if (input == 'N' || input == 'n')
            {
                while (true)
                {
                    Console.Write("Bitte die Subnetzmaske angeben: ");
                    snm_dec = Console.ReadLine();

                    Match match = Regex.Match(snm_dec, @"^(((?!25?[6-9])[12]\d|[1-9])?\d\.?\b){4}$");
                    if (match.Success)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Bitte eine gültige Subnetzmaske im Format XXX.XXX.XXX.XXX angeben!\n");
                    }
                }

                snm_bin = dez_to_bin(snm_dec.Split('.'));
            }
            else
            {
                int cidr = -1;

                while (true)
                {
                    Console.Write("Bitte die CIDR-Suffix angeben: /");
                    try
                    {
                        cidr = int.Parse(Console.ReadLine());

                        if (cidr <= 32 && cidr >= 0)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Wert zwischen 0 und 32 angeben.\n");
                        }
                    }
                    catch(Exception x) // Sollten Buchstaben als Input gegeben werden
                    {
                        Console.WriteLine("Fehler: " + x.Message);
                    }
                    
                }

                snm_bin = CIDR_to_bin(cidr);
            }

            var id_dec = adressen_calc(ip_bin, snm_bin);
            var bca_dec = adressen_calc(ip_bin, snm_bin, true);
            var host_range = range_calc(snm_bin);
            var first_host = host_calc(id_dec, true);
            var last_host = host_calc(bca_dec, false);

            Console.WriteLine("\nNetz-ID: " + id_dec +
                              "\nBroadcastadresse:" + bca_dec +
                              "\nHost-Range: " + host_range +
                              "\nErster Host: " + first_host +
                              "\nLetzter Host: " + last_host);
        }
        public static string dez_to_bin(string[] input)
        {
            string adresse = string.Empty;

            foreach (string oktett in input)
            {
                adresse += Convert.ToString(int.Parse(oktett), 2).PadLeft(8, '0') + "."; // konvertiert dezimale Zahlen zu binäre Zahlen
            }

            return adresse.Remove(adresse.Length - 1);
        }

        public static string CIDR_to_bin(int cidr)
        {
            string snm = string.Empty;

            snm += new string('1', cidr); // fügt 1 ein
            snm += new string('0', 32 - cidr); // fügt 0 ein

            snm = snm.Insert(24, ".");
            snm = snm.Insert(16, ".");
            snm = snm.Insert(8, ".");

            return snm;
        }

        public static string adressen_calc(string ip, string snm, bool broadcast = false)
        {
            string calc_adresse = string.Empty;

            // splittet Adressen in Oktette für bitwise Operationen
            var ip_split = ip.Split('.');
            var snm_split = snm.Split('.');

            for (int i = 0; i < ip_split.Length; i++)
            {
                var ips = ip_split[i];
                var ids = snm_split[i];
                int ergebnis_okt = 0;

                if (broadcast)
                {
                    // Berechnet Broadcast-Adresse durch Invertierten der SNM und bitwise OR
                    ergebnis_okt = (Convert.ToInt16(ips, 2) | (~Convert.ToInt16(ids, 2) & 0xFF)); // 0xFF, damit nur 8 Bit behalten werden, weil ~ aus int ansonsten eine 32-Bit Zahl wird
                }
                else
                {
                    // Berechnet Netz-ID durch bitwise AND
                    ergebnis_okt = Convert.ToInt16(ips, 2) & Convert.ToInt16(ids, 2);
                }

                calc_adresse += ergebnis_okt.ToString() + ".";
            }

            calc_adresse = calc_adresse.Remove(calc_adresse.Length - 1);

            return calc_adresse;
        }

        public static double range_calc(string snm)
        {
            int zero_count = snm.Replace(".", "").Count('0'); // Zählt 0 für Host-Teil

            return Math.Pow(2, zero_count) - 2;
        }

        public static string host_calc(string adresse, bool first)
        {
            string host = string.Empty;
            string zahl = Convert.ToString(1, 2).PadLeft(8, '0');
            var okt_list = new List<string>();
            var last_oktett = adresse.Split('.')[3];

            for (int i = 0; i < 4; i++)
            {
                okt_list.Add(adresse.Split('.')[i]); // Splittet Adresse in Oktette
            }

            if (first)
            {
                zahl = (int.Parse(last_oktett) + 1).ToString(); // First Host = Netz-ID +1
            }
            else
            {
                zahl = (int.Parse(last_oktett) - 1).ToString(); // Last Host = BCA - 1
            }

            return okt_list[0] + "." + okt_list[1] + "." + okt_list[2] + "." + zahl; // Setzt Hosts zusammen
        }
    }
}