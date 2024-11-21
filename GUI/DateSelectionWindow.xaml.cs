using System;
using System.Windows;

namespace GUI
{
    public partial class DateSelectionWindow : Window
    {
        public DateTime? SelectedDate { get; private set; }

        public DateSelectionWindow()
        {
            InitializeComponent();

            // Setze das aktuelle Datum als Standarddatum
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = DatePicker.SelectedDate;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
