using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

        public ICommand StartZokeeper { get { return new RelayCommand(StartZokeeperExcute); } }

        private Process zoKeeperProcess;
        private void StartZokeeperExcute()
        {
            if (zoKeeperProcess != null)
                zoKeeperProcess.Close();
            StringBuilder sb = new StringBuilder();
            zoKeeperProcess = ProcessHelper.CreateProcess("ping 127.0.0.1", new System.Diagnostics.DataReceivedEventHandler((o, e) =>
            {
                ZoKeeperConsole = sb.Append(e.Data).Append("\r\n").ToString();
            }));



            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0, 5);
            //timer.Tick += new EventHandler((object sender, EventArgs e) =>
            // {
            //     ZoKeeperConsole = process.StandardOutput.ReadToEnd();
            // });  
            //timer.Start();

        }

    }
}
