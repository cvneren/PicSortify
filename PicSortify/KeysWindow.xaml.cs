using System;
using System.Collections.Generic;
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
using System.Windows.Forms;
using System.IO;

namespace PicSortify
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class KeysWindow : Window
    {
        Key folderKey;
        string folderPath;
        public KeysWindow(int keyIndex)
        {
            InitializeComponent();
            infoLabel.Content = "Please assign a key for output folder " + keyIndex;
        }

        private void folderPathButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderPathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderPathTextBox.Text == "") System.Windows.MessageBox.Show("Please provide a valid folder path");
            else if (!buttonAssignedLabel.Content.Equals("Key assigned"))
            {
                System.Windows.MessageBox.Show("Please assign a key");
            }
            else
            {
                try
                {

                    folderPath = folderPathTextBox.Text;
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    MainWindow.assignedKeys.Add(folderKey, folderPath);
                    ((MainWindow)this.Owner).UpdateKeysCount();
                    ((MainWindow)this.Owner).UpdateKeysAssignedText();
                    ((MainWindow)this.Owner).incrementKeyIndex();
                    this.Owner.Focus();
                    this.Close();
                    /*int pos = folderPathTextBox.Text.LastIndexOf("\\") + 1;
                    System.Windows.MessageBox.Show("\n" + folderPathTextBox.Text.Substring(pos, folderPathTextBox.Text.Length - pos));*/
                }
                catch (ArgumentException)
                {
                    System.Windows.MessageBox.Show("Folder already selected. Please select a different path.");
                }
            }
        }

        private void assignButton_Click(object sender, RoutedEventArgs e)
        {
            assignButton.Content = "Press a key";
        }

        private void assignButton_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (MainWindow.assignedKeys.ContainsKey(e.Key))
            {
                System.Windows.MessageBox.Show("Key already assigned. Please use anoher key");
            }
            else if (assignButton.Content.Equals("Press a key"))
            {
                folderKey = e.Key;
                buttonAssignedLabel.Content = "Key assigned";
                buttonAssignedLabel.Foreground = System.Windows.Media.Brushes.LightGreen;
                assignButton.Content = "Assign key";
            }
        }

        private void assignButton_LostFocus(object sender, RoutedEventArgs e)
        {
            assignButton.Content = "Assign key";
        }
    }
}
