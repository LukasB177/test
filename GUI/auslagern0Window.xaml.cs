using MySql.Data.MySqlClient;
using System;
using System.IO.Ports;
using System.Windows;

namespace GUI
{
    public partial class Auslagern0Window : Window
    {
        public Auslagern0Window()
        {
            InitializeComponent();
        }

        private void ButtonBeenden_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Leerer Handler, um den Fehler zu vermeiden
        }

        private void ButtonSchabloneAuslagern_Click(object sender, RoutedEventArgs e)
        {
            string schablonenNummer = SchablonennummerTextBox.Text;
            string typSchab;
            string seite;
            string suchNum = schablonenNummer; // CK#
            string benutzer = Environment.UserName;
            DateTime datum = DateTime.Now;
            string lager = "";

            string schablonennummer = "";
            string cKNummer = "";
            string neueAusgelagert = "Ja";

            // Überprüfen, ob die Schablonennummer eingegeben wurde
            if (string.IsNullOrWhiteSpace(schablonenNummer))
            {
                MessageBox.Show("Bitte Schablonennummer eingeben!");
                SchablonennummerTextBox.Focus();
                return;
            }

            // Überprüfen, ob der Schablonentyp ausgewählt wurde
            if (TypRadioButton601.IsChecked == true)
            {
                typSchab = "601-";
            }
            else if (TypRadioButtonVS.IsChecked == true)
            {
                typSchab = "VS-";
            }
            else
            {
                MessageBox.Show("Verarbeitungsfehler: bitte Schablonentyp selektieren!");
                return;
            }

            // Neu: Überprüfen, ob die Seite (TOP/BOTTOM) ausgewählt wurde
            if (SeiteRadioButtonTOP.IsChecked == true)
            {
                seite = "TOP";
            }
            else if (SeiteRadioButtonBOTTOM.IsChecked == true)
            {
                seite = "BOTTOM";
            }
            else
            {
                MessageBox.Show("Verarbeitungsfehler: bitte eine Seite (TOP oder BOTTOM) selektieren!");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    // Zuerst überprüfen, ob die Schablonennummer existiert
                    string queryExist = "SELECT COUNT(*) FROM sliste WHERE `Schablonennummer` = @suchNum";
                    using (MySqlCommand cmdExist = new MySqlCommand(queryExist, conn))
                    {
                        cmdExist.Parameters.AddWithValue("@suchNum", suchNum);

                        int existCount = Convert.ToInt32(cmdExist.ExecuteScalar());

                        // Wenn die Schablonennummer nicht existiert
                        if (existCount == 0)
                        {
                            MessageBox.Show($"Schablonennummer {suchNum} wurde nicht gefunden.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Jetzt prüfen, ob alle Schablonen bereits ausgelagert sind
                    string queryCheckAllAusgelagert = "SELECT COUNT(*) FROM sliste WHERE `Schablonennummer` = @suchNum AND `ausgelagert` != 'Ja'";
                    using (MySqlCommand cmdCheckAllAusgelagert = new MySqlCommand(queryCheckAllAusgelagert, conn))
                    {
                        cmdCheckAllAusgelagert.Parameters.AddWithValue("@suchNum", suchNum);
                        int countNotAusgelagert = Convert.ToInt32(cmdCheckAllAusgelagert.ExecuteScalar());

                        if (countNotAusgelagert == 0)
                        {
                            MessageBox.Show($"Alle Schablonen der Nummer {suchNum} sind bereits ausgelagert.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    // Abfrage zur Auswahl der Schablone mit den niedrigsten 'aktZyklen', die nicht ausgelagert ist
                    string query = "SELECT * FROM sliste WHERE `Schablonennummer` = @suchNum AND `Typ` = @typ AND `TOPBOTTOM` = @seite AND `ausgelagert` != 'Ja' ORDER BY CAST(`aktZyklen` AS UNSIGNED) ASC LIMIT 1";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@suchNum", suchNum);
                        cmd.Parameters.AddWithValue("@typ", typSchab);
                        cmd.Parameters.AddWithValue("@seite", seite);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show($"Die Schablone {suchNum} des Typs {typSchab} und der Seite {seite} wurde nicht gefunden oder ist bereits ausgelagert.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }

                            while (reader.Read())
                            {
                                schablonennummer = reader["Schablonennummer"].ToString();
                                cKNummer = reader["CK"].ToString(); //CK-Nummer
                                lager = reader["Lagerplatz"].ToString();
                                string aktZyklenText = reader["aktZyklen"].ToString();
                                string ausgelagert = reader["ausgelagert"].ToString();

                                // Versuch, 'aktZyklen' in eine Ganzzahl zu konvertieren
                                if (!int.TryParse(aktZyklenText, out int aktZyklen))
                                {
                                    MessageBox.Show($"Fehler: 'aktZyklen' konnte nicht konvertiert werden: {aktZyklenText}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                // Überprüfen, ob die Schablone bereits ausgelagert wurde
                                if (ausgelagert.Equals("Ja", StringComparison.OrdinalIgnoreCase))
                                {
                                    MessageBox.Show($"Die Schablone {schablonennummer} wurde bereits ausgelagert.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    return;
                                }

                                // Benutzerabfrage, ob die Schablone wirklich ausgelagert werden soll
                                MessageBoxResult result = MessageBox.Show(
                                    $"Die Schablone {schablonennummer} befindet sich am Lagerplatz {lager}.\n" +
                                    "Möchten Sie die Schablone wirklich auslagern?",
                                    "Auslagerung bestätigen",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                                if (result == MessageBoxResult.No)
                                {
                                    // Vorgang abbrechen, wenn der Benutzer "Nein" wählt
                                    return;
                                }

                            }
                                // Schließen des Readers nach dem Lesen der Daten
                                reader.Close();

                                // Aktualisieren der ausgelagert Spalte
                                

                                // Datenbankupdate durchführen
                                string updateQuery = $"UPDATE sliste SET `ausgelagert` = @neueAusgelagert WHERE `Schablonennummer` = @suchNum AND `Lagerplatz` = @lagerplatz";
                                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@neueAusgelagert", neueAusgelagert);
                                    updateCmd.Parameters.AddWithValue("@suchNum", suchNum);
                                    updateCmd.Parameters.AddWithValue("@lagerplatz", lager);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Logik zum Eintragen des Vorgangs in die 'lagerung'-Tabelle
                                string insertQuery = "INSERT INTO lagerung (datum, Lagerplatz, Typ, Schablonennummer, TOPBOT, `CK`, Benutzer, Meldungstext) VALUES " +
                                                     "(@datum, @lagerplatz, @typ, @schablonennummer, @TOPBOT,  @ckNummer, @benutzer, @vorgang)";
                                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                                {
                                    string datumFormatted = datum.ToString("yyyy-MM-dd HH:mm:ss");
                                    insertCmd.Parameters.AddWithValue("@datum", datumFormatted);
                                    insertCmd.Parameters.AddWithValue("@lagerplatz", lager);
                                    insertCmd.Parameters.AddWithValue("@typ", typSchab);
                                    insertCmd.Parameters.AddWithValue("@schablonennummer", schablonennummer);
                                    insertCmd.Parameters.AddWithValue("@TOPBOT", seite);
                                    insertCmd.Parameters.AddWithValue("@ckNummer", cKNummer); // CK# ist `Schablonennummer`
                                    insertCmd.Parameters.AddWithValue("@benutzer", benutzer);
                                    insertCmd.Parameters.AddWithValue("@vorgang", "VORGANG: Schablone ausgelagert.");

                                    insertCmd.ExecuteNonQuery();
                                }

                                // Lagerplatz an Arduino senden
                                int lagerplatzNummer = GetLagerplatzNummer(lager);  // Hier wird der Lagerplatz umgerechnet
                                SendeAnArduino(lagerplatzNummer);

                                MessageBox.Show($"Die Schablone {typSchab}{suchNum} wurde erfolgreich ausgelagert. Sie muss von Lagerplatz {lager} entnommen werden.\n" +
                                                $"An Arduino gesendete Zahl: {lagerplatzNummer}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                SchablonennummerTextBox.Clear();

                                try
                                {
                                    // Zerlege die Zahl in zwei Bytes
                                    byte lowByte = (byte)(2000 & 0xFF); // Niedrigstes Byte (0-255)
                                    byte highByte = (byte)((2000 >> 8) & 0xFF); // Höchstes Byte

                                    using (SerialPort port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One))
                                    {
                                        port.Open();
                                        port.Write(new byte[] { lowByte }, 0, 1);
                                        System.Threading.Thread.Sleep(100);
                                        port.Write(new byte[] { highByte }, 0, 1);
                                        port.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Fehler beim Senden der Zahl an den Arduino: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                
                            
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Fehler bei der Verarbeitung von Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Verbindung zur Datenbank: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetLagerplatzNummer(string lagerplatz)
        {
            string buchstabe = lagerplatz.Substring(0, 1);
            int zahl = int.Parse(lagerplatz.Substring(1));

            int lagerplatzBasis;
            switch (buchstabe)
            {
                case "A":
                    lagerplatzBasis = 0;
                    break;
                case "B":
                    lagerplatzBasis = 120;
                    break;
                case "C":
                    lagerplatzBasis = 240;
                    break;
                case "D":
                    lagerplatzBasis = 360;
                    break;
                case "E":
                    lagerplatzBasis = 480;
                    break;
                case "F":
                    lagerplatzBasis = 600;
                    break;
                case "G":
                    lagerplatzBasis = 720;
                    break;
                case "H":
                    lagerplatzBasis = 840;
                    break;
                case "I":
                    lagerplatzBasis = 960;
                    break;
                case "J":
                    lagerplatzBasis = 1080;
                    break;
                case "K":
                    lagerplatzBasis = 1200;
                    break;
                case "L":
                    lagerplatzBasis = 1320;
                    break;
                case "M":
                    lagerplatzBasis = 1413;
                    break;
                case "N":
                    lagerplatzBasis = 1506;
                    break;
                default:
                    throw new ArgumentException($"Ungültiger Lagerplatzbuchstabe: {buchstabe}");
            }

            return lagerplatzBasis + zahl;

        }

        private void SendeAnArduino(int lagerplatz)
        {
            try
            {
                // Zerlege die Zahl in zwei Bytes
                byte lowByte = (byte)(lagerplatz & 0xFF); // Niedrigstes Byte (0-255)
                byte highByte = (byte)((lagerplatz >> 8) & 0xFF); // Höchstes Byte

                using (SerialPort port = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One))
                {
                    port.Open();
                    port.Write(new byte[] { lowByte }, 0, 1);
                    System.Threading.Thread.Sleep(100);
                    port.Write(new byte[] { highByte }, 0, 1);
                    port.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden der Zahl an das Arduino: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
