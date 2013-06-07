using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace LazyLog.Framework
{
    class SafeObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _currentDispatcher;

        public SafeObservableCollection() : base()
        {            
            _currentDispatcher = Dispatcher.CurrentDispatcher;
        }        
 
        private void DoDispatchedAction(Action action)
        {
            if (_currentDispatcher.CheckAccess())
            {
                action.Invoke();
            }
            else
            {
                _currentDispatcher.Invoke(DispatcherPriority.DataBind, action);
            }
        }
 
        protected override void ClearItems()
        {
            DoDispatchedAction(delegate { base.ClearItems(); });
        }
  
        protected override void InsertItem(int index, T item)
        {
            DoDispatchedAction(delegate { base.InsertItem(index, item); });
        }
  
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            DoDispatchedAction(delegate { base.MoveItem(oldIndex, newIndex); });
        }
       
        /// <summary> 
        /// Removes the item at the specified index 
        /// </summary> 
        ///<param name="index">The index of the item which should be removed</param> 
        protected override void RemoveItem(int index)
        {
            DoDispatchedAction(delegate { base.RemoveItem(index); });
        }
    
        protected override void SetItem(int index, T item)
        {
            DoDispatchedAction(delegate { base.SetItem(index, item); });
        }
 
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DoDispatchedAction(delegate { base.OnCollectionChanged(e); });
        }
 
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            DoDispatchedAction(delegate { base.OnPropertyChanged(e); });
        }       
    }
}
