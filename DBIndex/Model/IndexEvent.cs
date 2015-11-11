using System;

namespace DatabaseIndex.Entities
{
    public static class IndexEvent
    {
        //Event to handle rows bein added or deleted
        public static EventHandler<IndexEventArgs> IndexEventHandler;
    }
}
