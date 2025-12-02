using KARR_IPv4_24._11;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        Adresse decIp;
        Adresse decSnm;

        string inputString;
        char input; // \0 char-Äquivalent zu string.Empty
        int inputZahl;
        int storedCidr = 0;

        while (true)
        {
            Console.Write("Bitte die IP-Adresse eingeben: ");
            inputString = Console.ReadLine();

            Match match = Regex.Match(inputString!, @"^(((?!25?[6 - 9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
            if (match.Success)
            {
                decIp = new Adresse(inputString!, true);
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
                    decSnm = new Adresse(inputString!, true);
                    storedCidr = decSnm.BinAdresse.Count('1');
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
                Console.Write("\nBitte die CIDR-Suffix angeben: /");
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

        Adresse decNetzId = new Adresse(AdressenCalc(decIp.DecOktette!, decSnm.DecOktette!, false), true);
        Adresse decBroadcast = new Adresse(AdressenCalc(decIp.DecOktette!, decSnm.DecOktette!, true), true);
        double hostRange = decSnm.RangeCalc();
        Adresse firstHost = decNetzId + 1;
        Adresse lastHost = decBroadcast - 1;

        char Klasse = NetClassIdentifier(decNetzId);

        PrintBlock(decNetzId, decBroadcast, hostRange, firstHost, lastHost, decSnm, Klasse);

        while (true)
        {
            Console.Write("\nSoll das Netz in Subnetze aufgeteilt werden? (Y/N) ");
            input = Console.ReadKey().KeyChar;
            Console.WriteLine();

            if (input == 'Y' || input == 'y')
            {
                while (true)
                {
                    Console.Write("\nSollen die Subnetze anhand der Hosts oder der Anzahl der Netze aufgeteilt werden? (H/A)\n H - Anzahl der Hosts\n A - Anzahl der Netze\nAuswahl: ");
                    input = Console.ReadKey().KeyChar;
                    Console.WriteLine();

                    if (input == 'H' || input == 'h' || input == 'a' || input == 'A')
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Bitte entweder mit Y oder N Auswahl treffen.\n");
                    }
                }
                break;
            }

            else if (input == 'N' || input == 'n')
            {
                Console.WriteLine("\nProgramm beendet.\nFenster kann jetzt geschlossen werden.");
                break;
            }

            else
            {
                Console.WriteLine("Bitte entweder mit Y oder N Auswahl treffen.\n");
            }
        }
        if(input == 'A' || input == 'a')
        {
            Adresse SubnetMask;
            List<Adresse> SubnetIDs = new();

            while (true)
            {
                Console.Write("\nBitte die Anzahl der gewünschten Subnetze angeben: ");
                try
                {
                    inputZahl = int.Parse(Console.ReadLine()!);

                    if (inputZahl <= Math.Pow(2, decSnm.BinAdresse.Count('0')) && inputZahl >= 0) // Math.Log2(inputZahl) um auf die Benötigten Bits zu kommen
                    {
                        SubnetMask = new Adresse((int)Math.Ceiling(Math.Log2(inputZahl))+storedCidr);
                        double HostRange = SubnetMask.RangeCalc();

                        if (HostRange > 0)
                        {
                            SubnetIDs = decSnm.SubnetCalc(inputZahl, decNetzId, storedCidr);
                            Adresse SubnetBroadcast = new();
                            Adresse SubnetFirstHosts = new();
                            Adresse SubnetLastHosts = new();

                            Klasse = NetClassIdentifier(SubnetIDs[0]);

                            int count = 1;

                            foreach (Adresse adresse in SubnetIDs)
                            {
                                SubnetBroadcast = new Adresse(AdressenCalc(adresse.DecOktette, SubnetMask.DecOktette, true), true);
                                SubnetFirstHosts = adresse + 1;
                                SubnetLastHosts = SubnetBroadcast - 1;

                                Console.WriteLine($"\n{count}. Subnetz:");
                                PrintBlock(adresse, SubnetBroadcast, HostRange, SubnetFirstHosts, SubnetLastHosts, SubnetMask, Klasse);
                                count++;
                            }

                            Console.WriteLine("\nProgramm beendet.\nFenster kann jetzt geschlossen werden.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"\nZu viele Netze, es wären keine Hosts verfügbar! (Maximal Anzahl = {Math.Pow(2, decSnm.BinAdresse.Count('0')-2)})");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Kein plausibler Wert eingegeben.\n");
                    }
                }
                catch (FormatException) // Sollten Buchstaben als Input gegeben werden
                {
                    Console.WriteLine("Es wurde keine Zahl eingetippt.");
                }
            }
        }
        if (input == 'H' || input == 'h') // Muss noch ausprogrammiert werden
        {
            Adresse SubnetMask;
            List<Adresse> SubnetIDs = new();

            while (true)
            {
                Console.Write("\nBitte die Anzahl der gewünschten Adressen pro Subnetz (= Hosts + 2) angeben: ");
                try
                {
                    inputZahl = int.Parse(Console.ReadLine()!);

                    if (Math.Ceiling(Math.Log2(inputZahl)) <= (32 - storedCidr - 1) && inputZahl >= 0) // Math.Log2(inputZahl) um auf die Benötigten Bits zu kommen
                    {
                        double BenötigteBits = Math.Floor(32 - Math.Log2(inputZahl) - storedCidr);
                        SubnetMask = new Adresse((int)BenötigteBits + storedCidr);
                        double HostRange = SubnetMask.RangeCalc();

                        if (HostRange > 0)
                        {                            
                            SubnetIDs = decSnm.SubnetCalc(SubnetTranslate(HostRange), decNetzId, storedCidr);
                            Adresse SubnetBroadcast = new();
                            Adresse SubnetFirstHosts = new();
                            Adresse SubnetLastHosts = new();

                            Klasse = NetClassIdentifier(SubnetIDs[0]);

                            int count = 1;

                            foreach (Adresse adresse in SubnetIDs)
                            {
                                SubnetBroadcast = new Adresse(AdressenCalc(adresse.DecOktette, SubnetMask.DecOktette, true), true);
                                SubnetFirstHosts = adresse + 1;
                                SubnetLastHosts = SubnetBroadcast - 1;

                                Console.WriteLine($"\n{count}. Subnetz:");
                                PrintBlock(adresse, SubnetBroadcast, HostRange, SubnetFirstHosts, SubnetLastHosts, SubnetMask, Klasse);
                                count++;
                            }

                            Console.WriteLine("\nProgramm beendet.\nFenster kann jetzt geschlossen werden.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"\nZu viele Netze, es wären keine Hosts verfügbar! (Maximal Anzahl = {Math.Pow(2, decSnm.BinAdresse.Count('0') - 2)})");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("0 ist kein gültiger Wert. (Mind. 1 Subnetz wird benötigt)\n");
                    }
                }
                catch (FormatException) // Sollten Buchstaben als Input gegeben werden
                {
                    Console.WriteLine("Es wurde keine Zahl eingetippt.");
                }
            }
        }
    }
    public static string AdressenCalc(List <byte> ipOktette, List <byte> snmOktette, bool broadcast = false)
    {
        string calc_adresse = string.Empty;

        for (int i = 0; i < ipOktette.Count; i++)
        {
            int ergebnis_okt;

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

    public static char NetClassIdentifier(Adresse NetzID)
    {
        int dot = NetzID.DecAdresse.IndexOf(".");
        switch (byte.Parse(NetzID.DecAdresse.Remove(dot)))
        {
            case <128:
                return 'A';
            case <192:
                return 'B';
            case <224:
                return 'C';
            case <240:
                return 'D';
            default:
                return 'E';
        }
    }

    public static void PrintBlock(Adresse decNetzId, Adresse decBroadcast, double hostRange, Adresse firstHost, Adresse lastHost, Adresse Subnetmask, char Netclass)
    {
        Console.WriteLine($"\nNetz-ID: {decNetzId.DecAdresse} ({decNetzId.BinAdresse})" +
                           $"\nBroadcastadresse: {decBroadcast.DecAdresse} ({decBroadcast.BinAdresse})" +
                           $"\nHost-Range: {hostRange}" +
                           $"\nErster Host: {firstHost.DecAdresse} ({firstHost.BinAdresse})" +
                           $"\nLetzter Host: {lastHost.DecAdresse} ({lastHost.BinAdresse})" +
                           $"\nSubnetzmaske: {Subnetmask.DecAdresse} ({Subnetmask.BinAdresse})" +
                           $"\nCIDR-Präfix: {Subnetmask.BinAdresse.Count('1')}" +
                           $"\nNetzklasse: {Netclass}");
    }

    public static double SubnetTranslate(double Range)
    {
        switch(Range)
        {
            case <= 2:
                return 64;
            case <= 6:
                return 32;
            case <= 14:
                return 16;
            case <= 30:
                return 8;
            case <= 62:
                return 4;
            default:
                return 2;
        };
    }
}