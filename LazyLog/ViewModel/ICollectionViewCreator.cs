using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LazyLog.ViewModel
{
    public interface ICollectionViewCreator
    {
        ICollectionView CreateView<T>(IEnumerable<T> source);
    }
}
