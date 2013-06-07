﻿using LazyLog.Framework;
using LazyLog.LogProviders;
using LazyLog.ViewModel.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace LazyLog.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private SafeObservableCollection<LogViewModel> _logs = new SafeObservableCollection<LogViewModel>();
        private SafeObservableCollection<LogRecord> _logRecords = new SafeObservableCollection<LogRecord>();
        private ILogProvdier _logProvider;
        private ILogParser _logParser;

        #region Commands Properties
        
        private RelayCommand _openFileCommand = null;
        public ICommand OpenFileCommand  { get  {  return _openFileCommand; } }

        private RelayCommand _closeFileCommand = null;
        public ICommand CloseFileCommand { get { return _closeFileCommand; } }

        private RelayCommand _openLogWindowCommand = null;
        public ICommand OpenLogWindowCommand { get { return _openLogWindowCommand; } }
        
        #endregion // Commands Properties 


        #region Title
        private string _title = null;
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
        #endregion

        public MainWindowViewModel()
        {
            _openFileCommand = new RelayCommand((p) => OnOpenFile(p));
            _closeFileCommand = new RelayCommand((p) => OnCloseFile(p));
            _openLogWindowCommand = new RelayCommand((p) => OnOpenLogWindow(p), (p) => CanOpenLogWindow(p));
            _logParser = new CsvLogParser();

            Title = "LazyLog";            
        }


        #region Commands Actions

        private void OnOpenFile(object parameter)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {   
                CloseFile();
                OpenFile(dlg.FileName);             
            }
        }

        private void OnCloseFile(object parameter)
        {
            CloseFile();
        }

        private void OnOpenLogWindow(object parameter)
        {
            OpenLogWindow();
        }

        private bool CanOpenLogWindow(object parameter)
        {
            return _logProvider != null;    
        }

        #endregion


        private void OpenFile(string filePath)
        {
            if (_logProvider == null)
            {
                _logProvider = new FileLogProvider(filePath);
                _logProvider.OnNewData += HandleNewData;
                _logProvider.Start();
                
                Title = "LazyLog - " + _logProvider.Description;
                OpenLogWindow();                
            }
        }

        private void HandleNewData(string newData)
        {
            IEnumerable<LogRecord> newRecords = _logParser.Parse(newData);
            Dispatcher.CurrentDispatcher.Invoke((Action) 
                (() =>
                {
                    foreach (LogRecord record in newRecords)
                    {
                        _logRecords.Add(record);
                    }
                }));
        }

        private void CloseFile()
        {
            CloseLogWindows();

            if (_logProvider != null)
	        {
		        _logProvider.Stop();
                _logProvider.OnNewData -= HandleNewData;
                _logProvider = null;
	        }
        }

        private void CloseLogWindows()
        {
            _logs.Clear();
            RaisePropertyChanged("Logs");
        }

        private void OpenLogWindow()
        {
            var logViewModel = new LogViewModel(_logRecords);
            _logs.Add(logViewModel);
            RaisePropertyChanged("Logs");
        }
    }
}