using System;
using System.Collections.Generic;
using System.Text;

namespace KARR_IPv4_24._11
{
    internal class Cidr
    {
        public int inputCidr { get; private set; }
        public string binCidr { get; private set; }

        public Cidr() 
        {
            this.inputCidr = 0;
            this.binCidr = "";
        }

        public Cidr(int cidr)
        {
            this.inputCidr = cidr;
            this.binCidr = CIDR_to_bin(this.inputCidr);
        }
        private string CIDR_to_bin(int cidr)
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
