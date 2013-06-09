using LazyLog.Framework;
using LazyLog.LogProviders;
using LazyLog.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace LazyLog.ViewModel
{
    class LogViewModel : ViewModelBase
    {           
        public ICollectionView FilteredLogRecords { get; private set;}
        public object SelectedItem { get; private set; }

        private RelayCommand _filterCommand = null;
        public ICommand FilterCommand { get { return _filterCommand; } }

        #region Title
        
        private IList<FilterOption> _filterOptions;
        public IList<FilterOption> FilterOptions
        {
            get
            {
                return _filterOptions;
            }

            private set
            {
                _filterOptions = value;

                if (_filterOptions.Count == 0)
                {
                    FilteredLogRecords.Filter = (item) => true;
                }
                else
                {
                   IEnumerable<Predicate<LogRecord>> predicates = _filterOptions.Select(option => option.Predicate);
                   FilteredLogRecords.Filter = (item) => PredicatesExtensions.And<LogRecord>(predicates)(item as LogRecord);
                }

                RaisePropertyChanged("Filter");
                RaisePropertyChanged("LogViewTitle");
                RaisePropertyChanged("FilteredLogRecords");
            }
        }       
        
        public string LogViewTitle
        {
            get
            {
                if (_filterOptions.Count == 0)
                {
                    return "Unfiltered";
                }
                return String.Join(" AND ", _filterOptions.Select(option => option.Description));
            }
        }

        #endregion


        public LogViewModel(ObservableCollection<LogRecord> logRecords) : this(logRecords, new List<FilterOption>())
        {                       
        }

        public LogViewModel(ObservableCollection<LogRecord> logRecords, IList<FilterOption> filter)          
        {
            FilteredLogRecords = CollectionViewSource.GetDefaultView(logRecords);
            FilterOptions = filter;
            _filterCommand = new RelayCommand((p) => { RunFilter(p); }, (p) => CanFilter(p));
        }

        #region FilterMenu
        private bool CanFilter(object p)
        {
            return SelectedItem != null;
        }

        private void RunFilter(object p)
        {
            FilterOption option = p as FilterOption;

            if (!_filterOptions.Contains(option))
            {
                _filterOptions.Add(option);
                FilterOptions = _filterOptions;
            }            
        }

        public IList<FilterOption> MenuFilterOptions
        {            
            get
            {
                var currentRecord = SelectedItem as LogRecord;
                if (currentRecord == null)
                {
                    return null;
                }

                return new FilterOption[] 
                { 
                    new FilterOption(String.Format("ThreadId is {0}",  currentRecord.ThreadId), (record) => (record.ThreadId == currentRecord.ThreadId)),
                    new FilterOption(String.Format("ProcessId is {0}",  currentRecord.ProcessId), (record) => (record.ProcessId == currentRecord.ProcessId)),
                    new FilterOption(String.Format("ModuleName is {0}",  currentRecord.ModuleName), (record) => (record.ModuleName == currentRecord.ModuleName)),
                    new FilterOption(String.Format("ThreadId is not {0}",  currentRecord.ThreadId), (record) => (record.ThreadId != currentRecord.ThreadId)),
                    new FilterOption(String.Format("ProcessId is not {0}",  currentRecord.ProcessId), (record) => (record.ProcessId != currentRecord.ProcessId)),
                    new FilterOption(String.Format("ModuleName is not {0}",  currentRecord.ModuleName), (record) => (record.ModuleName != currentRecord.ModuleName)),
                };
            }
        }     

        #endregion     
    }
}
