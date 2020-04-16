using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
using System.IO;

namespace sp3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolidColorBrush def;
        string source = "";
        string dest = "";
        bool encode;
        public MainWindow()
        {
            InitializeComponent();
            BrushConverter brushConverter = new BrushConverter();
            def = (brushConverter.ConvertFromString("#FFABADB3") as SolidColorBrush);
        }

        private void Source_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "*.txt | *.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                tbSource.BorderBrush = def;
                source = tbSource.Text = openFileDialog.FileName;
            }
        }

        private void Destination_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "*.txt | *.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                tbDestination.BorderBrush = def;
                dest = tbDestination.Text = saveFileDialog.FileName;
            }
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            tbKey.BorderBrush = def;
            if (String.IsNullOrWhiteSpace(tbSource.Text))
            {
                tbSource.BorderBrush = Brushes.Red;
                tbSource.Text = "Select source";
                return;
            }
            if (String.IsNullOrWhiteSpace(tbDestination.Text))
            {
                tbDestination.BorderBrush = Brushes.Red;
                tbDestination.Text = "Select destination";
                return;
            }
            if (String.IsNullOrWhiteSpace(tbKey.Text))
            {
                tbKey.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter code key");
                return;
            }
            encode = Convert.ToBoolean(rbEncode.IsChecked);
            Thread thread = new Thread(new ParameterizedThreadStart(Code));
            thread.IsBackground = true;
            thread.Start(tbKey.Text);
        }
        public void Code(object obj)
        {
            byte key = Convert.ToByte(obj);
            byte[] file = File.ReadAllBytes(source);
            progress.Dispatcher.Invoke(() =>
            {
                progress.Maximum = file.Length;
                progress.Value = 0;
            });
            for (int i = 0; i < file.Length; i++)
            {
                if (encode)
                    file[i] += key;
                else
                    file[i] -= key;
                progress.Dispatcher.Invoke(() => progress.Value++);
            }
            File.WriteAllBytes(dest, file);
            MessageBox.Show("Done");
            progress.Dispatcher.Invoke(() => progress.Value = 0);
        }
    }
}
