using KARR_IPv4_24._11;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        Adresse decIp = new();
        Adresse decSnm = new();

        var inputString = string.Empty;
        var input = '\0'; // \0 char-Äquivalent zu string.Empty
        int inputZahl = -1;
        int storedCidr = 0;

        while (true)
        {
            Console.Write("Bitte die IP-Adresse eingeben: ");
            inputString = Console.ReadLine();

            Match match = Regex.Match(inputString!, @"^(((?!25?[6 - 9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
            if (match.Success)
            {
                decIp = new Adresse(inputString!);
                inputString = string.Empty;
                break;
            }
            else
            {
                Console.WriteLine("Bitte eine gültige IP-Adresse im Format XXX.XXX.XXX.XXX angeben!\n");
            }
        }

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
                inputString = Console.ReadLine();

                Match match = Regex.Match(inputString!, @"^(((?!25?[6-9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
                if (match.Success)
                {
                    decSnm = new Adresse(inputString!);
                    break;
                }
                else
                {
                    Console.WriteLine("Bitte eine gültige Subnetzmaske im Format XXX.XXX.XXX.XXX angeben!\n");
                }
            }
        }

        else
        {
            while (true)
            {
                Console.Write("Bitte die CIDR-Suffix angeben: /");
                try
                {
                    inputZahl = int.Parse(Console.ReadLine()!);

                    if (inputZahl <= 32 && inputZahl >= 0)
                    {
                        storedCidr = inputZahl;
                        decSnm = new Adresse(inputZahl);
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
        }

        Adresse decNetzId = new Adresse(AdressenCalc(decIp.decOktette!, decSnm.decOktette!, false));
        Adresse decBroadcast = new Adresse(AdressenCalc(decIp.decOktette!, decSnm.decOktette!, true));
        var host_range = RangeCalc(decSnm.binAdresse!);
        Adresse firstHost = new Adresse(HostCalc(decNetzId.decOktette!, true));
        Adresse lastHost = new Adresse(HostCalc(decBroadcast.decOktette!, false));

        Console.WriteLine( $"\nNetz-ID: {decNetzId.decAdresse} ({decNetzId.binAdresse})" +
                           $"\nBroadcastadresse: {decBroadcast.decAdresse} ({decBroadcast.binAdresse})" +
                           $"\nHost-Range: {host_range}" +
                           $"\nErster Host: {firstHost.decAdresse} ({firstHost.binAdresse})" +
                           $"\nLetzter Host: {lastHost.decAdresse} ({lastHost.binAdresse})");

        while (true)
        {
            Console.Write("\nSoll das Netz in Subnetze aufgeteilt werden? (Y/N) ");
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
        if(input == 'Y' || input == 'y')
        {
            Adresse SubnetMask = new();

            while (true)
            {
                Console.Write("Bitte die Anzahl der gewünschten Subnetze angeben: ");
                try
                {
                    inputZahl = int.Parse(Console.ReadLine()!);

                    if (Math.Log2(inputZahl) <= decSnm.binAdresse.Count('0') && inputZahl >= 0) // Math.Log2(inputZahl) um auf die Benötigten Bits zu kommen
                    {
                        SubnetMask = new Adresse(inputZahl+storedCidr);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Kein plausibler Wert eingegeben.\n");
                    }
                }
                catch (Exception x) // Sollten Buchstaben als Input gegeben werden
                {
                    Console.WriteLine("Fehler: " + x.Message);
                }
            }
        }
    }
    
    public static string AdressenCalc(List <byte> ipOktette, List <byte> snmOktette, bool broadcast = false)
    {
        string calc_adresse = string.Empty;

        for (int i = 0; i < ipOktette.Count; i++)
        {
            int ergebnis_okt = 0;

            if (broadcast)
            {
                // Berechnet Broadcast-Adresse durch Invertierten der SNM und bitwise OR
                ergebnis_okt = (byte)(ipOktette[i] | ~snmOktette[i]); // (byte) weil ~ aus byte ansonsten eine 32-Bit Zahl macht
            }
            else
            {
                // Berechnet Netz-ID durch bitwise AND
                ergebnis_okt = ipOktette[i] & snmOktette[i];
            }

            if (i != ipOktette.Count - 1) calc_adresse += ergebnis_okt.ToString() + ".";  else calc_adresse += ergebnis_okt.ToString();
        }

        return calc_adresse;
    }

    public static double RangeCalc(string snm)
    {
        int zero_count = snm.Replace(".", "").Count('0'); // Zählt 0 für Host-Teil

        return Math.Pow(2, zero_count) - 2;
    }

    public static string HostCalc(List <byte> oktette, bool first)
    {
        string zahl = Convert.ToString(1, 2).PadLeft(8, '0');

        if (first)
        {
            zahl = (oktette[oktette.Count - 1] + 1).ToString(); // First Host = Netz-ID +1
        }
        else
        {
            zahl = (oktette[oktette.Count - 1] - 1).ToString(); // Last Host = BCA - 1
        }

        return oktette[0] + "." + oktette[1] + "." + oktette[2] + "." + zahl;
    }
}