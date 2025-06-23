using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class PathFind
    {
        public List<int> map;
        public int height;
        public int width;

        public List<> nodes;

        public PathFind(List<int> map)
        {
            this.map = map;
            height = map.Count;
            width = map[0].Count;
            nodes = new List<PathNode>();
        }
    }
}