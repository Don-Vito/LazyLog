using System.Collections.Generic;
using System.ComponentModel;

namespace LazyLog.ViewModel
{
    public interface ICollectionViewCreator
    {
        ICollectionView CreateView<T>(IEnumerable<T> source);
    }
}
