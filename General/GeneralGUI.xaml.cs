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
        Dictionary<int, string> MasterFile = new Dictionary<int, string>();


        public GeneralGUI()
        {
            InitializeComponent();
            //this.KeyDown += GeneralGUI_KeyDown;
            //RoutedCommand loadCmd = new RoutedCommand();
            //loadCmd.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Alt));
            //CommandBindings.Add(new CommandBinding(loadCmd, LoadStaffData));
            ShortCut_Command(Key.L, LoadCsvFile);
            ShortCut_Command(Key.T, TerminateProgram);
        }

        //private void GeneralGUI_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        //    {
        //        LoadStaffData();
        //    }
        //}

        private void ShortCut_Command(Key input, Action function)
        {
            RoutedCommand command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(input, ModifierKeys.Alt));
            CommandBindings.Add(new CommandBinding(command, (sender, e) => function()));
        }

        private void LoadCsvFile()
        {
            // Read the selected CSV file
            string filePath = Environment.CurrentDirectory + "\\MalinStaffNamesV2.csv";

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
                            int key = int.Parse(columns[0].Trim());
                            string value = columns[1].Trim();

                            // Add key-value pair to the dictionary
                            MasterFile[key] = value;
                        }
                    }

                    DisplayData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading CSV file: " + filePath + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayData()
        {
            // Set the ListBox's ItemsSource to the loaded dictionary
            listBoxData.ItemsSource = MasterFile;
            listBoxFilter.ItemsSource = MasterFile;
        }

        private void TerminateProgram()
        {
            Close();
        }
    }
}
