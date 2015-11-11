using System;

namespace DatabaseIndex.Entities
{
    public class IndexEventArgs : EventArgs
    {
        public DataState State;
        public object Row;
    }
}
