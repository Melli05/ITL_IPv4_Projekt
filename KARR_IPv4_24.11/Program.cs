using KARR_IPv4_24._11;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        Adresse decIp = new();
        Adresse decSnm = new();

        string inputString;
        char input;
        int inputZahl;
        int storedCidr = 0;
        int MenueAuwahl;

        while (true)
        {
            Console.Write("Bitte Auswahl treffen:" +
                            "\n1 - IP-Analyse" +
                            "\n2 - Subnetze anhand der Anzahl der Subnetze berechnen" +
                            "\n3 - Subnetze anhand der Anzahl der Hosts berechnen" +
                            "\n0 - Programm beenden" +
                            "\nAuswahl: ");
            MenueAuwahl = int.Parse(Console.ReadLine()!);
            Console.Clear();

            switch (MenueAuwahl)
            {
                case 0:
                    Environment.Exit(0);
                    break;
                case 1:
                    EnterAdresse(ref decIp, ref decSnm);
                    CalcNetwork(ref decIp, ref decSnm);
                    Return();
                    break;
                case 2:
                    OptASubnetCalc(ref decIp, ref decSnm);
                    Return();
                    break;
                case 3:
                    Return();
                    break;
                default:
                    Console.WriteLine("Keine gültige Auswahl getroffen!\n");
                    break;
            }
        }
    }
    public static void EnterAdresse(ref Adresse ipAdresse, ref Adresse snmAdresse)
    {
        string inputString;
        char input;
        int inputZahl;

        while (true)
        {
            Console.Write("Bitte die IP-Adresse eingeben: ");
            inputString = Console.ReadLine();

            Match match = Regex.Match(inputString!, @"^(((?!25?[6 - 9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
            if (match.Success)
            {
                ipAdresse = new Adresse(inputString!, true);
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
                    snmAdresse = new Adresse(inputString!, true);
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
                        snmAdresse = new Adresse(inputZahl);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Wert zwischen 0 und 32 angeben.\n");
                    }
                }
                catch (Exception x) // Sollten Buchstaben als Input gegeben werden
                {
                    Console.WriteLine("Fehler: " + x.Message);
                }
            }
        }
    }

    public static void CalcNetwork(ref Adresse ipAdresse, ref Adresse snmAdresse)
    {
        Adresse decNetzId = new Adresse(Funktionen.AdressenCalc(ipAdresse.DecOktette!, snmAdresse.DecOktette!, false), true);
        Adresse decBroadcast = new Adresse(Funktionen.AdressenCalc(ipAdresse.DecOktette!, snmAdresse.DecOktette!, true), true);
        double hostRange = snmAdresse.RangeCalc();
        Adresse firstHost = decNetzId + 1;
        Adresse lastHost = decBroadcast - 1;

        char Klasse = Funktionen.NetClassIdentifier(decNetzId);

        Funktionen.PrintBlock(decNetzId, decBroadcast, hostRange, firstHost, lastHost, snmAdresse, Klasse);
    }

    public static void Return()
    {
        Console.WriteLine("Beliebigen Knopf drücken um zum Menü zurückzukehren.");
        Console.ReadKey();
        Console.Clear();
    }

    public static void OptASubnetCalc(ref Adresse ipAdresse, ref Adresse snmAdresse)
    {
        Adresse SubnetMask;
        List<Adresse> SubnetIDs = new();
        char input;
        int inputZahl;

        while (true)
        {
            if (ipAdresse.DecAdresse != "" && snmAdresse.DecAdresse != "")
            {
                while (true)
                {
                    Console.Write("\nMöchten Sie die Werte ihrer vorherigen Eingabe übernehmen? (Y/N): ");
                    input = Console.ReadKey().KeyChar;

                    if (input == 'Y' || input == 'y')
                    {
                        break;
                    }
                    if (input == 'N' || input == 'n')
                    {
                        Console.WriteLine();
                        EnterAdresse(ref ipAdresse, ref snmAdresse);
                        break;
                    }
                }  
            }
            else
            {
                EnterAdresse(ref ipAdresse, ref snmAdresse);
            }

            Console.Write("\nBitte die Anzahl der gewünschten Subnetze angeben: ");
            Console.WriteLine();

            try
            {
                inputZahl = int.Parse(Console.ReadLine()!);
                int storedCidr = snmAdresse.BinAdresse.Count('1');

                if (inputZahl <= Math.Pow(2, snmAdresse.BinAdresse.Count('0')) && inputZahl >= 0) // Math.Log2(inputZahl) um auf die Benötigten Bits zu kommen
                {
                    SubnetMask = new Adresse((int)Math.Ceiling(Math.Log2(inputZahl)) + storedCidr);
                    double HostRange = SubnetMask.RangeCalc();

                    if (HostRange > 0)
                    {
                        SubnetIDs = snmAdresse.SubnetCalc(inputZahl, ipAdresse, storedCidr);

                        int count = 1;

                        for (int i = 0; i < SubnetIDs.Count; i++)
                        {
                            Console.WriteLine($"{count}. Subnetzwerk:");
                            var tmp = SubnetIDs[i];               // ansonsten gibt ref einen fehler 
                            CalcNetwork(ref tmp, ref SubnetMask);
                            count++;
                        }

                        Console.WriteLine("\nProgramm beendet.\nFenster kann jetzt geschlossen werden.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"\nZu viele Netze, es wären keine Hosts verfügbar! (Maximal Anzahl = {Math.Pow(2, snmAdresse.BinAdresse.Count('0') - 2)})");
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
}