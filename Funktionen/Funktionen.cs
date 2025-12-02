namespace Funktionen
{
    public class Funktionen
    {
        public static string AdressenCalc(List<byte> ipOktette, List<byte> snmOktette, bool broadcast = false)
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
            int dot = NetzID.DecAdresse.IndexOf(".");
            switch (byte.Parse(NetzID.DecAdresse.Remove(dot)))
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
                               $"\nCIDR-Präfix: {Subnetmask.BinAdresse.Count('1')}" +
                               $"\nNetzklasse: {Netclass}");
        }
    }
}
