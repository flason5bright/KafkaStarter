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

		public void Dispose()
		{
			if (zoKeeperProcess != null)
				zoKeeperProcess.Close();
			if (kafkaServerProcess != null)
				kafkaServerProcess.Close();
			if (commandProcess != null)
				commandProcess.Close();
		}

		public ObservableCollection<string> zoKeeperConsoles { get; set; } = new ObservableCollection<string>();

		public ICommand StartZokeeper { get { return new RelayCommand(StartZokeeperExcute); } }
		public ICommand StartKafkaServer { get { return new RelayCommand(StartKafkaServerExcute); } }
		public ICommand StartCommand { get { return new RelayCommand(StartCommandExcute); } }
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
			{
				zoKeeperProcess.Close();
				zoKeeperConsoles.Clear();
			}

			StringBuilder sb = new StringBuilder();
			zoKeeperProcess = ProcessHelper.CreateProcess("zkServer", new System.Diagnostics.DataReceivedEventHandler((o, e) =>
			{
				App.Current.Dispatcher.Invoke(() =>
				{
					zoKeeperConsoles.Add(e.Data);
				});
			}), "");
		}

		public ObservableCollection<string> kafkaServerConsoles { get; set; } = new ObservableCollection<string>();

		private Process kafkaServerProcess;
		private void StartKafkaServerExcute()
		{
			if (kafkaServerProcess != null)
			{
				kafkaServerProcess.Close();
				kafkaServerConsoles.Clear();
			}
			StringBuilder sb = new StringBuilder();
			kafkaServerProcess = ProcessHelper.CreateProcess(@".\bin\windows\kafka-server-start.bat .\config\server.properties", new System.Diagnostics.DataReceivedEventHandler((o, e) =>
			{
				App.Current.Dispatcher.Invoke(() =>
				{
					kafkaServerConsoles.Add(e.Data);
				});
			}), KafkaRootPath);
		}

		private string commandText =
			@".\bin\windows\kafka-topics.bat --create --zookeeper localhost:2181 --replication-factor 1 --partitions 1 --topic testDemo";
		public string CommandText
		{
			get { return commandText; }
			set
			{
				commandText = value;
				NotifyPropertyChanged(nameof(CommandText));
			}
		}

		public ObservableCollection<string> commandConsoles { get; set; } = new ObservableCollection<string>();

		private Process commandProcess;
		private void StartCommandExcute()
		{
			if (commandProcess == null)
			{
				commandProcess = ProcessHelper.CreateProcess(KafkaRootPath,
					new System.Diagnostics.DataReceivedEventHandler((o, e) =>
					{
						App.Current.Dispatcher.Invoke(() => { commandConsoles.Add(e.Data); });
					}));
			}

			commandProcess.StandardInput.WriteLine(CommandText);
		}

	}
}
