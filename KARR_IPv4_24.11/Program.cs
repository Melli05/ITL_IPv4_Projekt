internal class Program
{
    private static void Main(string[] args)
    {
        var snm_dec = string.Empty;
        var snm_bin = string.Empty;
        var input = '\0';

        Console.Write("Bitte die IP-Adresse eingeben: ");
        var ip_dec = Console.ReadLine();

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
        }

        if (input == 'N' || input == 'n')
        {
            Console.Write("Bitte die Subnetzmaske angeben: ");
            snm_dec = Console.ReadLine();
            snm_bin = dez_to_bin(snm_dec.Split('.'));
        }
        else
        {
            Console.Write("Bitte die CIDR-Suffix angeben: /");
            var cidr = Console.ReadLine();
            snm_bin = CIDR_to_bin(int.Parse(cidr));
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
            adresse += Convert.ToString(int.Parse(oktett), 2).PadLeft(8, '0') + ".";
        }

        return adresse.Remove(adresse.Length - 1);
    }

    public static string CIDR_to_bin(int cidr)
    {
        string snm = string.Empty;

        snm += new string('1', cidr);
        snm += new string('0', 32 - cidr);

        snm = snm.Insert(24, ".");
        snm = snm.Insert(16, ".");
        snm = snm.Insert(8, ".");

        return snm;
    }

    public static string adressen_calc(string ip, string snm, bool broadcast = false)
    {
        string calc_adresse = string.Empty;

        var ip_split = ip.Split('.');
        var snm_split = snm.Split('.');
        
        for(int i = 0; i < ip_split.Length; i++)
        {
            var ips = ip_split[i];
            var ids = snm_split[i];
            int ergebnis_okt = 0;
            int eins = 1;

            if (broadcast)
            {
                ergebnis_okt = (Convert.ToInt16(ips, 2) | (~Convert.ToInt16(ids, 2) & 0xFF)); // 0xFF, damit nur 8 Bit behalten werden, weil ~ aus int ansonsten eine 32-Bit Zahl wird
            }
            else 
            {
                ergebnis_okt = Convert.ToInt16(ips, 2) & Convert.ToInt16(ids, 2);
            }


            calc_adresse += ergebnis_okt.ToString() + ".";
        }

        calc_adresse = calc_adresse.Remove(calc_adresse.Length - 1);

        return calc_adresse;
    }

    public static double range_calc(string snm)
    {
        int zero_count = snm.Replace(".", "").Count('0');
        
        return Math.Pow(2, zero_count)-2;
    }

    public static string host_calc(string adresse, bool first)
    {
        string host = string.Empty;
        string zahl = Convert.ToString(1, 2).PadLeft(8, '0');
        var okt_list = new List<string>();

        var last_oktett = adresse.Split('.')[3];
        for( int i = 0; i < 4; i++)
        {
            okt_list.Add(adresse.Split('.')[i]);
        }

        if (first)
        {
            zahl = (int.Parse(last_oktett) + 1).ToString();
        }
        else
        {
            zahl = (int.Parse(last_oktett) - 1).ToString();
        }

        return okt_list[0]+ "." +okt_list[1]+ "." +okt_list[2]+ "." +zahl;
    }
}