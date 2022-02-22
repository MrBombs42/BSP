using System;
using System.Collections.Generic;
using System.Text;

namespace BSP.Assets.Code.BSP
{
    public class Tree<T> : ITree<T>
    {
        public INode<T> Root => _root;

        private INode<T> _root;

        public Tree(){
            _root = null;
        }

        public void SetRoot(INode<T> root){
            _root = root;
        }

        public virtual INode<T> CreateNode(INode<T> parent, T data, string id){
            var node = new Node<T>(parent, data, id);
            return node;
        }


        /// <summary>
        /// Get nodes with no childrens
        /// </summary>
        /// <param name="leafList"></param>
        /// <param name="root"></param>
        public void GetLeafs(ref List<INode<T>> leafList, INode<T> root){
            if(root.Left == null && root.Right == null){
                leafList.Add(root);
                return;
            }

            GetLeafs(ref leafList, root.Left);
            GetLeafs(ref leafList, root.Right);
        }

        public void GetNodesAtLevel(ref HashSet<INode<T>> nodesList, INode<T> root, int level)
        {
            if (level == 0)
            {
                nodesList.Add(root);
                return;
            }

            int startLevelLeft = level;
            GetNodesAtLevel(ref nodesList, root.Left, --startLevelLeft);
            int startLevelRight = level;
            GetNodesAtLevel(ref nodesList, root.Right, --startLevelRight);
        }

        public int GetTreeSize(INode<T> root)
        {
            if (root.Left == null && root.Right == null)
            {
                return 0;
            }

            return 1 + GetTreeSize(root.Left);
        }

        public INode<T> Search(INode<T> node, float height)
        {
            if(node == null){
                //UnityEngine.Debug.Log("Null");
                return node;
            }

            if(node.Heigth == height){
                //UnityEngine.Debug.Log("igual");
                return node;
            }

            if(height < node.Heigth){
                //UnityEngine.Debug.Log($"Left {node.Id}");
                return Search(node.Left, height);
            }

            return Search(node.Right, height);//height > node.Height
        }

        public void Insert(T data, float height){
            _root = InsertRec(_root, null, data, height);
        }

        private INode<T> InsertRec(INode<T> node, INode<T> parent, T data, float height){
            if(node == null){
                node = CreateNode(parent, data, "");
                return node;
            }

            if(height < node.Heigth){
                node.Left = InsertRec(node.Left, node, data, height);
            }
            if(height > node.Heigth){
                node.Right = InsertRec(node.Right, node, data, height);
            }

            return node;
        }

        private void InOrderRec(INode<T> root){
            if(root != null){
                InOrderRec(root.Left);
                UnityEngine.Debug.Log($"Id:{root.Id}  Data:{root.Data} Height {root.Heigth}");
                InOrderRec(root.Right);
            }
        }



        private INode<T> NavigateToRight(INode<T> node){
            return node.Right;
        }

        private INode<T> NavigateToLeft(INode<T> node){
            return node.Left;
        }


        public void Print()
        {
            InOrderRec(_root);
            // if(_root == null){
            //     return "Null";
            // }

            // var stringBuilder = new StringBuilder();
            // var node = NavigateToRight(_root);
            // while(node != null){
            //     node = NavigateToRight(node);
            //     var childRigth = node.Right != null ? node.Right.Id:"null";
            //     node = NavigateToLeft(node);
            //     var childLeft = node.Left != null ? node.Left.Id:"null";
            //     stringBuilder.AppendLine($"{node.Parent.Id} ({childRigth}, {childLeft})");
            // }

            // return stringBuilder.ToString();
        }
    }
}
