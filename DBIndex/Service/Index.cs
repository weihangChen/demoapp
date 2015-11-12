using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using BinaryTree;
using AlgorithmStub;


namespace DatabaseIndex.Service
{
    internal class Index<TIndex, TRow>
        where TIndex : IComparable
    {
        //private BinaryTree<IndexNode<TIndex, TRow>> _tree;
        private IDbIndexAlgorithm<IndexNode<TIndex, TRow>> _tree;
        internal Func<TRow, TIndex> PropertyFunc;

        public Index(Func<TRow, TIndex> getPropertyFunc, IEnumerable<TRow> tableData)
        {


            PropertyFunc = getPropertyFunc;
            _tree = new BinaryTree<IndexNode<TIndex, TRow>>();
            InitIndex(tableData);

        }

        //Initilizing the index by providing the table data to be able to save a reference to the rows
        //and also providing a Function that determine which property will be indexed.
        private void InitIndex(IEnumerable<TRow> tableData)
        {
            //TIndex - Field like Name, Age    TRow - Entity like Person

            var groups = tableData.GroupBy(row => PropertyFunc(row));
            List<IndexNode<TIndex, TRow>> nodes = new List<IndexNode<TIndex, TRow>>();

            foreach (var group in groups)
            {
                IndexNode<TIndex, TRow> node = new IndexNode<TIndex, TRow>(PropertyFunc(group.First()));

                foreach (var row in group)
                {
                    node.AddRow(row);
                }

                nodes.Add(node);
            }

            _tree.Add(nodes);

        }

        //Retrieve a collection of rows by range.
        internal ICollection<TRow> Retrieve(TIndex from, TIndex to)
        {
            ICollection<IndexNode<TIndex, TRow>> indexNodes = FindNodes(from, to);

            //ICollection<TRow> rowsCollection = new Collection<TRow>();
            //foreach (var indexNode in indexNodes)
            //{
            //    foreach (var row in indexNode.Rows.Values)
            //    {
            //        rowsCollection.Add(row);
            //    }
            //}
            var rowsCollection = indexNodes.SelectMany(x => x.Rows.Values).ToList();
            return rowsCollection;
        }

        //Remove a node when a row was deleted from the table
        internal bool RemoveRowFromIndex(TRow row)
        {
            ICollection<IndexNode<TIndex, TRow>> indexNodes = FindNodes(PropertyFunc(row), PropertyFunc(row));

            foreach (var indexNode in indexNodes)
            {
                if (indexNode.RemoveRow(row))
                {
                    //if node is now empty, remove it
                    if (indexNode.Count == 0)
                    {
                        _tree.Remove(indexNode);
                    }

                    return true;
                }
            }

            return false;
        }

        //Add a node when a row was added to the table
        internal void AddIndexNode(TRow row)
        {
            ICollection<IndexNode<TIndex, TRow>> indexNodes = FindNodes(PropertyFunc(row), PropertyFunc(row));

            //An indexnode is missing for the property value, need to add new node for that value
            if (indexNodes.Count == 0)
            {
                IndexNode<TIndex, TRow> nodeToAdd = new IndexNode<TIndex, TRow>(PropertyFunc(row));
                nodeToAdd.AddRow(row);
                _tree.Add(nodeToAdd);
            }

            else
            {
                foreach (var indexNode in indexNodes)
                {
                    indexNode.AddRow(row);
                }
            }
        }

        //return indexnodes from a given range
        private ICollection<IndexNode<TIndex, TRow>> FindNodes(TIndex from, TIndex to)
        {
            IndexNode<TIndex, TRow> fromNode = new IndexNode<TIndex, TRow>(from);
            IndexNode<TIndex, TRow> toNode = new IndexNode<TIndex, TRow>(to);

            return _tree.Contains(fromNode, toNode).ToList();
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var indexNode in _tree)
                {
                    count += indexNode.Count;
                }

                return count;
            }
        }
    }
}
