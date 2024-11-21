using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using MySql.Data.MySqlClient;
using System.Windows;
using System.IO.Ports;

namespace GUI



{
    //public static class GlobalVariablesTakeWindow
    //{
    //    // Deklariere eine statische Variable
    //    public static int takeWindowSchablone;
    //}

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPortManager SerialPortManager { get; private set; }

      

        public MainWindow()
        {
            InitializeComponent();
            SerialPortManager = new SerialPortManager("COM4", 9600, Parity.None, 8, StopBits.One, Handshake.None);
            SerialPortManager.DataReceived += SerialPortManager_DataReceived;
        }

        private void SerialPortManager_DataReceived(object sender, string data)
        {
            Dispatcher.Invoke(() => MessageBox.Show($"Received: {data}"));
        }

       


        public class GlobalSettings
        {
            private static GlobalSettings instance = null;
            private static readonly object padlock = new object();

            public string Server { get; private set; }
            public string Database { get; private set; }
            public string Username { get; private set; }
            public string Password { get; private set; }

            private MySqlConnection connection;

            private GlobalSettings()
            {
                //SQL Server
                Server = "localhost";
                Database = "importliste";
                Username = "root";
                Password = "root";


            }
            
            
            
            //Connect to SQL Database anfang
            public static GlobalSettings Instance
            {
                get
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new GlobalSettings();
                        }
                        return instance;
                    }
                }
            }

            public MySqlConnection Connection
            {
                get
                {
                    if (connection == null)
                    {
                        string constring = $"SERVER={Server};DATABASE={Database};UID={Username};PASSWORD={Password};";
                        connection = new MySqlConnection(constring);
                        connection.Open();
                    }
                    else if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    return connection;
                }
            }

            public string GetConnectionString()
            {
                return $"server={Server};userid={Username};password={Password};database={Database}";
            }

            public void TestConnection()
            {
                try
                {
                    using (var conn = this.Connection)
                    {
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            MessageBox.Show("Verbindung erfolgreich hergestellt.", "Verbindungsstatus", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Verbindung fehlgeschlagen.", "Verbindungsstatus", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler bei der Verbindung: {ex.Message}", "Verbindungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }


        }
        //SQL Connection ende




        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            //Schablonen such Blatt öffnen
            //search search = new search();
            //search.Show();
              

            search searchWindow = new search(this);
            searchWindow.Show();

            //helpWindow helpWindow = new helpWindow();
            //helpWindow.Show();
        }


        private void btn_Help_Click(object sender, RoutedEventArgs e)
        {
            //help Window öffnen
            helpWindow helpWindow = new helpWindow();
            helpWindow.Show();
            
        }

        private void btn_einlagerung_Click(object sender, RoutedEventArgs e)
        {
            Einlagern0Window einlagern0Window = new Einlagern0Window();
            einlagern0Window.Show();
        }

        private void btn_auslagerung_Click(object sender, RoutedEventArgs e)
        {

            //Schablone nehmen Window
            Auslagern0Window auslagern0Window = new Auslagern0Window();
            auslagern0Window.Show();
        }

        private void btn_beenden_Click(object sender, RoutedEventArgs e)
        {
            //Programm beenden
            this.Close();
        }

        private void btn_reinigen_Click(object sender, RoutedEventArgs e)
        {
            cleanWindow cleanWindow = new cleanWindow();
            cleanWindow.Show();
        }

        private void btn_kopiedrucken_Click(object sender, RoutedEventArgs e)
        {
            CopyandPrint cop = new CopyandPrint();
            cop.Show();
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            
        }



        private void Schablonenliste_Click(object sender, RoutedEventArgs e)
        {
            SchablonenlisteAnzeigenWindow schablonenliste = new SchablonenlisteAnzeigenWindow();
            schablonenliste.Show();
        }

        private void Warnungen_Click(object sender, RoutedEventArgs e)
        {
            WarnungenAnzeigenWindow warnungen = new WarnungenAnzeigenWindow();
            warnungen.Show();
        }

        private void Schablonenvorgänge_Click(object sender, RoutedEventArgs e)
        {
            SchablonenvorgängeAnzeigenWindow vorgänge = new SchablonenvorgängeAnzeigenWindow();
            vorgänge.Show();
        }

        private void Lagervorgänge_Click(object sender, RoutedEventArgs e)
        {
            lagervorgängeAnzeigenWindow lvZeigen = new lagervorgängeAnzeigenWindow();
            lvZeigen.Show();
        }

        private void Schablonenprüfungen_Click(object sender, RoutedEventArgs e)
        {
            SchablonenprüfungenAnzeigenWindow prüfungen = new SchablonenprüfungenAnzeigenWindow();
            prüfungen.Show();
        }

        private void btn_archivieren_Click(object sender, RoutedEventArgs e)
        {
            ArchivierenWindow arch = new ArchivierenWindow();
            arch.Show();
        }

        //private void vorgänge_Anzeigen_Click(object sender, RoutedEventArgs e)
        //{
        //    lagervorgängeAnzeigenWindow lvzeigen = new lagervorgängeAnzeigenWindow();
        //    lvzeigen.Show();
        //}






        //tesing dat shid


    }
}
