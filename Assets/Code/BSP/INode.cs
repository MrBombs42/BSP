using System;

namespace BSP.Assets.Code.BSP
{
    public interface INode<T>
    {
        string Id{get;}
        INode<T> Parent{get;}
        INode<T> Left{get;set;}
        INode<T> Right{get;set;}
        T Data{get;}
        float Heigth{get;}
        void SetChild(INode<T> left,INode<T> right);
    }
}
