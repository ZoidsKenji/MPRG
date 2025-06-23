using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class PathNode
    {
        public int x;
        public int y;
        public bool drivible = true;
        public int dCost = 0;
        public int hCost = 0;
        public int tCost = 0;
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
    }
}