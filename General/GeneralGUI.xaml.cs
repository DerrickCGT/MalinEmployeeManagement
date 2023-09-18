using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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


namespace General
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GeneralGUI : Window
    {
        Dictionary<string, string> staffData = new Dictionary<string, string>();

        public GeneralGUI()
        {
            InitializeComponent();
        }



        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an OpenFileDialog to allow the user to select a CSV file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                // Read the selected CSV file
                string filePath = openFileDialog.FileName;   

                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        // Read and process each line in the CSV file
                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            string[] columns = line.Split(','); // Assuming CSV is comma-separated

                            if (columns.Length >= 2)
                            {
                                string key = columns[0].Trim();
                                string value = columns[1].Trim();

                                // Add key-value pair to the dictionary
                                staffData[key] = value;
                            }
                        }
                    }

                    // Set the ListBox's ItemsSource to the loaded dictionary
                    listBoxData.ItemsSource = myDictionary;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading CSV file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
