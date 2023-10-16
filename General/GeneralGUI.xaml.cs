using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        // 4.1.	Create a Dictionary data structure with a TKey of type integer and a TValue of type string, name the new data structure “MasterFile”.
        public static Dictionary<int, string> MasterFile = new Dictionary<int, string>();
        string csvFilePath = Environment.CurrentDirectory + "\\MalinStaffNamesV2.csv";


        public GeneralGUI()
        {
            InitializeComponent();
            //this.KeyDown += GeneralGUI_KeyDown;
            //RoutedCommand loadCmd = new RoutedCommand();
            //loadCmd.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Alt));
            //CommandBindings.Add(new CommandBinding(loadCmd, LoadStaffData));
            DisplayData();
            ShortCut_Command(Key.L, ModifierKeys.Control, DisplayData);
            ShortCut_Command(Key.Q, ModifierKeys.Control, TerminateProgram);
            ShortCut_Command(Key.I, ModifierKeys.Control, ClearTextBox_StaffId);
            ShortCut_Command(Key.N, ModifierKeys.Control, ClearTextBox_StaffName);
            ShortCut_Command(Key.F, ModifierKeys.Control, SelectStaffData);
            ShortCut_Command(Key.A, ModifierKeys.Alt, OpenAdminControl);

        }

        //private void GeneralGUI_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.L && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
        //    {
        //        LoadStaffData();
        //    }
        //}

        // Method to create shortcut key and allow user to input parameter and function accordingly
        private void ShortCut_Command(Key input, ModifierKeys modifierKeys, Action function)
        {
            RoutedCommand command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(input, modifierKeys));
            CommandBindings.Add(new CommandBinding(command, (sender, e) => function()));
        }

        // 4.2.	Create a method that will read the data from the .csv file into the Dictionary data structure when the GUI loads. 
        // Utilise a keyboard shortcut of Ctrl+T.
        private void LoadCsvFile()
        {
            // Read the selected CSV file
            MasterFile.Clear();
            try
            {
                using (StreamReader reader = new StreamReader(csvFilePath))
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
                        StatusBarFeedback("Sucessful", $"CSV File loaded: {csvFilePath}");
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading CSV file: " + csvFilePath + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusBarFeedback("Error", $"Failed to read CSV file: {csvFilePath}");
            }
        }

        // 4.3.	Create a method to display the Dictionary data into a non-selectable display only list box (ie read only).
        private void DisplayData()
        {
            listBoxData.ItemsSource = null;
            LoadCsvFile();
            // Set the ListBox's ItemsSource to the loaded dictionary
            listBoxData.ItemsSource = MasterFile;            
        }

        // Method to terminate the program.
        // Utilise a keyboard shortcut of Ctrl+T.
        private void TerminateProgram()
        {
            Close();
        }

        // 4.5.	Create a method to filter the Staff ID data from the Dictionary into the second filtered and selectable list box.
        // This method must use a text box input and update as each number is entered.The list box must reflect the filtered data in real time.
        private void FilterStaffId()
        {
            listBoxFilter.ItemsSource = null;

            string input = TextBoxStaff_Id.Text;

            var filterList = MasterFile.Where(kv => kv.Key.ToString().Contains(input)).ToList();

            listBoxFilter.ItemsSource = filterList;

            //foreach (var item in staffData)
            //{
            //    if (item.Key.ToString().Contains(filterText))
            //    {
            //        listBoxFilter.Items.Add(item);
            //    }
            //}
        }

        // Filter method triggers when input StaffId textbox textChanged
        private void FilterTextBoxStaffId_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterStaffId();
        }

        // Keypress filter input user in textbox StaffId
        private void KeyPressTextBoxStaff_Id(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true; // Block non-numeric input                
            }

            if (e.Text != "7" && TextBoxStaff_Id.Text.Length == 0)
            {
                e.Handled = true; // Only allow "7" on the first index as requirement for UK phone number
            }
        }

        // 4.4.	Create a method to filter the Staff Name data from the Dictionary into a second filtered and selectable list box.
        // This method must use a text box input and update as each character is entered. The list box must reflect the filtered data in real time.
        private void FilterStaffName()
        {
            listBoxFilter.ItemsSource = null;

            string input = TextBoxStaff_Name.Text.ToLower();

            var filterList = MasterFile.Where(kv => kv.Value.ToLower().Contains(input)).ToList();

            listBoxFilter.ItemsSource = filterList;
        }

        // Filter method triggers when input StaffName textbox textChanged
        private void FilterTextBoxStaffName_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterStaffName();
        }

        // Keypress filter input user in textbox StaffName
        private void KeyPressTextBoxStaff_Name(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[a-zA-Z]");

            if (!regex.IsMatch(e.Text))
            {
                e.Handled = true; // Block non-numeric input                
            }

        }

        // 4.6.	Create a method for the Staff Name text box which will clear the contents and place the focus into the Staff Name text box.
        // Utilise a keyboard shortcut of Ctrl+N.
        private void ClearTextBox_StaffName()
        {
            TextBoxStaff_Name.Clear();
            TextBoxStaff_Name.Focus();
            StatusBarFeedback("Ready", "TextBox Staff Name is cleared.");

        }

        // 4.7.	Create a method for the Staff ID text box which will clear the contents and place the focus into the text box.
        // Utilise a keyboard shortcut of Ctrl+I.
        private void ClearTextBox_StaffId()
        {
            TextBoxStaff_Id.Clear();
            TextBoxStaff_Id.Focus();
            StatusBarFeedback("Ready", "TextBox Staff ID is cleared.");
        }

        // 4.8.	Create a method for the filtered and selectable list box which will populate the two text boxes when a staff record is selected.
        // Utilise the Tab and keyboard keys with keyboard shortcut of Alt+F.
        private void SelectStaffData()
        {
            if (listBoxFilter.SelectedIndex >= 0 && listBoxFilter.SelectedItem != null)
            {
                // Ref:https://stackoverflow.com/questions/6404520/how-to-get-listbox-selected-item-as-keyvaluepairstring-string-in-c
                var selectedItem = (KeyValuePair<int, string>)listBoxFilter.SelectedItem;
               
                TextBoxStaff_Id.Text = selectedItem.Key.ToString();
                TextBoxStaff_Name.Text = selectedItem.Value;
                StatusBarFeedback("Successful", $"Staff ID: {selectedItem.Key} is selected.");
            }
        }

        private void OpenAdminControl()
        {
            AdminGUI adminControl = new AdminGUI(MasterFile, TextBoxStaff_Id.Text);
            adminControl.ShowDialog();
            StatusBarClear();
        }

        private void StatusBarFeedback(string status_message, string feedback_message)
        {
            TextBlockStatus.Text = status_message;
            TextBlockFeedback.Text = feedback_message;
        }

        private void StatusBarClear()
        {
            TextBlockStatus.Text = "Ready";
            TextBlockFeedback.Text = "";
        }

    }
}
