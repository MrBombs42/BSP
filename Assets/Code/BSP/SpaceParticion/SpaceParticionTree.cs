using System;

namespace BSP.Assets.Code.BSP.SpaceParticion
{
    public class SpaceParticionTree : Tree<SpaceParticionData>
    {
        public override INode<SpaceParticionData> CreateNode(INode<SpaceParticionData> parent, SpaceParticionData data, string id)
        {
            var node = new SpaceParticionNode(parent, data, id);
            InsertLeaf(node);
            return node;
        }
    }
}
