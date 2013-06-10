using System;
using System.IO;
using System.Text;
using System.Threading;


namespace LazyLog.LogProviders
{
    public class FileLogProvider : ILogProvdier
    {   
        private readonly string _filePath;
        private long _offset;      
        private Timer _timer;


        #region ILogProvider    
   
        public event NewDataHandler OnNewData;

        public string Description
        {
            get
            {
                return "File: " + _filePath;
            }
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _timer = new Timer(PollingTask, null, 0, Timeout.Infinite);    
            }
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion


        public FileLogProvider(string filePath)
        {
            _filePath = filePath;   
            _offset = 0;
        }

        private void PollingTask(object state)
        {
            string newData = ReadFileLines();
            if (!String.IsNullOrEmpty(newData))
            {
                FireNewData(newData);
            }
            _timer.Change(1000, Timeout.Infinite);
        }

        private void FireNewData(string newData)
        {
            if (OnNewData != null)
            {
                OnNewData(newData);
            }
        }

        private string ReadFileLines()
        { 
            if (!File.Exists(_filePath))
            {
                throw new LogProviderException();
            }

            try
            {            
                using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fs.Length < _offset)
	                {
                        // The records were removed from file
                        // TODO: we need to decide how to handle this case
                        throw new LogProviderException();
	                }  
                    
                    fs.Position = _offset;                
              
                    using (var reader = new StreamReader(fs, Encoding.UTF8))
                    {
                        string newData = reader.ReadToEnd();
                        _offset = fs.Position;
                        return newData;
                    }                
                }
            }
            catch (IOException)
            {
                // If the file is inaccessible we'll try later
                return null;
            }
        }

        public bool IsRunning
        {
            get 
            {
                return _timer != null;
            }
        }
    }
}
