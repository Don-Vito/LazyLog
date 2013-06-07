using LazyLog.LogProviders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyLog.ViewModel
{
    class LogViewModel : ViewModelBase
    {        
        public ObservableCollection<LogRecord> LogRecords { get; private set; }


        #region Title
        
        private string _filter;
        public string Filter
        {
            get
            {
                return _filter;
            }
            private set
            {
                _filter = value;
                RaisePropertyChanged("Filter");
                RaisePropertyChanged("LogViewTitle");
            }
        }

        public string LogViewTitle
        {
            get
            {
                if (String.IsNullOrEmpty(Filter))
                {
                    return "Unfiltered";
                }
                return Filter ;
            }
        }

        #endregion


        public LogViewModel(ObservableCollection<LogRecord> logRecords) : this(logRecords, null)
        {                       
        }

        public LogViewModel(ObservableCollection<LogRecord> logRecords, string filter)          
        {
            LogRecords = logRecords;
            Filter = filter;
        }
    }
}
