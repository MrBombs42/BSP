using System;
using UnityEngine;

namespace BSP.Assets.Code.BSP.SpaceParticion
{
    public enum SplitDirection{
        None = 0,
        Horizontal, 
        Vertical
    }

    public class SpaceParticionData{
        public RectInt Container;
        public RectInt Room;
        public SplitDirection SplitDirection;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Container: {Container}, Room: {Room}, SplitDirection: {SplitDirection}";
        }
    }

    public class SpaceParticionNode : Node<SpaceParticionData>
    {
        public SpaceParticionNode(INode<SpaceParticionData> parent, SpaceParticionData data, string id) : base(parent, data, id)
        {
        }

        public override float Heigth => _data.Container.width * _data.Container.height;//rectangle area
    }
}
