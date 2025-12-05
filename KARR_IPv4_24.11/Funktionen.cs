using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KARR_IPv4_24._11
{
    public static class Funktionen // static, damit Klasse nicht instanziert werden muss
    {
        public static string AdressenCalc(List<byte> ipOktette, List<byte> snmOktette, bool broadcast)
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

                if (i != ipOktette.Count - 1) calc_adresse += ergebnis_okt.ToString() + "."; else calc_adresse += ergebnis_okt.ToString();
            }

            return calc_adresse;
        }

        public static char NetClassIdentifier(Adresse NetzID)
        {
            int dot = int.Parse(NetzID.DecAdresse!.Remove(NetzID.DecAdresse.IndexOf("."))); // dot = 1. Oktett

            switch (dot)
            {
                case < 128:
                    return 'A';
                case < 192:
                    return 'B';
                case < 224:
                    return 'C';
                case < 240:
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
                               $"\nCIDR-Suffix: {Subnetmask.BinAdresse.Count('1')}" +
                               $"\nNetzklasse: {Netclass}\n");
        }

        public static void EnterAdresse(ref Adresse ipAdresse, ref Adresse snmAdresse)
        {
            string inputString;
            char input;
            int inputZahl;

            while (true)
            {
                Console.Write("Bitte die IP-Adresse eingeben: ");
                inputString = Console.ReadLine()!;

                Match match = Regex.Match(inputString!, @"^(((?!25?[6 - 9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
                if (match.Success)
                {
                    ipAdresse = new Adresse(inputString!, true);
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
                    inputString = Console.ReadLine()!;
                    string[] Okt = inputString.Split('.');
                    bool properSubnet = false;

                    for (int i = 1; i < Okt.Count() - 1; i++)
                    {
                        if (int.Parse(Okt[i]) <= int.Parse(Okt[i-1])) // Prüft ob das hintere Oktett kleiner gleich dem vorderen Oktett ist
                        {
                            if (int.Parse(Okt[i-1]) != 255 && int.Parse(Okt[i]) > 0) // Wenn vorderes Oktett < 255, MUSS hinteres Oktett 0 sein
                            {
                                Console.WriteLine("Ungültige Subnetzmaske!");
                                properSubnet = false;
                                break;
                            }
                            properSubnet = true;
                        }
                        else
                        {
                            Console.WriteLine("Ungültige Subnetzmaske!");
                            break;
                        }
                    }

                    Match match = Regex.Match(inputString!, @"^(((?!25?[6-9])[12]\d|[1-9])?\d\.?\b){4}$"); // Regex von Stackoverflow, prüft ob Input gültig
                    if (match.Success && properSubnet)
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
                    catch (FormatException) // Sollten Buchstaben als Input gegeben werden
                    {
                        Console.WriteLine("Input-Fehler: Bitte eine Zahl eintippen.");
                    }
                    catch(Exception)
                    {
                        Console.WriteLine("Unbekannter Fehler: Kein gültiger Wert angegeben.");
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

        public static void SubnetAusgabe(List<Adresse> subnetIDs, Adresse subnetMask)
        {
            int count = 1;

            for (int i = 0; i < subnetIDs.Count; i++)
            {
                Console.WriteLine($"{count}. Subnetzwerk:");
                var tmp = subnetIDs[i];               // ansonsten gibt ref einen Fehler, weil Prop ohne Verweisrückgabe
                CalcNetwork(ref tmp, ref subnetMask);
                count++;
            }
        }

        public static void SubnetCalc(ref Adresse ipAdresse, ref Adresse snmAdresse, bool OptA)
        {
            Adresse SubnetMask;
            List<Adresse> SubnetIDs = new();

            char input;
            int inputZahl;
            int storedCidr = snmAdresse.BinAdresse.Count('1');

            if (ipAdresse.DecAdresse != "" && snmAdresse.DecAdresse != "")
            {
                while (true)
                {
                    Console.Write("Möchten Sie die Werte ihrer vorherigen Eingabe übernehmen? (Y/N): ");
                    input = Console.ReadKey().KeyChar;

                    if (input == 'Y' || input == 'y')
                    {
                        break;
                    }
                    if (input == 'N' || input == 'n')
                    {
                        Console.WriteLine();
                        EnterAdresse(ref ipAdresse, ref snmAdresse);
                        storedCidr = snmAdresse.BinAdresse.Count('1');
                        break;
                    }
                }
            }
            else
            {
                EnterAdresse(ref ipAdresse, ref snmAdresse);
                storedCidr = snmAdresse.BinAdresse.Count('1');
            }

            if (OptA)
            {
                while (true)
                {
                    Console.Write("\nBitte die Anzahl der gewünschten Subnetze angeben: ");
                    Console.WriteLine();

                    try
                    {
                        inputZahl = int.Parse(Console.ReadLine()!);

                        if (inputZahl <= Math.Pow(2, snmAdresse.BinAdresse.Count('0')) && inputZahl > 0) // Math.Log2(inputZahl) um auf die Benötigten Bits zu kommen
                        {
                            SubnetMask = new Adresse((int)Math.Ceiling(Math.Log2(inputZahl)) + storedCidr);
                            double HostRange = SubnetMask.RangeCalc();

                            if (HostRange > 0)
                            {
                                SubnetIDs = snmAdresse.SubnetCalc(inputZahl, ipAdresse, storedCidr);

                                SubnetAusgabe(SubnetIDs, SubnetMask);

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
                        Console.WriteLine("Fehler: Es wurde keine Zahl eingetippt.");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Unbekannter Fehler: Kein gültiger Wert angegeben.");
                    }
                }
            }

            else
            {
                while (true)
                {
                    Console.Write("\nBitte die Anzahl der gewünschten Hosts pro Subnetz angeben: ");
                    try
                    {
                        inputZahl = int.Parse(Console.ReadLine()!) + 2;

                        if (Math.Ceiling(Math.Log2(inputZahl)) <= (32 - storedCidr - 1) && inputZahl > 0) // Math.Log2(inputZahl) um auf die Benötigten Bits zu kommen
                        {
                            double BenötigteBits = Math.Floor(32 - Math.Log2(inputZahl) - storedCidr);
                            SubnetMask = new Adresse((int)BenötigteBits + storedCidr);
                            double HostRange = SubnetMask.RangeCalc();

                            if (HostRange > 0)
                            {
                                double AnzahlSubnetze = Math.Pow(2, BenötigteBits);
                                SubnetIDs = snmAdresse.SubnetCalc(AnzahlSubnetze, ipAdresse, storedCidr);

                                SubnetAusgabe(SubnetIDs, SubnetMask);
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"\nZu wenige Adressen, es werden mind. 4 benötigt.");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("0 ist kein gültiger Wert. (Mind. 4 Adressen pro Netz werden benötigt)\n");
                        }
                    }
                    catch (FormatException) // Sollten Buchstaben als Input gegeben werden
                    {
                        Console.WriteLine("Es wurde keine Zahl eingetippt.");
                    }
                }
            }
        }
    }
}
