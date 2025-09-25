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

        // ```
        // Name : LargerThan
        // Parameter : PathNode other
        // Return : --
        // Purpose : check if the other has a larger cost
        // ```
        public bool LargerThan(PathNode other)
        {
            return this.tCost < other.tCost;
        }

        // ```
        // Name : SmallerThan
        // Parameter : PathNode other
        // Return : --
        // Purpose : check if the other has a smaller cost
        // ```
        public bool SmallerThan(PathNode other)
        {
            return this.tCost > other.tCost;
        }

        // ```
        // Name : Equal
        // Parameter : PathNode other
        // Return : --
        // Purpose : check if the other is the same path
        // ```
        public bool Equal(PathNode other)
        {
            return x == other.x && y == other.y;
        }

    }
}