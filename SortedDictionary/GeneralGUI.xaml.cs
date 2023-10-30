using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SortedDictionary
{
    /// <summary>
    /// Interaction logic for GeneralGUI.xaml
    /// </summary>
    public partial class GeneralGUI : Window
    {
        // 4.1.	Create a SortedDictionary data structure with a TKey of type integer and a TValue of type string, name the new data structure “MasterFile”.
        public static SortedDictionary<int, string> MasterFile = new SortedDictionary<int, string>();
        string csvFilePath = Environment.CurrentDirectory + "\\MalinStaffNamesV2.csv";

        string logFile = "logFile.txt";
        TextWriterTraceListener traceListener;

        public GeneralGUI()
        {
            InitializeComponent();
            DisplayData();
            ShortCut_Command(Key.L, ModifierKeys.Control, DisplayData);
            ShortCut_Command(Key.Q, ModifierKeys.Control, TerminateProgram);
            ShortCut_Command(Key.I, ModifierKeys.Control, ClearTextBox_StaffId);
            ShortCut_Command(Key.N, ModifierKeys.Control, ClearTextBox_StaffName);
            //ShortCut_Command(Key.S, ModifierKeys.Control, SelectStaffData);
            ShortCut_Command2(Key.Enter, SelectStaffData);
            ShortCut_Command(Key.A, ModifierKeys.Alt, OpenAdminControl);

            traceListener = new TextWriterTraceListener(logFile);
            Trace.Listeners.Add(traceListener);
        }

        #region ShortCut Command
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
        private void ShortCut_Command2(Key input, Action function)
        {
            RoutedCommand command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(input));
            CommandBindings.Add(new CommandBinding(command, (sender, e) => function()));
        }
        #endregion

        #region Load and Display Data
        // 4.2.	Create a method that will read the data from the .csv file into the Dictionary data structure when the GUI loads. 
        // Utilise a keyboard shortcut of Ctrl+T.
        private void LoadCsvFile()
        {
            // Read the selected CSV file
            MasterFile.Clear();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            long beforeMemory = GC.GetTotalMemory(false);

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

                long afterMemory = GC.GetTotalMemory(false);
                long memoryUsageChange = afterMemory - beforeMemory;

                stopwatch.Stop();
                long elapsed = stopwatch.Elapsed.Ticks;
                TimerTextBlock.Text = "Timer: " + elapsed.ToString() + " ticks";

                Trace.TraceInformation($"Load CSV File- Memory Usage: {memoryUsageChange}, Performance Timer: {elapsed} ticks");
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
        #endregion                

        #region Filter by ID
        // 4.5.	Create a method to filter the Staff ID data from the Dictionary into the second filtered and selectable list box.
        // This method must use a text box input and update as each number is entered.The list box must reflect the filtered data in real time.
        private void FilterStaffId()
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            listBoxFilter.ItemsSource = null;

            string input = TextBoxStaff_Id.Text;

            var filterList = MasterFile.Where(kv => kv.Key.ToString().Contains(input)).ToList();

            listBoxFilter.ItemsSource = filterList;

            //stopwatch.Stop();
            //long elapsed = stopwatch.Elapsed.Ticks;
            //TimerTextBlock.Text = "Timer: " + elapsed.ToString() + " ticks";

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
            StatusBarClear();
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

            //if (e.Text != "7" && TextBoxStaff_Id.Text.Length == 0)
            //{
            //    e.Handled = true; // Only allow "7" on the first index as requirement for UK phone number
            //}
        }
        #endregion

        #region Filter by name
        // 4.4.	Create a method to filter the Staff Name data from the Dictionary into a second filtered and selectable list box.
        // This method must use a text box input and update as each character is entered. The list box must reflect the filtered data in real time.
        private void FilterStaffName()
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            listBoxFilter.ItemsSource = null;

            string input = TextBoxStaff_Name.Text.ToLower();

            var filterList = MasterFile.Where(kv => kv.Value.ToLower().Contains(input)).ToList();

            listBoxFilter.ItemsSource = filterList;

            //stopwatch.Stop();
            //long elapsed = stopwatch.Elapsed.Ticks;
            //TimerTextBlock.Text = "Timer: " + elapsed.ToString() + " ticks";
        }

        // Filter method triggers when input StaffName textbox textChanged
        private void FilterTextBoxStaffName_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatusBarClear();
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
        #endregion

        #region Utilities and Methods
        // Method to terminate the program.
        // Utilise a keyboard shortcut of Ctrl+Q.
        private void TerminateProgram()
        {
            traceListener.Flush();
            traceListener.Close();

            Close();
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

        // PreviewKeyDown Event for TextBoxStaff_Id to clear the text for staffName, prevent conflicting of filtering
        private void StaffId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                return; // If Tab is pressed, do nothing and return
            }
            else
            {
                TextBoxStaff_Name.Clear();
            }
            
        }

        // PreviewKeyDown Event for TextBoxStaff_Name to clear the text for staffId, prevent conflicting of filtering
        private void StaffName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                return; // If Tab is pressed, do nothing and return
            }
            else
            {
                TextBoxStaff_Id.Clear();
            }
            
        }


        // 4.8.	Create a method for the filtered and selectable list box which will populate the two text boxes when a staff record is selected.
        // Utilise the Tab and keyboard keys with keyboard shortcut of Ctrl+F.
        private void SelectStaffData()
        {
            if (listBoxFilter.SelectedIndex >= 0 && listBoxFilter.SelectedItem != null)
            {
                // Ref:https://stackoverflow.com/questions/6404520/how-to-get-listbox-selected-item-as-keyvaluepairstring-string-in-c
                var selectedItem = (KeyValuePair<int, string>)listBoxFilter.SelectedItem;

                TextBoxStaff_Name.Text = selectedItem.Value;
                TextBoxStaff_Id.Text = selectedItem.Key.ToString();
                StatusBarFeedback("Successful", $"Staff ID: {selectedItem.Key} is selected.");
            }
        }
        #endregion

        #region Open Admin Form
        // 4.9.	Create a method that will open the Admin GUI when the Alt + A keys are pressed.
        // Ensure the General GUI sends the currently selected Staff ID and Staff Name to the Admin GUI for Update and Delete purposes and is opened as modal.
        // Create modified logic to open the Admin GUI to Create a new user when the Staff ID 77 and the Staff Name is empty.
        // Read the appropriate criteria in the Admin GUI for further information.
        // Utilise the Tab and keyboard keys with keyboard shortcut of Alt+A.
        private void OpenAdminControl()
        {

            //traceListener.Flush();
            //traceListener.Close();

            AdminGUI adminControl = new AdminGUI(MasterFile, TextBoxStaff_Id.Text, traceListener);
            adminControl.ShowDialog();
            StatusBarClear();

        }
        #endregion

        #region ErrorTrapping
        // 4.10. Add suitable error trapping and user feedback via a status strip or similar to ensure a fully functional User Experience.
        // Make all methods private and ensure the Dictionary is static and public.
        private void StatusBarFeedback(string status_message, string feedback_message)
        {
            TextBlockStatus.Text = status_message;
            TextBlockFeedback.Text = feedback_message;
        }

        // Method to clear StatusBar
        private void StatusBarClear()
        {
            TextBlockStatus.Text = "Ready";
            TextBlockFeedback.Text = "";
            TimerTextBlock.Text = "";
        }
        #endregion       
    }
}
