using System;

namespace BinaryTree
{
    public class AvlTreeBalance<T> : ITreeBalancer<T>
        where T : IComparable
    {
        /// <summary>
        /// Blanace the node with a cost of O(1)
        /// </summary>
        /// <param name="node">node to balance</param>
        /// <returns>balanced node</returns>
        public void Balance(ref BinaryTreeNode<T> node)
        {
            //Node is right heavy
            if (node.BalanceFactor < -1)
            {
                //If right subtree if left heavy
                //it needs to be rotated to the right first
                if (node.RightNode.BalanceFactor > 0)
                {
                    BinaryTreeNode<T> rightChild = node.RightNode;
                    RightRotation(ref rightChild);
                }

                LeftRotation(ref node);
            }

            //Node is left heavy
            else if (node.BalanceFactor > 1)
            {
                //If left subtree is right heavy
                //it needs to be rotated to the left first
                if (node.LeftNode.BalanceFactor < 0)
                {
                    BinaryTreeNode<T> leftChild = node.LeftNode;
                    LeftRotation(ref leftChild);
                }

                RightRotation(ref node);
            }


            //return node;
        }

        private void RightRotation(ref BinaryTreeNode<T> node)
        {
            // current sub tree state:
            //          6
            //         /  
            //        4
            //       / \
            //      2   null
            //
            // state after right rotation:
            //          4 (newRoot)
            //         /  \
            //        2    6 (oldRoot)
            //            /
            //          null

            BinaryTreeNode<T> newRoot = node.LeftNode; //left child become the new root
            BinaryTreeNode<T> parent = node.ParentNode; //saving a reference to the old root's parent
            BinaryTreeNode<T> oldRoot = node; //saving the old root
            newRoot.ParentNode = node.ParentNode; //setting the parent as the new root's parent

            if (parent != null) //if parent is not the tree's root
            {
                //checking which of the parent's childs is the oldRoot
                if (node.ParentNode.LeftNode == node)
                {
                    parent.LeftNode = newRoot;
                }
                else
                {
                    parent.RightNode = newRoot;
                }
            }

            oldRoot.LeftNode = newRoot.RightNode;

            if (oldRoot.LeftNode != null)
            {
                oldRoot.LeftNode.ParentNode = oldRoot;
            }

            newRoot.RightNode = oldRoot;
            oldRoot.ParentNode = newRoot;

            node = newRoot;
        }

        private void LeftRotation(ref BinaryTreeNode<T> node)
        {
            // current sub tree state:
            //          2
            //           \  
            //            4
            //           / \
            //        null  6 
            //
            // state after right rotation:
            //              4 (newRoot)
            //            /   \
            // (oldRoot) 2     6 
            //            \
            //            null


            BinaryTreeNode<T> newRoot = node.RightNode;
            BinaryTreeNode<T> parent = node.ParentNode;
            BinaryTreeNode<T> oldRoot = node;
            newRoot.ParentNode = node.ParentNode;

            if (parent != null)
            {
                if (node.ParentNode.LeftNode == node)
                {
                    parent.LeftNode = newRoot;
                }
                else
                {
                    parent.RightNode = newRoot;
                }
            }

            oldRoot.RightNode = newRoot.LeftNode;

            if (oldRoot.RightNode != null)
            {
                oldRoot.RightNode.ParentNode = oldRoot;
            }

            newRoot.LeftNode = oldRoot;
            oldRoot.ParentNode = newRoot;

            node = newRoot;
        }
    }
}
