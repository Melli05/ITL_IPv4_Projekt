using System;
using System.Collections.Generic;
using System.Text;

namespace KARR_IPv4_24._11
{
    internal class Adresse
    {
        public string? decAdresse {  get; private set; }
        public string? binAdresse { get; private set; }
        public List <byte>? decOktette { get; private set; }

        // Konstruktoren
        public Adresse() { }
        public Adresse(string adresse)
        {
            this.decAdresse = adresse;
            this.binAdresse = ConvertDecAndBin(adresse.Split('.'), true);
            this.decOktette = new List<byte>();
            foreach(string oktett in this.decAdresse.Split('.'))
            {
                decOktette.Add(Convert.ToByte(oktett));
            }
        }
        public Adresse(int cidr)
        {
            this.binAdresse = CidrToBin(cidr);
            this.decAdresse = ConvertDecAndBin(this.binAdresse.Split('.'), false);
            this.decOktette = new List<byte>();
            foreach (string oktett in this.decAdresse.Split('.'))
            {
                decOktette.Add(Convert.ToByte(oktett));
            }
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
    }
}
