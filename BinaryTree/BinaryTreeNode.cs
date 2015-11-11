
using System;


namespace BinaryTree
{
    public class BinaryTreeNode<T> : IComparable
        where T : IComparable
    {
        #region Proprties
        public T Value { get; internal set; }
        public BinaryTreeNode<T> LeftNode { get; internal set; }
        public BinaryTreeNode<T> RightNode { get; internal set; }
        public BinaryTreeNode<T> ParentNode { get; internal set; }


        //ctor
        public BinaryTreeNode(T data)
        {
            Value = data;
        }

        public int BalanceFactor
        {
            get
            {
                return GetHeight(LeftNode) - GetHeight(RightNode);
            }
        }

        public int LeftHeight
        {
            get
            {
                return GetHeight(LeftNode);
            }
        }

        public int RightHeight
        {
            get
            {
                return GetHeight(RightNode);
            }
        }
        #endregion

        public override string ToString()
        {
            return Value.ToString();
        }

        public int CompareTo(object obj)
        {
            var tmp = obj as BinaryTreeNode<T>;

            if (tmp == null)
                throw new InvalidCastException();

            return Value.CompareTo(tmp.Value);

        }

        private int GetHeight(BinaryTreeNode<T> node)
        {
            if (node == null)
            {
                return 0;
            }

            return 1 + Math.Max(GetHeight(node.LeftNode), GetHeight(node.RightNode));
        }
    }
}
