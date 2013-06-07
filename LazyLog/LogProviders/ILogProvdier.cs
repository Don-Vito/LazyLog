using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LazyLog.LogProviders
{
    public delegate void NewDataHandler(string newData);

    public interface ILogProvdier
    {
        event NewDataHandler OnNewData;

        string Description { get; }

        void Start();

        void Stop();
    }
}
