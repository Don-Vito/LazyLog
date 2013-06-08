using LazyLog.LogProviders;
using LazyLog.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
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
        
        private IList<string> _filter;
        public IList<string> Filter
        {
            get
            {
                return _filter;
            }

            private set
            {
                _filter = value;

                if (_filter.Count == 0)
                {
                    FilteredLogRecords.Filter = (item) => true;
                }
                else
                {
                    Delegate predicate = CreateNotTrivialFilter();
                    FilteredLogRecords.Filter = (item) => (bool)predicate.DynamicInvoke(item);
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
                if (_filter.Count == 0)
                {
                    return "Unfiltered";
                }
                return GetLambdaString();
            }
        }

        #endregion


        public LogViewModel(ObservableCollection<LogRecord> logRecords) : this(logRecords, new List<string>())
        {                       
        }

        public LogViewModel(ObservableCollection<LogRecord> logRecords, IList<string> filter)          
        {
            FilteredLogRecords = CollectionViewSource.GetDefaultView(logRecords);
            Filter = filter;
            _filterCommand = new RelayCommand((p) => { RunFilter(p); }, (p) => CanFilter(p));
        }

        #region FilterMenu
        private bool CanFilter(object p)
        {
            return SelectedItem != null;
        }

        private void RunFilter(object p)
        {
            _filter.Add(p as string);
            Filter = _filter;
        }

        public IList<string> FilterOptions
        {            
            get
            {
                var currentRecord = SelectedItem as LogRecord;
                if (currentRecord == null)
                {
                    return null;
                }

                return new string[] 
                { 
                    String.Format("ThreadId==\"{0}\"",  currentRecord.ThreadId),
                    String.Format("ProcessId==\"{0}\"",  currentRecord.ProcessId),
                    String.Format("ModuleName==\"{0}\"",  currentRecord.ModuleName),
                };
            }
        }     

        #endregion

        private string GetLambdaString()
        {
            return String.Join(" AND ", _filter);
        }

        private Delegate CreateNotTrivialFilter()
        {
            ParameterExpression p = Expression.Parameter(typeof(LogRecord), "LogRecord");
            LambdaExpression e = System.Linq.Dynamic.DynamicExpression.ParseLambda<LogRecord, bool>(GetLambdaString(), new[] { p });
            Delegate predicate = e.Compile();
            return predicate;
        }       
    }
}
