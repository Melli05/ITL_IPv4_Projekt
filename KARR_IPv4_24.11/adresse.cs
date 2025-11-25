using System;
using System.Collections.Generic;
using System.Text;

namespace KARR_IPv4_24._11
{
    public class Adresse
    {
        public string oktett1 { get; set; }
        public string oktett2 { get; set; }
        public string oktett3 { get; set; }
        public string oktett4 { get; set; }
        public Adresse()
        {
            this.oktett1 = string.Empty;
            this.oktett2 = string.Empty;
            this.oktett3 = string.Empty;
            this.oktett4 = string.Empty;
        }

        public Adresse(string erstesOktett, string zweitesOktett, string drittesOktett, string viertesOktett)
        {
            this.oktett1 = Convert.ToString(int.Parse(erstesOktett), 2);
            this.oktett2 = Convert.ToString(int.Parse(zweitesOktett), 2);
            this.oktett3 = Convert.ToString(int.Parse(drittesOktett), 2);
            this.oktett4 = Convert.ToString(int.Parse(viertesOktett), 2);
        }

    }
}
