using System;
using System.Collections.Generic;
using System.Text;

namespace KARR_IPv4_24._11
{
    internal class Adresse
    {
        public string? decAdresse {  get; private set; }
        public string? binAdresse { get; private set; }
        public byte erstesOktett { get; private set; }
        public byte zweitesOktett { get; private set; }
        public byte drittesOktett { get; private set; }
        public byte viertesOktett { get; private set; }

        public Adresse() { }
        public Adresse(string adresse)
        {
            this.decAdresse = adresse;
            this.binAdresse = DezToBin(adresse.Split('.'));
            this.erstesOktett = Convert.ToByte(this.decAdresse.Split(".")[0]);
            this.zweitesOktett = Convert.ToByte(this.decAdresse.Split(".")[1]);
            this.drittesOktett = Convert.ToByte(this.decAdresse.Split(".")[2]);
            this.viertesOktett = Convert.ToByte(this.decAdresse.Split(".")[3]);
        }

        private string DezToBin(string[] input)
        {
            string adresse = string.Empty;

            foreach (string oktett in input)
            {
                adresse += Convert.ToString(int.Parse(oktett), 2).PadLeft(8, '0') + "."; // konvertiert dezimale Zahlen zu binäre Zahlen
            }

            return adresse.Remove(adresse.Length - 1);
        }
    }
}
