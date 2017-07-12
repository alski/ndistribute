using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace nDistribute.Data.Tests
{
    internal class DataSync<T>
    {
        private ICollection<T> data;

        public DataSync(ObservableCollection<T> data)
        {
            this.data = data;
        }

        public Guid CurrentVersion => Guid.Empty;
    }
}