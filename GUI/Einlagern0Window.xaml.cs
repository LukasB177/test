using MySql.Data.MySqlClient;
using System;
using System.IO.Ports;
using System.Windows;

namespace GUI
{
    public partial class Einlagern0Window : Window
    {
        public Einlagern0Window()
        {
            InitializeComponent();
        }

        private void ButtonBeenden_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonSchabloneEinlagern_Click(object sender, RoutedEventArgs e)
        {
            // Variablen definieren
            string ckNr = SchablonennummerTextBox.Text;
            string typSchab = string.Empty;
            string seite = string.Empty;
            string benutzer = Environment.UserName;
            string nameBearbeiter = string.Empty;
            DateTime datum = DateTime.Now; // Datum ohne Formatierung

            if (string.IsNullOrWhiteSpace(ckNr))
            {
                MessageBox.Show("Bitte Schablonennummer eingeben!");
                SchablonennummerTextBox.Focus();
                return;
            }

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
                MessageBox.Show("Verarbeitungsfehler: Bitte Schablonentyp selektieren!");
                return;
            }

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
                MessageBox.Show("Verarbeitungsfehler: Keine Bestückungsseite ausgewählt!");
                return;
            }

            // Verbindungsaufbau zur Datenbank
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    // Abfrage der Schablone basierend auf der CK-Nummer
                    string query = "SELECT * FROM sliste WHERE `CK` = @ckNr";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ckNr", ckNr);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show($"Die Schablone {typSchab}{ckNr} wurde auf keinem Lagerort gefunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }

                            // Lesen der Schablonendaten

                            reader.Read();
                            string lager = reader["Lagerplatz"].ToString();
                            string schablonennummer = reader["Schablonennummer"].ToString();
                            string topBottom = reader["TOPBOTTOM"].ToString();
                            string typ = reader["Typ"].ToString();
                            int nutzengröße = Convert.ToInt32(reader["Nutzengröße"]);
                            int aktZyklen = Convert.ToInt32(reader["aktZyklen"]);
                            int maxZyklen = Convert.ToInt32(reader["maxZyklen"]);
                            int warnZyklen = Convert.ToInt32(reader["WarnungZyklen"]);
                            string ausgelagert = reader["ausgelagert"].ToString();

                            // Überprüfen, ob die Seite und der Typ übereinstimmen
                            if (!topBottom.Equals(seite, StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show($"Die Schablone mit der Nummer {ckNr} ist für die Seite {topBottom} vorgesehen, nicht {seite}.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }

                            if (!typ.Equals(typSchab, StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show($"Der Typ der Schablone {ckNr} ist {typ}, nicht {typSchab}.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }



                            // Überprüfen, ob die Schablone ausgelagert ist
                            if (!string.IsNullOrEmpty(ausgelagert))
                            {
                                if (ausgelagert.Equals("Ja", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Schablone ist bereits ausgelagert, weiter mit der Einlagerung
                                }
                                else if (ausgelagert.Equals("Nein", StringComparison.OrdinalIgnoreCase))
                                {
                                    MessageBox.Show($"Die Schablone mit der Nummer {ckNr} ist bereits eingelagert.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }
                                else if (ausgelagert.Equals("Gitterbox", StringComparison.OrdinalIgnoreCase))
                                {
                                    var result = MessageBox.Show($"Die Schablone mit der Nummer {ckNr} ist archiviert (Gitterbox). Soll diese wirklich eingelagert werden?", "Archivierte Schablone", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (result == MessageBoxResult.No)
                                    {
                                        return;
                                    }
                                    // Wenn Ja ausgewählt wurde, geht das Programm weiter und führt die Einlagerung durch
                                }
                            }

                            


                            // Schließen des Readers, bevor eine andere Datenbankoperation ausgeführt wird
                            reader.Close();

                            // Auftragsmenge abfragen und prüfen
                            int auftragsmenge;
                            while (true)
                            {
                                string auftrag = Microsoft.VisualBasic.Interaction.InputBox("Bitte erfassen Sie jetzt die Auftragsmenge:", "Auftragsmenge erfassen");

                                // Wenn "Abbrechen" gedrückt wird, gibt die InputBox einen leeren String zurück
                                if (string.IsNullOrEmpty(auftrag))
                                {
                                    // Vorgang abbrechen und Funktion beenden
                                    MessageBox.Show("Vorgang abgebrochen.", "Abbruch", MessageBoxButton.OK, MessageBoxImage.Information);
                                    return;
                                }

                                if (int.TryParse(auftrag, out auftragsmenge) && auftragsmenge > 0)
                                {
                                    if (auftragsmenge % nutzengröße != 0)
                                    {
                                        MessageBox.Show("Ungültige Eingabe! Auftragsmenge ist kein Vielfaches der Nutzengröße!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                        continue;
                                    }

                                    var ans = MessageBox.Show($"Stimmt die Auftragszahl {auftragsmenge} Stück mit Ihren Unterlagen überein?", "Sicherheitsabfrage", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                    if (ans == MessageBoxResult.Yes)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Bitte eine gültige Auftragsmenge eingeben!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                            }

                            // Prüfung gemäß AI-653 017
                            var geprueft = MessageBox.Show("Schablone entsprechend AI-653 017 geprüft?", "Prüfung", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (geprueft == MessageBoxResult.No)
                            {
                                MessageBox.Show("Ungeprüfte Schablonen dürfen nicht eingelagert werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }

                            // Namenskürzel des Bearbeiters abfragen
                            while (true)
                            {
                                nameBearbeiter = Microsoft.VisualBasic.Interaction.InputBox("Namenskürzel des Bearbeiters:", "Bearbeiter erfassen");
                                if (!string.IsNullOrWhiteSpace(nameBearbeiter))
                                {
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show("Name darf nicht leer sein! Vorgang abgebrochen!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                            }

                            // **Bestätigungsabfrage hinzufügen**
                            MessageBoxResult confirmation = MessageBox.Show(
                                $"Die Schablone {schablonennummer} befindet sich aktuell am Lagerplatz {lager}.\n" +
                                "Möchten Sie die Schablone wirklich einlagern?",
                                "Einlagerung bestätigen",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (confirmation == MessageBoxResult.No)
                            {
                                // Vorgang abbrechen, wenn der Benutzer "Nein" wählt
                                return;
                            }


                            // Berechnung der neuen Rakelzahl
                            int rakelzahl = (int)Math.Ceiling((double)auftragsmenge / nutzengröße);
                            int rakelNeu = aktZyklen + rakelzahl;

                            // **Lagerplatznummer berechnen und an Arduino senden**
                            int lagerplatzNummer = GetLagerplatzNummer(lager);
                            SendeAnArduino(lagerplatzNummer);

                            // Datenbankupdate durchführen
                            string neueAusgelagert = "Nein"; // Setzt den Status auf "Nein" für eingelagert
                            string updateQuery = "UPDATE sliste SET `ausgelagert` = @neueAusgelagert, `aktZyklen` = @rakelNeu WHERE `CK` = @ckNr";
                            using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@neueAusgelagert", neueAusgelagert);
                                updateCmd.Parameters.AddWithValue("@rakelNeu", rakelNeu);
                                updateCmd.Parameters.AddWithValue("@ckNr", ckNr);
                                updateCmd.ExecuteNonQuery();
                            }

                            // Überprüfung der Zyklen und Erstellung von Warnungen
                            if (rakelNeu > (maxZyklen - warnZyklen) && rakelNeu <= maxZyklen)
                            {
                                EintragInWarnungen(conn, datum, lager, typSchab, schablonennummer, topBottom, ckNr, benutzer, "WARNUNG: Standzeit bald erreicht.");
                                MessageBox.Show("Standzeit bald erreicht", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                                //E-Mail an Harald schicken wenn Warnung ausgegebn wird!
                            }
                            else if (rakelNeu > maxZyklen)
                            {
                                EintragInWarnungen(conn, datum, lager, typSchab, schablonennummer, topBottom, ckNr, benutzer, "KRITISCH: Standzeit erreicht!");
                                MessageBox.Show("KRITISCH: Standzeit erreicht!", "Kritisch", MessageBoxButton.OK, MessageBoxImage.Error);
                                
                            }

                            // **Eintrag in die lagerung-Tabelle erstellen**
                            string insertLagerungQuery = "INSERT INTO lagerung (datum, Lagerplatz, Typ, Schablonennummer, TOPBOT, `CK`, Benutzer, Meldungstext) VALUES " +
                                                         "(@datum, @lagerplatz, @typ, @schablonennummer, @TOPBOT, @ckNummer, @benutzer, @vorgang)";
                            using (MySqlCommand insertCmd = new MySqlCommand(insertLagerungQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@datum", datum);
                                insertCmd.Parameters.AddWithValue("@lagerplatz", lager);
                                insertCmd.Parameters.AddWithValue("@typ", typSchab);
                                insertCmd.Parameters.AddWithValue("@schablonennummer", schablonennummer);
                                insertCmd.Parameters.AddWithValue("@TOPBOT", seite);
                                insertCmd.Parameters.AddWithValue("@ckNummer", ckNr);
                                insertCmd.Parameters.AddWithValue("@benutzer", benutzer);
                                insertCmd.Parameters.AddWithValue("@vorgang", "VORGANG: Schablone eingelagert.");

                                insertCmd.ExecuteNonQuery();
                            }
                            // **Bestätigung anzeigen**
                            MessageBox.Show($"Die Schablone {typSchab} {ckNr} wurde erfolgreich eingelagert. Sie muss an Lagerplatz {lager} gebracht werden.\n" +
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

                    // Logbuch-Eintrag erstellen
                    LogbuchEintragErstellen(conn, datum, ckNr, nameBearbeiter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Verbindung zur Datenbank: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogbuchEintragErstellen(MySqlConnection conn, DateTime datum, string ckNr, string bearbeiter)
        {
            try
            {
                string insertQuery = "INSERT INTO prüfungen (`Datum`, `CK-Nummer`, `Bearbeiter`) VALUES (@datum, @ckNr, @bearbeiter)";
                using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@datum", datum);
                    insertCmd.Parameters.AddWithValue("@ckNr", ckNr);
                    insertCmd.Parameters.AddWithValue("@bearbeiter", bearbeiter);
                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }

      
        private void EintragInWarnungen(MySqlConnection conn, DateTime datum, string lagerplatz, string typ, string schablonennummer, string topBottom, string ckNummer, string benutzer, string meldung)
        {
            string insertWarnungQuery = "INSERT INTO warnungen (`datum`, `Lagerplatz`, `Typ`, `Schablonennummer`, `TOPBOT`, `CK#`, `Benutzer`, `Meldungstext`) VALUES " +
                                        "(@datum, @lagerplatz, @typ, @schablonennummer, @topBottom, @ckNummer, @benutzer, @meldung)";
            using (MySqlCommand insertWarnungCmd = new MySqlCommand(insertWarnungQuery, conn))
            {
                insertWarnungCmd.Parameters.AddWithValue("@datum", datum);
                insertWarnungCmd.Parameters.AddWithValue("@lagerplatz", lagerplatz);
                insertWarnungCmd.Parameters.AddWithValue("@typ", typ);
                insertWarnungCmd.Parameters.AddWithValue("@schablonennummer", schablonennummer);
                insertWarnungCmd.Parameters.AddWithValue("@topBottom", topBottom);
                insertWarnungCmd.Parameters.AddWithValue("@ckNummer", ckNummer);
                insertWarnungCmd.Parameters.AddWithValue("@benutzer", benutzer);
                insertWarnungCmd.Parameters.AddWithValue("@meldung", meldung);
                insertWarnungCmd.ExecuteNonQuery();
            }
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

    }
}
