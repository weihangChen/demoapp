using System;
using System.Collections.Generic;

namespace DatabaseIndex.Service
{
    internal class IndexNode<TIndex, TRow> : IComparable
        where TIndex : IComparable
    {
        public TIndex IndexBy { get; private set; } //the property value
        public Dictionary<string, TRow> Rows { get; private set; } //reference to rows in the table
        private int _count;


        public IndexNode(TIndex indexBy)
        {
            _count = 0;
            IndexBy = indexBy;
            Rows = new Dictionary<string, TRow>();
        }

        public int CompareTo(object obj)
        {
            IndexNode<TIndex, TRow> tmp = (IndexNode<TIndex, TRow>)obj;

            if (tmp == null)
            {
                throw new InvalidCastException(string.Format("Object is type: {0} , expected type: {1}", obj.GetType(),
                    this.GetType()));
            }

            Type inner = IndexBy.GetType();
            Type outer = tmp.IndexBy.GetType();

            //object value =  Convert.ChangeType(tmp.IndexBy, typeof (TIndex));

            return IndexBy.CompareTo(tmp.IndexBy);
        }

        /// <summary>
        /// Check if a row exists
        /// </summary>
        /// <param name="row">row to find</param>
        /// <returns>truw if exists</returns>
        public bool Contains(TRow row)
        {
            string rowHash = Hash(row).ToString();
            string rowHash2 = AlternativeHash(row).ToString();

            return Rows.ContainsKey(rowHash) || Rows.ContainsKey(rowHash2);
        }

        /// <summary>
        /// Add a row
        /// </summary>
        /// <param name="row">row to add</param>
        public void AddRow(TRow row)
        {
            string rowHash = Hash(row).ToString();
            try
            {
                Rows.Add(rowHash, row);
                _count++;
            }
            catch (ArgumentException)
            {
                rowHash = AlternativeHash(row).ToString();
                Rows.Add(rowHash, row);
                _count++;
            }
        }

        /// <summary>
        /// Remove a row
        /// </summary>
        /// <param name="row">row to remove</param>
        /// <returns>true if removed</returns>
        public bool RemoveRow(TRow row)
        {
            string rowHash = Hash(row).ToString();
            string rowHash2 = AlternativeHash(row).ToString();

            if (Rows.Remove(rowHash) || Rows.Remove(rowHash2))
            {
                _count--;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return IndexBy.ToString();
        }

        //Return hash code based on the row's properties
        private int Hash(TRow row)
        {
            var properties = row.GetType().GetProperties();
            int hash = 0;

            foreach (var property in properties)
            {
                hash += property.GetValue(row).GetHashCode();
            }

            return hash;
        }

        //Alternative hash method incase 2 rows get the same hash value from the first method
        private int AlternativeHash(TRow row)
        {
            Random rnd = new Random();
            var properties = row.GetType().GetProperties();
            int hash = 0;
            int count = 0;

            foreach (var property in properties)
            {
                if (count < properties.Length - 2)
                {
                    hash += property.GetValue(row).GetHashCode();
                    count++;
                }
            }

            return hash;
        }

        //Return number of rows
        public int Count
        {
            get { return _count; }
        }


    }
}