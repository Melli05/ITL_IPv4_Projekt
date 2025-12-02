using System;
using System.Collections.Generic;
using System.Text;

namespace KARR_IPv4_24._11
{
    internal class Adresse
    {
        public string? DecAdresse {  get; private set; }
        public string? BinAdresse { get; private set; }
        public List <byte>? DecOktette { get; private set; }

        // Konstruktoren
        public Adresse() { DecAdresse = ""; BinAdresse = ""; DecOktette = []; }
        public Adresse(string adresse, bool DecToBin)
        {
            if(DecToBin)
            {
                this.DecAdresse = adresse;
                this.BinAdresse = ConvertDecAndBin(adresse.Split('.'), DecToBin);
            }
            else
            {
                this.BinAdresse = adresse;
                this.DecAdresse = ConvertDecAndBin(adresse.Split('.'), DecToBin);
            }
            
            this.DecOktette = new();

            foreach (string oktett in this.DecAdresse.Split('.'))
            {
                DecOktette.Add(Convert.ToByte(oktett));
            }
        }
        public Adresse(int cidr)
        {
            this.BinAdresse = CidrToBin(cidr);
            this.DecAdresse = ConvertDecAndBin(this.BinAdresse.Split('.'), false);
            this.DecOktette = new();
            foreach (string oktett in this.DecAdresse.Split('.'))
            {
                DecOktette.Add(Convert.ToByte(oktett));
            }
        }
        // Operator Overload
        public static Adresse operator+(Adresse adresse, byte zahl)
        {
            Adresse rückgabe = new();
            
            for(int i = 0; i < adresse.DecOktette.Count; i++)
            {
                if(i == 3)
                {
                    rückgabe.DecOktette.Add((byte)(adresse.DecOktette[i] + zahl));
                }
                else
                {
                    rückgabe.DecOktette.Add(adresse.DecOktette[i]);
                }
            }
            rückgabe.DecAdresse = $"{rückgabe.DecOktette[0]}.{rückgabe.DecOktette[1]}.{rückgabe.DecOktette[2]}.{rückgabe.DecOktette[3]}";
            rückgabe.BinAdresse = rückgabe.ConvertDecAndBin(rückgabe.DecAdresse.Split('.'), true);
            return rückgabe;
        }

        public static Adresse operator -(Adresse adresse, byte zahl)
        {
            Adresse rückgabe = new();

            for (int i = 0; i < adresse.DecOktette.Count; i++)
            {
                if (i == 3)
                {
                    rückgabe.DecOktette.Add((byte)(adresse.DecOktette[i] - zahl));
                }
                else
                {
                    rückgabe.DecOktette.Add(adresse.DecOktette[i]);
                }
            }
            rückgabe.DecAdresse = $"{rückgabe.DecOktette[0]}.{rückgabe.DecOktette[1]}.{rückgabe.DecOktette[2]}.{rückgabe.DecOktette[3]}";
            rückgabe.BinAdresse = rückgabe.ConvertDecAndBin(rückgabe.DecAdresse.Split('.'), true);
            return rückgabe;
        }

        // Methoden
        private string ConvertDecAndBin(string[] input, bool decToBin)
        {
            string adresse = string.Empty;


            foreach (string oktett in input)
            {
                if (decToBin)
                {
                    adresse += Convert.ToString(int.Parse(oktett), 2).PadLeft(8, '0') + "."; // konvertiert dezimale zu binär
                }
                else
                {
                    adresse += Convert.ToInt16(oktett, 2).ToString() + "."; // konvertiert binär zu dezimal
                }
            }

            return adresse.Remove(adresse.Length - 1);
        }

        private string CidrToBin(int cidr)
        {
            string snm = string.Empty;

            snm += new string('1', cidr); // fügt 1 ein
            snm += new string('0', 32 - cidr); // fügt 0 ein

            snm = snm.Insert(24, ".");
            snm = snm.Insert(16, ".");
            snm = snm.Insert(8, ".");

            return snm;
        }

        public double RangeCalc()
        {
            int zero_count = BinAdresse.Replace(".", "").Count('0'); // Zählt 0 für Host-Teil

            return Math.Pow(2, zero_count) - 2;
        }
    
        public List <Adresse> SubnetCalc(double InputZahl, Adresse NetzID, int Cidr)
        {
            List <Adresse> Subnetze = new();
            List <string> IDs = new();
            List <string> Masks = new();
            int BitLen = 0;
            int anzahl = (int)InputZahl - 1;
            string snm;

            string mask = NetzID.BinAdresse.Replace(".", "").Remove(Cidr);

            for(int i = anzahl; i >=0; i--)
            {
                if(i == anzahl)
                {
                    snm = Convert.ToString(i, 2);
                    BitLen = Convert.ToString(i, 2).Length; // Damit alle Subnetze eine einheitliche Bitlänge haben, ansonsten wären 001, 010 und 100 gleich
                }
                else
                {
                    snm = Convert.ToString(i, 2).PadLeft(BitLen, '0');
                }

                IDs.Add(snm);
            }

            IDs.Reverse();

            foreach(string id in IDs)
            {
                string adresse = Convert.ToString(mask + id).PadRight(32, '0');
                adresse = adresse.Insert(24, ".");
                adresse = adresse.Insert(16, ".");
                adresse = adresse.Insert(8, ".");

                Masks.Add(adresse);
            }

            foreach(string subnetzmaske in Masks)
            {
                Subnetze.Add(new Adresse(subnetzmaske, false));
            }

            return Subnetze; // Gibt die Netz-IDs zurück
        }
    }
}
