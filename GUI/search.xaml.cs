using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using static GUI.MainWindow;
using System.Runtime.Serialization.Formatters;
using System.Windows.Interop;
using System.IO.Ports;

namespace GUI
{
    /// <summary>
    /// Interaktionslogik für search.xaml
    /// </summary>
    public partial class search : Window
    {
        private MainWindow mainWindow;

        public search(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            string ckNummer;
            string typSchab;
            //string Ausgabe;
            int Lagerplatz = 0;
            string LagerplatzAB = "";

            //Keine Eingabe in textBox
            if (string.IsNullOrEmpty(txt_Schablonennum.Text))
            {
                MessageBox.Show("Bitte geben Sie die CK-Nummer ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ckNummer = txt_Schablonennum.Text;

            //Schablonen Typ bestimmen
            if (SerienSchab.IsChecked == true)
            {

                typSchab = "601-";
            }
            else if (VSSchab.IsChecked == true)
            {
                typSchab = "VS-";
            }
            else
            {
                MessageBox.Show("Bitte Schablonentyp selektieren!");
                return;
            }


            // Schritt 3: Verbindung zur Datenbank herstellen und die CK-Nummer suchen
            var conn = GlobalSettings.Instance.Connection;

            string LagerplatzStr = $"SELECT Lagerplatz FROM sliste WHERE CK = @ckNummer AND Typ = @typSchab ORDER BY aktZyklen ASC LIMIT 1";

            //Label Lagerplatz befüllen
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(LagerplatzStr, conn))
                {
                    cmd.Parameters.AddWithValue("@ckNummer", ckNummer);
                    cmd.Parameters.AddWithValue("@typSchab", typSchab);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            LagerplatzAB = Convert.ToString(reader["LAGERPLATZ"]);

                            // Überprüfen, ob Lagerplatz NULL oder leer ist
                            if (string.IsNullOrEmpty(LagerplatzAB))
                            {
                                MessageBox.Show($"Die Schablone {ckNummer} liegt in der Gitterbox.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }

                            // Aktualisieren der Labels

                            //Ausgabe = Convert.ToString(lagerplatz);
                        }
                        else
                        {
                            MessageBox.Show($"Die Schablone mit der CK-Nummer {ckNummer} und dem Typ {typSchab} wurde nicht gefunden.", "Nicht gefunden", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Datenbankabfrage: {ex.Message}", "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            //Lagerplatz-Wert basierend auf dem Buchstaben bestimmen
            try
            {
                string buchstabe = LagerplatzAB.Substring(0, 1); // z.B. "A"
                int zahl = int.Parse(LagerplatzAB.Substring(1)); // z.B. "17"

                switch (buchstabe)
                {
                    case "N":
                        Lagerplatz = 1506;
                        break;
                    case "M":
                        Lagerplatz = 1413;
                        break;
                    case "L":
                        Lagerplatz = 1320;
                        break;
                    case "K":
                        Lagerplatz = 1200;
                        break;
                    case "J":
                        Lagerplatz = 1080;
                        break;
                    case "I":
                        Lagerplatz = 960;
                        break;
                    case "H":
                        Lagerplatz = 840;
                        break;
                    case "G":
                        Lagerplatz = 720;
                        break;
                    case "F":
                        Lagerplatz = 600;
                        break;
                    case "E":
                        Lagerplatz = 480;
                        break;
                    case "D":
                        Lagerplatz = 360;
                        break;
                    case "C":
                        Lagerplatz = 240;
                        break;
                    case "B":
                        Lagerplatz = 120;
                        break;
                    case "A":
                        Lagerplatz = 0;
                        break;
                    default:
                        MessageBox.Show($"Ungültiger Lagerplatz-Buchstabe: {buchstabe}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                // Füge den Zahlenteil hinzu
                Lagerplatz += zahl; // z.B. 0 (A) + 17 = 17 für "A17"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Lagerplatz-Berechnung: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Schritt 5: Lagerplatz an den Arduino senden
            try
            {
                // Zerlege die Zahl in zwei Bytes
                //byte lowByte = (byte)(Lagerplatz & 0xFF); // Niedrigstes Byte (0-255)
                //byte highByte = (byte)((Lagerplatz >> 8) & 0xFF); // Höchstes Byte (über 255)

                //using (SerialPort port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One))
                //{
                //    port.Open();
                //    port.Write(new byte[] { lowByte }, 0, 1);
                //    System.Threading.Thread.Sleep(100);
                //    port.Write(new byte[] { highByte }, 0, 1);
                //    port.Close();
                //}

                // Schritt 6: Erfolgsnachricht anzeigen
                MessageBox.Show($"Die Schablone mit der Nummer {ckNummer} befindet sich auf dem Lagerplatz {LagerplatzAB}.\n" +
                                $"An Arduino gesendete Zahl: {Lagerplatz}.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                //lowByte = (byte)(2000 & 0xFF); // Niedrigstes Byte (0-255)
                //highByte = (byte)((2000 >> 8) & 0xFF); // Höchstes Byte (über 255)

                //using (SerialPort port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One))
                //{
                //    port.Open();
                //    port.Write(new byte[] { lowByte }, 0, 1);
                //    System.Threading.Thread.Sleep(100);
                //    port.Write(new byte[] { highByte }, 0, 1);
                //    port.Close();
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden an das Arduino: {ex.Message}", "Arduino-Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



       

        private void btn_searchAbbruch_Click(object sender, RoutedEventArgs e)
        {
            //Abbrechen
            this.Close();

        }


    }
}