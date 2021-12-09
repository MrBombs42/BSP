using System;
using BSP.Assets.Code.BSP;

namespace BSP.Assets.Code.InterTreeTest
{
    public class IntegerTree : Tree<int>
    {
        public override INode<int> CreateNode(INode<int> parent, int data, string id)
        {
            var node = new IntegerNode(parent, data, id);
            InsertLeaf(node);
            return node;
        }
    }
}
