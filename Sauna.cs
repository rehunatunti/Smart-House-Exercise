using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AlytaloWPF
{
    public class Sauna
    {
        // ominaisuudet
        public bool Switched { get; set; }
        // public int CurrentTemp { get; set; }
        

        private Thermostat SaunaThermostat = new Thermostat(); // saunaluokka omistaa olion SaunaThermostat
                                                               // ja on tietoturvan takia private, voidaan muokata vain sauna-luokan sisältä 

        public void AsetaSaunanLampotila(double saunanLampotila)
        {
            SaunaThermostat.Temperature = saunanLampotila; // välittäjämetodi, joka vie käyttöliittymästä sauna-olion tiedon termostaattioliolle
        }

        public double getSaunanLpt()
        {
            return SaunaThermostat.Temperature; // koska ei ole void, tarvii paluuarvoa varten returnin
        }
        
    }
}
