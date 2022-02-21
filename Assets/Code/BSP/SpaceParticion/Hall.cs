using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code.BSP.SpaceParticion
{
    public class Hall
    {
        public List<RectInt> Halls => _halls;
        public Guid Id => _id;

        private Guid _id;
        private List<RectInt> _halls;

        public Hall()
        {
            _halls = new List<RectInt>();
            _id = Guid.NewGuid();
        }

        public void Add(RectInt hall)
        {
            _halls.Add(hall);
        }
    }
}
