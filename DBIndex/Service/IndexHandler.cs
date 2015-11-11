using System.Collections;
using System.Collections.Generic;
using System;
using DatabaseIndex.Entities;

namespace DatabaseIndex.Service
{
    internal class IndexHandler<TIndex, TRow> : IEnumerable
        where TIndex : IComparable
    {
        private Dictionary<string, Index<TIndex, TRow>> _indexCollection;
        static IEnumerable<TRow> _tableData;

        public IndexHandler(IEnumerable<TRow> data)
        {
            _tableData = data;
            _indexCollection = new Dictionary<string, Index<TIndex, TRow>>();
            IndexEvent.IndexEventHandler += OnRowDeleted;
            IndexEvent.IndexEventHandler += OnRowAdded;
        }

        /// <summary>
        /// Create a new Index
        /// </summary>
        /// <param name="key">Key of the new index</param>
        /// <param name="getPropertyFunc">Func delegate to extract the index property</param>
        public void CreateIndex(string key, Func<TRow, TIndex> getPropertyFunc)
        {
            _indexCollection.Add(key, new Index<TIndex, TRow>(getPropertyFunc, _tableData));
        }

        /// <summary>
        /// Drop an index
        /// </summary>
        /// <param name="key">key of the index</param>
        /// <returns>true if index droped</returns>
        public bool DropIndex(string key)
        {
            return _indexCollection.Remove(key);
        }

        /// <summary>
        /// Check if an index exists for a given key
        /// </summary>
        /// <param name="key">Key of the index</param>
        /// <returns>true if exists</returns>
        public bool ContainsKey(string key)
        {
            return _indexCollection.ContainsKey(key);
        }

        /// <summary>
        /// Retrieve a collection of rows with the given index value
        /// </summary>
        /// <param name="key">the index key to retrieve from</param>
        /// <param name="value">value to search</param>
        /// <returns>a collection of rows</returns>
        public ICollection<TRow> Retrieve(string key, TIndex value)
        {
            return Retrieve(key, value, value);
        }

        /// <summary>
        /// Retrieve a collection of rows with the given index value range
        /// </summary>
        /// <param name="key">the index key to retrieve from</param>
        /// <param name="from">min value</param>
        /// <param name="to">max value</param>
        /// <returns>a collection of rows</returns>
        public ICollection<TRow> Retrieve(string key, TIndex from, TIndex to)
        {
            if (ContainsKey(key))
            {
                return _indexCollection[key].Retrieve(from, to);
            }

            throw new KeyNotFoundException("An index with the given key do not exists");
        }


        //Delete a row from the Index when the IndexEventHandler is fired up
        private void OnRowDeleted(object sender, IndexEventArgs e)
        {
            if (e.State == DataState.Remove)
            {
                TRow row = (TRow)e.Row;
                foreach (var key in _indexCollection.Keys)
                {
                    _indexCollection[key].RemoveRowFromIndex(row);
                }
            }
        }

        //Add a row to the Index when the IndexEventHandler is fired up
        private void OnRowAdded(object sender, IndexEventArgs e)
        {
            if (e.State == DataState.Add)
            {
                TRow row = (TRow)e.Row;

                foreach (var key in _indexCollection.Keys)
                {
                    _indexCollection[key].AddIndexNode(row);
                }
            }
        }

        //Gives the ability to enumerate over the index keys
        public IEnumerator GetEnumerator()
        {
            return _indexCollection.Keys.GetEnumerator();
        }

        public IEnumerable Values()
        {
            foreach (var item in _indexCollection.Values)
            {
                yield return item;
            }
        }
    }
}