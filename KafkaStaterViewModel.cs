using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace KafkaStarter
{
    public class KafkaStaterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string zoKeeperConsole = "";
        public string ZoKeeperConsole
        {
            get { return zoKeeperConsole; }
            set
            {
                zoKeeperConsole = value;
                NotifyPropertyChanged(nameof(ZoKeeperConsole));
            }
        }

        private string kafkaRootPath = "Select Kafka root path";
        public string KafkaRootPath
        {
            get { return kafkaRootPath; }
            set
            {
                kafkaRootPath = value;
                NotifyPropertyChanged(nameof(KafkaRootPath));
            }
        }

        public ObservableCollection<string> zoKeeperConsoles { get; set; } = new ObservableCollection<string>();

        public ICommand StartZokeeper { get { return new RelayCommand(StartZokeeperExcute); } }
        public ICommand StartKafkaServer { get { return new RelayCommand(StartKafkaServerExcute); } }
        public ICommand OpenDirectorySelector
        {
            get
            {
                return new RelayCommand(() =>
                {
                    FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                    if (folderBrowser.ShowDialog() == DialogResult.OK)
                    {
                        KafkaRootPath = folderBrowser.SelectedPath;
                    }
                });
            }
        }


        private Process zoKeeperProcess;
        private void StartZokeeperExcute()
        {
            if (zoKeeperProcess != null)
                zoKeeperProcess.Close();
            StringBuilder sb = new StringBuilder();
            zoKeeperProcess = ProcessHelper.CreateProcess("zkServer", new System.Diagnostics.DataReceivedEventHandler((o, e) =>
            {
                //ZoKeeperConsole = sb.Append(e.Data).Append("\r\n").ToString();
                App.Current.Dispatcher.Invoke(() =>
                {
                    zoKeeperConsoles.Add(e.Data);
                });
            }));
        }

        public ObservableCollection<string> kafkaServerConsoles { get; set; } = new ObservableCollection<string>();

        private Process kafkaServerProcess;
        private void StartKafkaServerExcute()
        {
            if (kafkaServerProcess != null)
                kafkaServerProcess.Close();
            StringBuilder sb = new StringBuilder();
            kafkaServerProcess = ProcessHelper.CreateProcess(@".\bin\windows\kafka-server-start.bat .\config\server.properties", KafkaRootPath, new System.Diagnostics.DataReceivedEventHandler((o, e) =>
            {
                //ZoKeeperConsole = sb.Append(e.Data).Append("\r\n").ToString();
                App.Current.Dispatcher.Invoke(() =>
                {
                    kafkaServerConsoles.Add(e.Data);
                });
            }));
        }

    }
}
