using LazyLog.Framework;
using LazyLog.LogProviders;
using LazyLog.ViewModel.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace LazyLog.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private const int MAX_MRU_SIZE = 10;

        private readonly SafeObservableCollection<LogViewModel> _logs = new SafeObservableCollection<LogViewModel>();
        private readonly SafeObservableCollection<LogRecord> _logRecords = new SafeObservableCollection<LogRecord>();
        private readonly SafeObservableCollection<string> _recentFiles = new SafeObservableCollection<string>();

        private ILogProvdier _logProvider;
        private readonly ILogParser _logParser;
        private readonly MruManager _mruManager;
        private readonly ICollectionViewCreator _iCollectionViewCreator;
        #region Commands Properties
        
        private readonly RelayCommand _openFileCommand;
        public ICommand OpenFileCommand  { get  {  return _openFileCommand; } }

        private readonly RelayCommand _closeFileCommand;
        public ICommand CloseFileCommand { get { return _closeFileCommand; } }

        private readonly RelayCommand _openLogWindowCommand;
        public ICommand OpenLogWindowCommand { get { return _openLogWindowCommand; } }

        private readonly RelayCommand _openRecentFileCommand;
        public ICommand OpenRecentFileCommand { get { return _openRecentFileCommand; } }

        private readonly RelayCommand _pauseMonitoringCommand;
        public ICommand PauseMonitoringCommand { get { return _pauseMonitoringCommand; } }

        private readonly RelayCommand _resumeMonitoringCommand;
        public ICommand ResumeMonitoringCommand { get { return _resumeMonitoringCommand; } }

        #endregion // Commands Properties 


        #region Title

        private string _title;       
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        #endregion
        
        
        #region Logs Collection

        public ObservableCollection<LogViewModel> Logs
        {
            get
            {
                return _logs;
            }
        }

        public ObservableCollection<LogRecord> LogRecords
        {
            get
            {
                return _logRecords;
            }
        }

        #endregion


        #region MRU
        
        public ObservableCollection<string> RecentFiles
        {
            get
            {
                return _recentFiles;
            }
        }
        
        #endregion


        public MainWindowViewModel(ICollectionViewCreator iCollectionViewCreator)
        {
            _iCollectionViewCreator = iCollectionViewCreator;
            _openFileCommand = new RelayCommand(p => OnOpenFile());
            _closeFileCommand = new RelayCommand(p => OnCloseFile());
            _openLogWindowCommand = new RelayCommand(p => OnOpenLogWindow(), p => CanOpenLogWindow());
            _openRecentFileCommand = new RelayCommand(OnOpenRecentFile, p => CanOpenRecentFile());
            _pauseMonitoringCommand = new RelayCommand(p => OnPauseMonitor(), p => CanPauseMonitor());
            _resumeMonitoringCommand = new RelayCommand(p => OnResumeMonitor(), p => CanResumeMonitor());
             
        
            _logParser = new CsvLogParser();
            _mruManager = new MruManager(_recentFiles, MAX_MRU_SIZE);

            Title = "LazyLog";            
        }


        #region Commands Actions

        private void OnOpenFile()
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {   
                CloseFile();
                OpenFile(dlg.FileName);             
            }
        }

        private void OnCloseFile()
        {
            CloseFile();
        }

        private void OnOpenLogWindow()
        {
            OpenLogWindow(new List<FilterOption>());
        }

        private bool CanOpenLogWindow()
        {
            return _logProvider != null;    
        }


        private void OnOpenRecentFile(object parameter)
        {
            CloseFile();
            OpenFile(parameter as String); 
        }

        private bool CanOpenRecentFile()
        {
            return _recentFiles.Count > 0;
        }


        private void OnPauseMonitor()
        {
            _logProvider.Stop();
        }

        private bool CanPauseMonitor()
        {
            return (_logProvider != null && _logProvider.IsRunning);
        }

        private void OnResumeMonitor()
        {
            _logProvider.Start();
        }

        private bool CanResumeMonitor()
        {
            return (_logProvider != null && !_logProvider.IsRunning);
        }

        #endregion


        private void OpenFile(string filePath)
        {
            if (_logProvider == null)
            {
                _logProvider = new FileLogProvider(filePath);
                _logProvider.OnNewData += HandleNewData;
                _logProvider.Start();

                _mruManager.Add(filePath);
                
                Title = "LazyLog - " + _logProvider.Description;
                OpenLogWindow(new List<FilterOption>());                
            }
        }

        private void HandleNewData(string newData)
        {
            IEnumerable<LogRecord> newRecords = _logParser.Parse(newData);
            Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    foreach (LogRecord record in newRecords)
                    {
                        _logRecords.Add(record);
                    }
                });
        }

        private void CloseFile()
        {
            CloseLogWindows();

            if (_logProvider != null)
	        {
		        _logProvider.Stop();
                _logProvider.OnNewData -= HandleNewData;
                _logProvider = null;
                _logRecords.Clear();
	        }
        }

        private void CloseLogWindows()
        {
            _logs.Clear();
            RaisePropertyChanged("Logs");
        }

        private void OpenLogWindow(IList<FilterOption> filterOptions)
        {            
            var logViewModel = new LogViewModel(_iCollectionViewCreator.CreateView(_logRecords), filterOptions);
            logViewModel.OnNewTabRequest += OpenLogWindow;
            _logs.Add(logViewModel);            
            RaisePropertyChanged("Logs");
        }        
    }
}