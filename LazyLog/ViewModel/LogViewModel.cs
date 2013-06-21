using System.Collections.Specialized;
using LazyLog.Framework;
using LazyLog.LogProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace LazyLog.ViewModel
{
    public delegate void NewTabRequestHandler(IList<FilterOption> filterOptions);

    class LogViewModel : ViewModelBase
    {
        public event NewTabRequestHandler OnNewTabRequest;
        public ICollectionView FilteredLogRecords { get; private set;}     

        public ICommand FilterCommand { get; private set; }        
        public ICommand FilterInTabCommand { get; private set; }
        public ICommand ClearFiltersCommand { get; private set; }

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
                    FilteredLogRecords.Filter = item => true;
                }
                else
                {
                   IEnumerable<Predicate<LogRecord>> predicates = _filterOptions.Select(option => option.Predicate);
                   FilteredLogRecords.Filter = item => PredicatesExtensions.And(predicates)(item as LogRecord);
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

        #region SelectedItem

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged("SelectedItem");
                }
            }
        }
        #endregion SelectedItem

        #region AutoScroll

        private Boolean _isAutoScroll;

        public Boolean IsAutoScroll
        {
            get { return _isAutoScroll; }
            set
            {
                if (_isAutoScroll != value)
                {
                    _isAutoScroll = value;
                    UpdateAutoScrolling();
                    RaisePropertyChanged("IsAutoScroll");
                }
            }
        }

        private void UpdateAutoScrolling()
        {
            if (IsAutoScroll)
            {
                FilteredLogRecords.CollectionChanged += AutoScroll;
                ScrollDown();
            }
            else
            {
                FilteredLogRecords.CollectionChanged -= AutoScroll;
            }
        }

        private void ScrollDown()
        {
            FilteredLogRecords.MoveCurrentToLast();           
            SelectedItem = FilteredLogRecords.CurrentItem;
        }

        private void AutoScroll(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            ScrollDown();
        }

        #endregion AutoScroll

        public LogViewModel(ICollectionView filteredRecords, IList<FilterOption> filter)          
        {
            FilteredLogRecords = filteredRecords;
            FilterOptions = filter;
            FilterCommand = new RelayCommand(RunFilter, CanFilter);
            FilterInTabCommand = new RelayCommand(RunFilterInTab, CanFilter);
            ClearFiltersCommand = new RelayCommand(ClearFilters, p => FilterOptions.Count > 0);
            
            IsAutoScroll = true;
        }

        #region FilterMenu
        private bool CanFilter(object p)
        {
            return SelectedItem != null;
        }

        private void RunFilter(object p)
        {
            var option = p as FilterOption;

            if (!_filterOptions.Contains(option))
            {
                _filterOptions.Add(option);
                FilterOptions = _filterOptions;
            }            
        }

        private void ClearFilters(object p)
        {
            var option = p as FilterOption;
            _filterOptions.Remove(option);
            FilterOptions = _filterOptions;
        }

        private void RunFilterInTab(object p)
        {
            var option = p as FilterOption;
            IList<FilterOption> currentOptions = new List<FilterOption>(_filterOptions);

            if (!currentOptions.Contains(option))
            {
                currentOptions.Add(option);                
            }

            FireNewTabRequestEvent(currentOptions);
        }

        private void FireNewTabRequestEvent(IList<FilterOption> currentOptions)
        {
            if (OnNewTabRequest != null)
            {
                OnNewTabRequest(currentOptions);
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

                return new[] 
                { 
                    new FilterOption(String.Format("ThreadId is {0}",  currentRecord.ThreadId), record => (record.ThreadId == currentRecord.ThreadId)),
                    new FilterOption(String.Format("ProcessId is {0}",  currentRecord.ProcessId), record => (record.ProcessId == currentRecord.ProcessId)),
                    new FilterOption(String.Format("ModuleName is {0}",  currentRecord.ModuleName), record => (record.ModuleName == currentRecord.ModuleName)),
                    new FilterOption(String.Format("ThreadId is not {0}",  currentRecord.ThreadId), record => (record.ThreadId != currentRecord.ThreadId)),
                    new FilterOption(String.Format("ProcessId is not {0}",  currentRecord.ProcessId), record => (record.ProcessId != currentRecord.ProcessId)),
                    new FilterOption(String.Format("ModuleName is not {0}",  currentRecord.ModuleName), record => (record.ModuleName != currentRecord.ModuleName))
                };
            }
        }             

        #endregion     
    }
}
