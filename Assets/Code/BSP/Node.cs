using System;

namespace BSP.Assets.Code.BSP
{
    public class Node<T> : INode<T>
    {
        public string Id => _id;

        public INode<T> Parent => _parent;

        public INode<T> Left 
        { 
            get =>_left; 
            set =>_left = value;
        }

        public INode<T> Right  { 
            get =>_right; 
            set =>_right = value;
        }

        public T Data => _data;

        public virtual float Heigth => _heigth;

        protected string _id;
        protected INode<T> _parent;
        protected INode<T> _left; 
        protected INode<T> _right;
        protected T _data;
        protected float _heigth;

        public Node(INode<T> parent, T data, string id){
            _parent = parent;
            _data = data;
            _id = id;
        }

        public void SetChild(INode<T> left,INode<T> right){
            _left = left;
            _right = right;
        }

        public void SetHeight(float height){
            _heigth = height;
        }
    }
}
