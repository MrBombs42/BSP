using System;
using System.Collections.Generic;
using System.Text;

namespace BSP.Assets.Code.BSP
{
    public class Tree<T> : ITree<T>
    {
        public INode<T> Root => _root;
        public List<INode<T>> Leafs => _leafs;

        private INode<T> _root;
        private List<INode<T>> _leafs;

        public Tree(){
            _root = null;
            _leafs = new List<INode<T>>();
        }

        public void SetRoot(INode<T> root){
            _root = root;
        }

        public virtual INode<T> CreateNode(INode<T> parent, T data, string id){
            var node = new Node<T>(parent, data, id);
            InsertLeaf(node);
            return node;
        }

        protected void InsertLeaf(INode<T> node){
            _leafs.Insert(_leafs.Count ,node);
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
                node = CreateNode(parent, data, _leafs.Count.ToString());
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
