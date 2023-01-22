using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Globalization;

namespace AlytaloWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Tervehdys ja aika

        DateTime tanaan = DateTime.Today;
        string formaattiPvm = "dd.MM.yyyy";
        CultureInfo kulttuuri = CultureInfo.InvariantCulture;
        DateTime aika = DateTime.Now;
        string formaattiAika = "HH:mm:ss";

        // Valot

        Lights[] huoneet; // luodaan taulukko (array) -olio, 

        // Lämpötila
        Thermostat Thermostat = new Thermostat();

        // Sauna
        Sauna sauna = new Sauna();
        private DispatcherTimer kiukaanLammitin = new DispatcherTimer();
        private DispatcherTimer kiukaanJaahdytin = new DispatcherTimer();

        // Loki-ikkuna
        Loki lokiIkkuna = new Loki();
        


        public MainWindow()
        {
            InitializeComponent();
            
            string tervehdys = "Tervehdys! Tänään on " + tanaan.ToString(formaattiPvm) + ". ";
            string startAika = "Ohjelma käynnistetty klo" + aika.ToString(formaattiAika) + ". ";
            lblPaivamaara.Content = tervehdys + startAika;
            lokiIkkuna.VieLokiin(tervehdys + startAika);
            

            // **** VALOT: ****
            // Comboboxin määritys:
            huoneet = new Lights[5]; // luodaan taulukko jossa on 5 alkiota, indexissä numerot vastaavat huoneiden nimiä

            // taulukon alkiot (numerot 0-5) nimetään comboboxissa huoneiksi (olohuone, keittiö jne.)
            cbHuone.Items.Add("Olohuone");
            cbHuone.Items.Add("Keittiö");
            cbHuone.Items.Add("Makuuhuone");
            cbHuone.Items.Add("Kirjasto");
            cbHuone.Items.Add("kellari");
            cbHuone.Items.Add("Eteinen");

            // for-silmukassa taulukko täytetään olioilla
            for (int i = 0; i < huoneet.Count(); i++) 
            {
                huoneet[i] = new Lights();
            }

            // **** LÄMPÖTILA ***


            // **** SAUNA ****
            slKiuas.Minimum = 50;
            slKiuas.Maximum = 100;
            slKiuas.TickFrequency = 1;
            btnSaunaIndicator.Background = Brushes.LightGray;

        }



        // *** VALOT UI ***
        private void btnValotPaalle_Click(object sender, RoutedEventArgs e)
        {
            // Buttonista painamalla valo menee päälle siinä huoneessa, mikä on valittuna cb:ssä (cbHuone.SelectedIndex)
            // luodaan paikallismuuttuja index johon sijoitetaan cb:ssä oleva valinta.
            // Index tulee vastaamaan taulukossa sitä huonetta, mihin cb:n valinta viittaa.
            int index = cbHuone.SelectedIndex;  
            huoneet[index].Switched = true; // ja valo menee päälle siinä huoneessa
            
            int kirkkaus = huoneet[cbHuone.SelectedIndex].Dimmer; // getteri
            slDimmer.Value = kirkkaus;
            txtDimmer.Text = kirkkaus.ToString();

            if (slDimmer.Value > 0)
            {
                lokiIkkuna.VieLokiin(cbHuone.SelectedValue.ToString() + " valo päällä, kirkkaus " + kirkkaus.ToString() + " % ");
            }
            else
            {
                lokiIkkuna.VieLokiin(cbHuone.SelectedValue.ToString() + " valo päällä");
            }
            btnLightIndicator.Background = Brushes.LightYellow; 
        }

        private void btnValotPois_Click(object sender, RoutedEventArgs e)
        {
            huoneet[cbHuone.SelectedIndex].Switched = false;
            lokiIkkuna.VieLokiin(cbHuone.SelectedValue.ToString() + " valo sammutettu");
            btnLightIndicator.Background = Brushes.Gray;
        }

        private void cbHuone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnValotPaalle.IsEnabled = true;
            btnValotPois.IsEnabled = true;
            slDimmer.IsEnabled = true;
            txtDimmer.IsEnabled = true;

            // Tässä cb:n valinnan muutos aiheuttaa muutoksen. 
            // If-lause kysyy mikä huone on valittu ja sen indeksin takana olevan olion (huoneen) Switched-tilasta kysytään onko valot päällä
            // ja asetetaan btnLightIndikaattoriin kyseisen sen mukaan päälle. Elsessä pois päältä, kun Switched on false.
            if (huoneet[cbHuone.SelectedIndex].Switched == true) 
            {
                btnLightIndicator.Background = Brushes.LightGoldenrodYellow; 
            }
            else
            {
                btnLightIndicator.Background = Brushes.Gray;
            }
            int kirkkaus = huoneet[cbHuone.SelectedIndex].Dimmer; // getteri
            slDimmer.Value = kirkkaus;
            txtDimmer.Text = kirkkaus.ToString();

            // ylempänä luettavampi tapa, alla sama toiminta tiiviimmin, mutta vaikealukuisempana
            //slDimmer.Value = huoneet[cbHuone.SelectedIndex].Dimmer; // getteri
            //txtDimmer.Text = huoneet[cbHuone.SelectedIndex].Dimmer.ToString();
            if (huoneet[cbHuone.SelectedIndex].Switched == true)
            {
                lokiIkkuna.VieLokiin(cbHuone.SelectedValue.ToString() + "valo päällä, kirkkaus " + kirkkaus.ToString() + "%");
            }
            else
            {
                lokiIkkuna.VieLokiin(cbHuone.SelectedValue.ToString() + " valo sammutettu.");
            }
            
                        
        }

        private void slDimmer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Koska sliderin muuttuja on float (turhia desimaaleja), luettavuuden takia pitää muuttaa intiksi,
            // eli tehdään paikallismuuttuja i ja (int)slDimmer.Valuessa (int) merkitsee muuttujan castaamista intiksi
            // eli i-muuttujaan on sijoitettu intiksi castattu slDimmer.Value
            // ja textboxiin sijoitetaan tämä paikallismuuttuja i, joka muutetaan stringiksi
            int kirkkaus = (int)slDimmer.Value;
            txtDimmer.Text = kirkkaus.ToString();
            huoneet[cbHuone.SelectedIndex].Dimmer = kirkkaus; // setteri, sliderin arvo menee cbHuone.SelectedIndexin avulla valittuun olioon
            
        }


        // **** TERMOSTAATTI UI ****
        private void btnAsetalpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thermostat.Temperature = double.Parse(txtUusilpt.Text); // tallentuu Thermostatin Temperatureen
                lblLampotila.Content = Thermostat.Temperature;
                double uusiLpt = double.Parse(txtUusilpt.Text);
                if (uusiLpt > 28)
                {
                    lblUusiLptInfo.Content = "Maksimilämpötila on \n 28 astetta.";
                    Thermostat.Temperature = 28;
                    txtUusilpt.Text = Thermostat.Temperature.ToString();
                    lblLampotila.Content = 28;
                }
            }
            catch (Exception)
            {
                lblUusiLptInfo.Content = "* Käytä vain numeroita.";
                txtUusilpt.Text = "";
                lblLampotila.Content = "";
                lblSaunaLpt.Text = "";
                txtUusilpt.Focus();
            }

            lokiIkkuna.VieLokiin("Talon lämpötila on " + Thermostat.Temperature.ToString() + "astetta");
            btnKiuasOn.IsEnabled = true;
            lblSaunaLpt.Text = Thermostat.Temperature.ToString();

        }


        // **** SAUNA UI ****

        
                      
        private void slKiuas_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double lampo = slKiuas.Value;
            lblKiuaslampo.Content = lampo.ToString();
            sauna.AsetaSaunanLampotila(slKiuas.Value); // viedään sliderin value (int-castauksen avulla) AsetaSaunanLampotila metodiin
            
        }
        public void SaunaAjastin1() // olisi pitänyt nimetä "lämmitinajastin"
        {
            kiukaanLammitin.Tick += KiukaanLammitin_Tick;
            kiukaanLammitin.Interval = new TimeSpan(0, 0, 1 );
            
            if (!kiukaanLammitin.IsEnabled)
            {
                kiukaanLammitin.Stop(); // Tämä varmistaa, että lämmitin on varmasti ensin pysäytetty ennen käynnistämistä
                kiukaanJaahdytin.Stop(); // Tämä varmistaa, että jäähdytin on pysäytetty
                kiukaanJaahdytin.Tick -= KiukaanJaahdytin_Tick; // tämä varmistaa, että yhtään jäähdytin_Tickiä ei ole voimassa
                kiukaanLammitin.Start();
            }
            
        }

        
        public void SaunaAjastin2() // olisi pitänyt nimetä "jäähdytinajastin"
        {
            kiukaanJaahdytin.Tick += KiukaanJaahdytin_Tick;
            kiukaanJaahdytin.Interval = new TimeSpan(0, 0, 1);
            
            if (!kiukaanJaahdytin.IsEnabled)
            {
                kiukaanJaahdytin.Stop();
                kiukaanLammitin.Stop();
                kiukaanLammitin.Tick -= KiukaanLammitin_Tick;
                kiukaanJaahdytin.Start();
        
            }
        }
        private void btnKiuasOn_Click(object sender, RoutedEventArgs e)
        {
            lokiIkkuna.VieLokiin("Kiuas päällä, max lämpö " + slKiuas.Value + " astetta.");
            slKiuas.IsEnabled = false;
            btnKiuasOn.IsEnabled = false;
            sauna.Switched = true;
            SaunaAjastin1();
            // Asetetaan huoneen lämpötila saunan alkulämpötilaksi
            sauna.AsetaSaunanLampotila(Thermostat.Temperature); 
            btnKiuasOff.IsEnabled = true;
            btnSaunaIndicator.Background = Brushes.Red;
            kiukaanJaahdytin.IsEnabled = false;
            
        }

        private void KiukaanLammitin_Tick(object sender, EventArgs e)
        {
            
            double nykyinenSaunanLpt = sauna.getSaunanLpt();
            double tick = 0.5;
            if (sauna.Switched == true)
            {
                nykyinenSaunanLpt += tick;          
            }
            
            lblSaunaLpt.Text = "";
            lblSaunaLpt.AppendText(nykyinenSaunanLpt.ToString());

            lokiIkkuna.VieLokiin("Kiuas päällä, sauna on " + nykyinenSaunanLpt.ToString() + " astetta lämmin.");

            sauna.AsetaSaunanLampotila(nykyinenSaunanLpt);
                        
            if (nykyinenSaunanLpt == slKiuas.Value)
            {
                kiukaanLammitin.Stop();
                kiukaanLammitin.Tick -= KiukaanLammitin_Tick; // tämä pitää olla, muuten tickejä tulee tuplamäärä, kun käynnistää lämmityksen uudestaan
            }
            

        }
        private void btnKiuasOff_Click(object sender, RoutedEventArgs e)
        {
            lokiIkkuna.VieLokiin("Kiuas pois päältä.");
            btnKiuasOff.IsEnabled = false;
            btnKiuasOn.IsEnabled = true;
            slKiuas.IsEnabled = true;
            sauna.Switched = false;
            SaunaAjastin2();
            btnSaunaIndicator.Background = Brushes.LightGray;
            kiukaanLammitin.IsEnabled = false;
        }
        private void KiukaanJaahdytin_Tick(object sender, EventArgs e)
        {
            double nykyinenSaunanLpt = sauna.getSaunanLpt();
            
            if (sauna.Switched == false)
            {
                if (nykyinenSaunanLpt - Thermostat.Temperature < 1 )
                {
                    nykyinenSaunanLpt = Thermostat.Temperature;
                }
                else
                {
                    nykyinenSaunanLpt--;
                }
            }

            lblSaunaLpt.Text = "";
            lblSaunaLpt.AppendText(nykyinenSaunanLpt.ToString());
            
            lokiIkkuna.VieLokiin("Kiuas pois päältä, sauna on " + nykyinenSaunanLpt.ToString() + " astetta lämmin.");

            sauna.AsetaSaunanLampotila(nykyinenSaunanLpt);
            
            if (nykyinenSaunanLpt == Thermostat.Temperature)
            {
                kiukaanJaahdytin.Stop();
                kiukaanJaahdytin.Tick -= KiukaanJaahdytin_Tick;
            }

        }

        private void btnLoki_Click(object sender, RoutedEventArgs e)
        {
            lokiIkkuna.Show();
        }

       
    }
}
