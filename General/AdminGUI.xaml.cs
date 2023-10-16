using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace General
{
    /// <summary>
    /// Interaction logic for AdminGUI.xaml
    /// </summary>
    public partial class AdminGUI : Window
    {
        private Dictionary<int, string> _masterFile;
        string csvFilePath = Environment.CurrentDirectory + "\\MalinStaffNamesV2.csv";
        public int selectedStaffId;

        // Method to initialise AdminGUI
        public AdminGUI(Dictionary<int, string> masterFile, string id)
        {
            InitializeComponent();
            _masterFile = masterFile;
            if (!string.IsNullOrEmpty(id))
            {
                selectedStaffId = int.Parse(id);
            }
            else
            {
                selectedStaffId = 0;
            }
            DisplayStaffData();
            ShortCut_Command(Key.R, ModifierKeys.Alt, RemoveStaff);
            ShortCut_Command(Key.C, ModifierKeys.Alt, CreateStaff);
            ShortCut_Command(Key.U, ModifierKeys.Alt, UpdateStaff);
            ShortCut_Command(Key.G, ModifierKeys.Alt, GenerateNewStaffId);
            ShortCut_Command(Key.L, ModifierKeys.Alt, AdminGUIClose);
            ShortCut_Command(Key.S, ModifierKeys.Alt, SaveCsvFile);
            ShortCut_Command(Key.N, ModifierKeys.Alt, ClearTextBox_staffName);
        }

        // Method to create shortcut key and allow user to input parameter and function accordingly
        private void ShortCut_Command(Key input, ModifierKeys modifierKeys, Action function)
        {
            RoutedCommand command = new RoutedCommand();
            command.InputGestures.Add(new KeyGesture(input, modifierKeys));
            CommandBindings.Add(new CommandBinding(command, (sender, e) => function()));
        }

        // 5.2.	Create a method that will receive the Staff ID from the General GUI and then populate text boxes with the related data. 
        private void DisplayStaffData()
        {
            if (_masterFile.ContainsKey(selectedStaffId))
            {
                TextBoxStaff_Id.Text = selectedStaffId.ToString();
                TextBoxStaff_Name.Text = _masterFile[selectedStaffId];
                LabelUpdateOrRemove();
                StatusBarFeedback("Ready", $"{selectedStaffId} is selected for update or remove.");
            }
            else
            {
                TextBoxStaff_Name.Clear();
                TextBoxStaff_Id.Clear();
                LabelCreate();
                StatusBarFeedback("Ready", "No existing record selected. Input a new staff name to create a record."); }
        }

        // 5.3.	Create a method that will create a new Staff ID and input the staff name from the related text box.
        // The Staff ID must be unique starting with 77xxxxxxx while the staff name may be duplicated.
        // The new staff member must be added to the Dictionary data structure.               
        private void CreateStaff()
        {

            if (string.IsNullOrEmpty(TextBoxStaff_Id.Text))
            {
                GenerateNewStaffId();
            }
            int newStaffId = int.Parse(TextBoxStaff_Id.Text);

            if (IsValidStaffName(TextBoxStaff_Name.Text))
            {
                string newStaffName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TextBoxStaff_Name.Text.ToLower());

                _masterFile[newStaffId] = newStaffName;
                StatusBarFeedback("Successful", $"Staff record is created! New Staff ID: {newStaffId}");
            }
            else
            {
                return;
            }
        }

        // Method to generate new staffID as the TextBoxStaff_Id is readonly as per client requirement.
        private void GenerateNewStaffId()
        {
            if (string.IsNullOrEmpty(TextBoxStaff_Id.Text))
            {
                Random random = new Random();
                int generatedId;

                do
                {
                    generatedId = random.Next(700000000, 799999999);
                } while (_masterFile.ContainsKey(generatedId));

                TextBoxStaff_Id.Text = generatedId.ToString();
                StatusBarFeedback("Succesful", $"New Staff ID is generated: {generatedId}");
            }
        }

        // Method to check if the input TextBoxStaff_Name is valid including first and last name.
        private bool IsValidStaffName(string staffName)
        {
            string[] name = staffName.Split(" ");
            if (string.IsNullOrEmpty(staffName))
            {
                StatusBarFeedback("Error", "Staff Name Textbox is Empty!");
                return false;
            }            
            else if (name.Length != 2)
            {
                StatusBarFeedback("Error", "First and Last name is required!");
                return false;
            }
            else
            {
                return true;
            }
        }

        // 5.4.	Create a method that will Update the name of the current Staff ID.
        private void UpdateStaff()
        {

            if (int.TryParse(TextBoxStaff_Id.Text, out int updatedId) && _masterFile.ContainsKey(updatedId))
            {
                if (IsValidStaffName(TextBoxStaff_Name.Text))
                {
                    string updatedStaffName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(TextBoxStaff_Name.Text.ToLower());

                    _masterFile[updatedId] = updatedStaffName;
                    StatusBarFeedback("Successful", $"ID: {updatedId} updated with Staff name: {updatedStaffName}");
                }
                else
                {
                    return;
                }
            }
        }

        // 5.5.	Create a method that will Remove the current Staff ID and clear the text boxes.
        private void RemoveStaff()
        {
            if (int.TryParse(TextBoxStaff_Id.Text, out int removedId) && _masterFile.ContainsKey(removedId))
            {
                _masterFile.Remove(removedId);
                TextBoxStaff_Id.Clear();
                TextBoxStaff_Name.Clear();
                StatusBarFeedback("Successful", $"Staff record is deleted! Staff ID: {removedId}");
            }
        }

        // 5.6.	Create a method that will save changes to the csv file, this method should be called as the Admin GUI closes.
        private void SaveCsvFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    foreach (var record in _masterFile)
                    {
                        writer.WriteLine($"{record.Key},{record.Value}");
                        StatusBarFeedback("Successful", $"File is saved: {csvFilePath}");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading CSV file: " + csvFilePath + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusBarFeedback("Error", $"Failed to save file: {csvFilePath}");
            }
        }

        // 5.7.	Create a method that will close the Admin GUI when the Alt + L keys are pressed.
        private void AdminGUIClose()
        {
            SaveCsvFile();
            Close();
        }

        // Method to clear TextBoxStaff_Name
        private void ClearTextBox_staffName()
        {
            TextBoxStaff_Name.Clear();
            TextBoxStaff_Name.Focus();
            StatusBarFeedback("Ready", "Staff Name Text Box is cleared.");
        }

        // Method to create customize command shortcut for Create Only.
        private void LabelCreate()
        {
            LabelCommand.Content = "Keyboard Command:\nAlt - L: Save & Close Admin Control\nAlt - S : Save Data to Csv\nAlt - C: Create Staff" +
                "\nAlt - G: Generate New Staff Id\nAlt - N: Clear and Focus Staff Name\nTab: Navigate Control";
        }

        // Method to create customize command shortcut for Update and Remove Only.
        private void LabelUpdateOrRemove()
        {
            LabelCommand.Content = "Keyboard Command:\nAlt - L: Save & Close Admin Control\nAlt - S : Save Data to Csv\nAlt - R: Remove Staff" +
                "\nAlt - U: Update Staff\nAlt - N: Clear and Focus Staff Name\nTab: Navigate Control";
        }

        // 5.8.	Add suitable error trapping and user feedback via a status strip or similar to ensure a fully functional User Experience.
        // Make all methods private and ensure the Dictionary is updated. 
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
