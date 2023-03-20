using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Forms;

namespace PicSortify
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Folder
    {
        public Key key;
        public string path;
        public Folder()
        {
            key = Key.None;
            path = null;
        }
    }

    public partial class MainWindow : Window
    {
        //Setup variables
        private int keyIndex = 1;
        public static Dictionary<Key, string> assignedKeys = new Dictionary<Key, string>();
        IEnumerable<string> inputFiles;
        int imagesCount;
        int curCount;
        FileStream stream;
        Key lastkey = Key.None;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void selectInputFolder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inputFPath.Text = dialog.SelectedPath;
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if(inputFPath.Text == "Select an input folder...")
            {
                System.Windows.MessageBox.Show("Please set a valid input folder.");
            }
            //Possible Todo force assignment windows to pop up as long as outputfolders < 2
            else
            {
                //Todo Add Toggle Option for searching subdirectories -> EnumerateFiles(x, y, SearchOption.AllDirectories)
                //Todo Toggles in MainWindow for various filetypes -> Checkboxes for jpg, png, tiff, gif etc / Currently set to jpg and png only!

                //read every image in source directory
                inputFiles = Directory.EnumerateFiles(inputFPath.Text, "*.*")
                    .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png"));
                imagesCount = inputFiles.Count();
                curCount = 0;

                //overlay imageviewer
                Overlay.Visibility = Visibility.Visible;
                if(!(this.WindowState == WindowState.Maximized) && this.Width == 360 && this.Height == 300)
                {
                    this.Height = 540;
                    this.Width = 720;
                }
                stream = File.Open(inputFiles.ElementAt(curCount), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                BitmapImage curImage = new BitmapImage();
                curImage.BeginInit();
                curImage.StreamSource = stream;
                curImage.EndInit();
                ImageLoader.Source = curImage;

                UpdateImageCount(curCount, imagesCount);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Todo Undo for last file move
            /*if (e.Key == Key.Z && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && lastkey != Key.None)
            {
                int pos = inputFiles.ElementAt(curCount - 1).LastIndexOf("\\") + 1;
                string filename = inputFiles.ElementAt(curCount - 1).Substring(pos, inputFiles.ElementAt(curCount - 1).Length - pos);
                int realpos = inputFiles.ElementAt(curCount).LastIndexOf("\\") + 1;
                string realfilename = inputFiles.ElementAt(curCount).Substring(pos, inputFiles.ElementAt(curCount).Length - pos);
                System.Windows.MessageBox.Show(pos + "\n" + filename + "\n" + realpos +"\n" + realfilename);
                if (File.Exists(assignedKeys[lastkey] + "\\" + filename))
                {
                    stream.Close();
                    
                    File.Move(assignedKeys[lastkey] + "\\" + filename, inputFPath.Text);
                    curCount -= 1;

                    stream = File.Open(inputFiles.ElementAt(curCount), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BitmapImage curImage = new BitmapImage();
                    curImage.BeginInit();
                    curImage.StreamSource = stream;
                    curImage.DecodePixelHeight = (int)ImageLoader.ActualHeight;
                    curImage.EndInit();
                    ImageLoader.Source = curImage;

                    UpdateImageCount(curCount, imagesCount);
                    lastkey = Key.None;
                }
            }*/
            //Optional-Todo Allow scrolling with left/right arrow keys
            //if(e.Key == Key.Right)
            //{

            //}
            //if(e.Key == Key.Left)
            //{

            //}
            if(assignedKeys.ContainsKey(e.Key))
            {
                try
                {
                    stream.Close();
                    int pos = inputFiles.ElementAt(curCount).LastIndexOf("\\") + 1;
                    string filename = inputFiles.ElementAt(curCount).Substring(pos, inputFiles.ElementAt(curCount).Length - pos);
                    File.Move(inputFiles.ElementAt(curCount), assignedKeys[e.Key]+ "\\" + filename);
                    curCount += 1;

                    stream = File.Open(inputFiles.ElementAt(curCount), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BitmapImage curImage = new BitmapImage();
                    curImage.BeginInit();
                    curImage.StreamSource = stream;
                    curImage.DecodePixelHeight = (int)ImageLoader.ActualHeight;
                    curImage.EndInit();
                    ImageLoader.Source = curImage;
                    
                    UpdateImageCount(curCount, imagesCount);
                    lastkey = e.Key;
                }
                catch (ArgumentOutOfRangeException)
                {
                    curCount = 0;
                    imagesCount = 0;
                    Overlay.Visibility = Visibility.Hidden;
                    ImageLoader.Source = null;
                    inputFiles = null;
                    UpdateImageCount(0, 0);
                }
                catch (ArgumentNullException)
                {
                    return;
                }
            }
        }

        private void UpdateImageCount(int cur, int imageFiles)
        {
            imagesCountLabel.Content = (cur+1) + " / " + imageFiles;
        }

        public void incrementKeyIndex()
        {
            keyIndex += 1;
        }

        public void resetKeys_Click(object sender, RoutedEventArgs e)
        {
            keyIndex = 1;
            assignedKeys.Clear();
            UpdateKeysCount();
            UpdateKeysAssignedText();
        }

        private void addKey_Click(object sender, RoutedEventArgs e)
        {
            KeysWindow keysWindow = new KeysWindow(keyIndex);
            keysWindow.Owner = this;
            keysWindow.Show();
        }

        public void UpdateKeysCount()
        {
            keysCount.Content = assignedKeys.Count();
        }

        public void UpdateKeysAssignedText()
        {
            if (keysCount.Content.Equals("0"))
            {
                keysAssignedText.Text = "No keys assigned";
            }
            else
            {
                string content = "";
                foreach (var k in assignedKeys)
                {
                    content += k.Key.ToString() + ": " + k.Value + "\n";
                }
                keysAssignedText.Text = content;
            }
        }

        private void inputFPath_Focus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox tb = (sender as System.Windows.Controls.TextBox);
            if (tb != null)
            {
                tb.SelectAll();
            }
        }

        private void inputFPath_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.TextBox tb = (sender as System.Windows.Controls.TextBox);
            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    tb.Focus();
                }
            }
        }
    }
}
