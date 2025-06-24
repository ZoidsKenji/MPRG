using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class PathNode
    {
        public int x;
        public int y;
        public bool drivible = true;
        public float dCost = 0;
        public float hCost = 0;
        public float tCost = 0;
        public PathNode parent = null;
        public PathNode(int x, int y, bool drivible)
        {
            this.x = x;
            this.y = y;
            this.drivible = drivible;
        }

        public bool LargerThan(PathNode other)
        {
            return this.tCost < other.tCost;
        }

        public bool Equal(PathNode other)
        {
            return x == other.x && y == other.y;
        }

        public HashSet<int> hash(){
            return new HashSet<int>() {x, y};
        }
    }
}