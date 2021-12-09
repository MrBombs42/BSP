using System;
using BSP.Assets.Code.BSP;

namespace BSP.Assets.Code.InterTreeTest
{
    public class IntegerNode : Node<int>
    {
        public IntegerNode(INode<int> parent, int data, string id) : base(parent, data, id)
        {
        }

        public override float Heigth => (float)_data;
    }
}
