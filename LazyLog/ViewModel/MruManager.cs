using LazyLog.Framework;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace LazyLog.ViewModel
{
    class MruManager
    {
        private readonly SafeObservableCollection<string> _recentFiles;
        private readonly int _maxSize;

        public MruManager(SafeObservableCollection<string> recentFiles, int maxSize)
        {            
            _recentFiles = recentFiles;
            _maxSize = maxSize;
            
            Load();
        }

        public void Add(string filePath)
        {
            _recentFiles.Remove(filePath);

            if (_recentFiles.Count == _maxSize)
            {
                _recentFiles.RemoveAt(_recentFiles.Count - 1);
            }

            _recentFiles.Insert(0, filePath);

            Save();
        }

        private void Save()
        {
            Properties.Settings.Default["RecentFiles"] = new StringCollection();
            (Properties.Settings.Default["RecentFiles"] as StringCollection).AddRange(_recentFiles.ToArray());
            Properties.Settings.Default.Save();
        }

        private void Load()
        {
            StringCollection collection = Properties.Settings.Default.RecentFiles;

            if (collection == null)
            {
                return;
            }

            foreach (string filePath in collection)
            {
                if (File.Exists(filePath))
                {
                    _recentFiles.Add(filePath);    
                }                
            }            
        }

        
    }
}
