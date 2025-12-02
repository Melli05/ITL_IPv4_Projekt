using KARR_IPv4_24._11;
using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        Adresse decIp = new();
        Adresse decSnm = new();
        int MenueAuwahl;

        while (true)
        {
            try
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
                        Funktionen.EnterAdresse(ref decIp, ref decSnm);
                        Funktionen.CalcNetwork(ref decIp, ref decSnm);
                        Funktionen.Return();
                        break;
                    case 2:
                        Funktionen.SubnetCalc(ref decIp, ref decSnm, true);
                        Funktionen.Return();
                        break;
                    case 3:
                        Funktionen.SubnetCalc(ref decIp, ref decSnm, false);
                        Funktionen.Return();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Fehler: Keine gültige Auswahl getroffen!\n");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.Clear();
                Console.WriteLine("Fehler: Eingegebener Wert konnte nicht verarbeitet werdenw.\n");
            }
            catch (Exception)
            {
                Console.Clear();
                Console.WriteLine("Unbekannter Fehler: bitte Eingabe überprüfen.\n");
            }

        } 
    }
}