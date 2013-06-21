using System.Linq;
using LazyLog.Framework;
using LazyLog.LogProviders;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LazyLog.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private const int MaxMruSize = 10;

        private readonly SafeObservableCollection<LogViewModel> _logs = new SafeObservableCollection<LogViewModel>();
        private readonly SafeObservableCollection<LogRecord> _logRecords = new SafeObservableCollection<LogRecord>();
        private readonly SafeObservableCollection<string> _recentFiles = new SafeObservableCollection<string>();

        private ILogProvdier _logProvider;
        private readonly ILogParser _logParser;
        private readonly MruManager _mruManager;
        private readonly ICollectionViewCreator _iCollectionViewCreator;
        
        #region Commands Properties
        
        public ICommand OpenFileCommand  { get; private set; }
        public ICommand CloseFileCommand  { get; private set; }
        public ICommand OpenLogWindowCommand  { get; private set; }
        public ICommand OpenRecentFileCommand  { get; private set; }
        public ICommand PauseMonitoringCommand { get; private set; }
        public ICommand ResumeMonitoringCommand  { get; private set; }
        public ICommand ClearLogCommand { get; private set; }

        #endregion Commands Properties 


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

        #endregion Logs Collection


        #region MRU

        public ObservableCollection<string> RecentFiles
        {
            get
            {
                return _recentFiles;
            }
        }
        
        #endregion

        #region ActiveDocument

        private LogViewModel _activeLog;

        public LogViewModel ActiveDocument
        {
            get { return _activeLog; }
            set
            {
                if (_activeLog != value)
                {
                    _activeLog = value;
                    RaisePropertyChanged("ActiveDocument");
                }
            }
        }

        #endregion ActiveDocument


        public MainWindowViewModel(ICollectionViewCreator iCollectionViewCreator)
        {
            Title = "LazyLog";            
            _iCollectionViewCreator = iCollectionViewCreator;
            _logParser = new CsvLogParser();
            _mruManager = new MruManager(_recentFiles, MaxMruSize);            

            OpenFileCommand = new RelayCommand(
                p => {
                         var dlg = new OpenFileDialog();
                         if (dlg.ShowDialog().GetValueOrDefault())
                         {   
                             CloseFile();
                             OpenFile(dlg.FileName);             
                         }
                });
            
            CloseFileCommand = new RelayCommand(
                p => CloseFile());
            
            OpenLogWindowCommand = new RelayCommand(
                p => OpenLogWindow(new List<FilterOption>()), 
                p => IsFileOpen());
            
            OpenRecentFileCommand = new RelayCommand(
                p => { CloseFile(); OpenFile(p as String); }, 
                p => _recentFiles.Any());
            
            PauseMonitoringCommand = new RelayCommand(
                p => _logProvider.Stop(), 
                p => _logProvider != null && _logProvider.IsRunning);
            
            ResumeMonitoringCommand = new RelayCommand(
                p =>  _logProvider.Start(), 
                p => _logProvider != null && !_logProvider.IsRunning);
            
            ClearLogCommand = new RelayCommand(
                p => _logRecords.Clear(),                    
                p => IsFileOpen());                         
        }        

        private bool IsFileOpen()
        {
            return _logProvider != null;
        }

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
            _logRecords.AddRange(newRecords);
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

            ActiveDocument = _logs.Last();
        }        
    }
}