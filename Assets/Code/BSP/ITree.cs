using System;
using System.Collections.Generic;

namespace BSP.Assets.Code.BSP
{
    public interface ITree<T>
    {
        INode<T> Root {get;}
        void GetLeafs(ref List<INode<T>> leafList, INode<T> root);
        INode<T> CreateNode(INode<T> parent, T data, string id);
        INode<T> Search(INode<T> node, float height);
        void SetRoot(INode<T> root);
        void Print();

        void Insert(T data, float height);
    }
}
