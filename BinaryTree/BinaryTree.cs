using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AlgorithmStub;

namespace BinaryTree
{
    public sealed class BinaryTree<T> : ICollection<T>, IDbIndexAlgorithm<T>
        where T : IComparable
    {

        private BinaryTreeNode<T> _rootNode;
        private BinaryTreeNode<T> _searchResult;
        private ITreeBalancer<T> _balancer;
        public int Count { get; private set; }

        public BinaryTree()
        {
            _balancer = new AvlTreeBalance<T>();
            Count = 0;
        }

        #region Add
        public void Add(BinaryTreeNode<T> nodeToAdd)
        {
            Count++;

            if (_rootNode == null)
            {
                _rootNode = new BinaryTreeNode<T>(nodeToAdd.Value);
                return;
            }

            AddNode(_rootNode, nodeToAdd);
        }

        public void Add(IList<T> dataCollection)
        {
            IList<T> tmpList = dataCollection.OrderBy(d => d).ToList();
            Add(tmpList, 0, tmpList.Count - 1);
        }

        private void Add(IList<T> collection, int min, int max)
        {
            int mid = (min + max) / 2;

            if (max == min)
            {
                Add(collection[mid]);
                return;
            }
            if (max - min == 1)
            {
                Add(collection[min]);
                Add(collection[max]);
                return;
            }

            Add(collection[mid]);

            Add(collection, min, mid - 1);
            Add(collection, mid + 1, max);
        }

        private void AddNode(BinaryTreeNode<T> addTo, BinaryTreeNode<T> nodeToAdd)
        {
            if (addTo.CompareTo(nodeToAdd) > 0)
            {
                if (addTo.LeftNode == null)
                {
                    addTo.LeftNode = nodeToAdd;
                    nodeToAdd.ParentNode = addTo;
                    TraversalBack(nodeToAdd);
                    return;
                }
                AddNode(addTo.LeftNode, nodeToAdd);
                return;
            }

            if (addTo.CompareTo(nodeToAdd) <= 0)
            {
                if (addTo.RightNode == null)
                {
                    addTo.RightNode = nodeToAdd;
                    nodeToAdd.ParentNode = addTo;
                    TraversalBack(nodeToAdd);
                    return;
                }
                AddNode(addTo.RightNode, nodeToAdd);
            }
        }
        #endregion

        #region Search
        private void SearchNode(BinaryTreeNode<T> searchIn, BinaryTreeNode<T> searchFor)
        {
            if (searchIn == null)
            {
                return;
            }

            //Node found
            if (searchIn.CompareTo(searchFor) == 0)
            {
                _searchResult = searchIn;
                return;
            }

            SearchNode(searchIn.LeftNode, searchFor);
            SearchNode(searchIn.RightNode, searchFor);
        }

        /// <summary>
        /// search for a range of values
        /// </summary>
        /// <param name="from">from value</param>
        /// <param name="to">to value</param>
        /// <returns>ICollection of the found values</returns>
        public ICollection<T> Contains(T from, T to)
        {
            ICollection<T> list = new Collection<T>();
            InOrderRangeSearch(ref list, _rootNode, from, to);
            return list;
        }

        /// <summary>
        /// Search for range using a In-Order traversal
        /// For every node passed, the value will be compared with 'to' and 'from' values
        /// and will act according to the result.
        /// Cost is O(n)
        /// </summary>
        /// <param name="list">ICollection to store found values</param>
        /// <param name="node">node to travel</param>
        /// <param name="from">from value</param>
        /// <param name="to">to value</param>
        private void InOrderRangeSearch(ref ICollection<T> list, BinaryTreeNode<T> node, T from, T to)
        {
            if (node == null)
            {
                return;
            }

            InOrderRangeSearch(ref list, node.LeftNode, from, to);

            //store the current value if its inside the given range
            if (node.Value.CompareTo(from) >= 0 && node.Value.CompareTo(to) <= 0)
            {
                list.Add(node.Value);
            }

            InOrderRangeSearch(ref list, node.RightNode, from, to);
        }
        #endregion

        #region Remove

        /// <summary>
        /// Remove a node with left and right childrens
        /// </summary>
        /// <param name="nodeToRemove">Node to remove</param>
        private void RemoveTwoChildrensBranch(BinaryTreeNode<T> nodeToRemove)
        {
            //To find the successor we need to go one step to the right (right child)
            //And then to the left most child of that right child
            var successor = GetSuccessor(nodeToRemove.RightNode);

            //Saving the successor value
            T tmpValue = successor.Value;
            //Removing the successor from the tree (The balance will occur inside this step)
            CheckNumberOfChildsToRemove(successor);
            //Replacing the nodeToRemove's value to the successor's
            nodeToRemove.Value = tmpValue;
        }

        /// <summary>
        /// Find the successor by traveling to the elft most node
        /// </summary>
        /// <param name="node">The node to travel from</param>
        /// <returns>The successor</returns>
        private BinaryTreeNode<T> GetSuccessor(BinaryTreeNode<T> node)
        {
            var successor = node;

            //Finding the left most child
            while (successor.LeftNode != null)
            {
                successor = successor.LeftNode;
            }

            return successor;
        }

        /// <summary>
        /// Remove a node with one child
        /// </summary>
        /// <param name="nodeToRemove">Node to remove</param>
        private void RemoveOneChildBranch(BinaryTreeNode<T> nodeToRemove)
        {
            BinaryTreeNode<T> parent = nodeToRemove.ParentNode;

            //Check is the nodeToRemove is the left child
            if (parent.LeftNode == nodeToRemove)
            {
                if (nodeToRemove.LeftNode == null)
                {
                    parent.LeftNode = nodeToRemove.RightNode;
                    return;
                }

                parent.LeftNode = nodeToRemove.LeftNode;
            }


            else //nodeToRemove is the right child
            {
                if (nodeToRemove.LeftNode == null)
                {
                    parent.RightNode = nodeToRemove.RightNode;
                    return;
                }

                parent.RightNode = nodeToRemove.LeftNode;
            }

            _balancer.Balance(ref parent);
        }

        /// <summary>
        /// Remove node with no childrens
        /// </summary>
        /// <param name="parent"></param>
        private void RemoveLeaf(BinaryTreeNode<T> parent)
        {
            if (parent.RightNode == parent)
                parent.RightNode = null;
            else parent.LeftNode = null;

            _balancer.Balance(ref parent);
        }
        #endregion

        #region IEnumerable members

        /// <summary>
        /// Enumerate in a In-Order traversal
        /// </summary>
        /// <returns>IEnumerator which implement the In-Order Traversal</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return InOrderEnumerator(_rootNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator<T> InOrderEnumerator(BinaryTreeNode<T> node)
        {
            bool goLeft = true; //flag for traversing left \ right
            BinaryTreeNode<T> current = node;
            Stack<BinaryTreeNode<T>> stack = new Stack<BinaryTreeNode<T>>(); //a stack to hold the visited nodes

            stack.Push(current); //start by pushing the root to the stack

            while (stack.Count > 0) //stack.Count == 0 means we ran out of nodes to visit
            {
                if (goLeft) //going left child
                {
                    while (current.LeftNode != null) //reaching the most left child of the current node
                    {
                        stack.Push(current); //pushing visited nodes
                        current = current.LeftNode;
                    }
                }

                yield return current.Value;  //returning the current node (which all of his left subtree was visited)

                if (current.RightNode != null) //check if current node have right child, if not then both his left and right subtrees was visited
                {
                    current = current.RightNode; //going one step to the right
                    goLeft = true; //now visit the new current left subtree
                }

                else //if no right child
                {
                    current = stack.Pop(); //pop the parent node (which the method already visited all of his left subtree)
                    goLeft = false; //next loop go to his right subtree
                }

            }

        }
        #endregion

        #region ICollection members

        /// <summary>
        /// Add a new node to the tree.
        /// Cost O(log n)
        /// </summary>
        /// <param name="data"></param>
        public void Add(T data)
        {
            Add(new BinaryTreeNode<T>(data));
        }

        /// <summary>
        /// Remove a node. 
        /// Cost O(log n)
        /// </summary>
        /// <param name="data">Node's value</param>
        /// <returns>True if found and removed, false if not.</returns>
        public bool Remove(T data)
        {
            if (!Contains(data))
                return false; //node not found

            var nodeToRemove = _searchResult;

            CheckNumberOfChildsToRemove(nodeToRemove);

            Count--;

            return true;
        }

        /// <summary>
        /// Check if the tree contains a given value
        /// </summary>
        /// <param name="data">the value to search for</param>
        /// <returns>true if the node is found else false</returns>
        public bool Contains(T data)
        {
            _searchResult = null;
            SearchNode(_rootNode, new BinaryTreeNode<T>(data));
            return _searchResult != null;
        }

        /// <summary>
        /// Clear the tree
        /// </summary>
        public void Clear()
        {
            _rootNode = null;
        }

        /// <summary>
        /// Copy the entire collection to a given array
        /// </summary>
        /// <param name="array">the array to copy to</param>
        /// <param name="arrayIndex">the index to start the copy at</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            try
            {
                foreach (T value in this)
                {
                    array[arrayIndex++] = value;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(e.Message);
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        public override string ToString()
        {
            string result = string.Empty;
            foreach (T value in this)
            {
                result += string.Format("{0}, ", value.ToString());
            }

            return result;
        }

        /// <summary>
        /// traverse upwards from a given node
        /// </summary>
        /// <param name="node">node to start at</param>
        private void TraversalBack(BinaryTreeNode<T> node)
        {
            bool balanced = false; //Flag to stop traversing once a node has been balanced

            while (!balanced & node != null)
            {
                //Checking the balance factor of the node
                if (node.BalanceFactor < -1 | node.BalanceFactor > 1)
                {
                    _balancer.Balance(ref node); //balance the node
                    if (node.ParentNode == null) //if the node the root, set it as the new root
                    {
                        _rootNode = node;
                    }
                    balanced = true;
                }
                if (!balanced)
                {
                    node = node.ParentNode;
                }
            }

        }


        //Check how many childs a node have
        private void CheckNumberOfChildsToRemove(BinaryTreeNode<T> nodeToRemove)
        {

            //ToDo: remove the 'Remove' methods from this one. and just have this one returns an int of the childrens number

            //No childs
            if (nodeToRemove.LeftNode == null && nodeToRemove.RightNode == null)
                RemoveLeaf(nodeToRemove.ParentNode);

            //One child
            if (nodeToRemove.LeftNode == null || nodeToRemove.RightNode == null)
                RemoveOneChildBranch(nodeToRemove);

            //Two childs
            else RemoveTwoChildrensBranch(nodeToRemove);
        }

    }
}

