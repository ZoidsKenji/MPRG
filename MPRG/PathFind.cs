using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class PathFind
    {
        public List<List<int>> map;
        public int height;
        public int width;

        public List<List<PathNode>> nodes;

        public PathFind(List<List<int>> map)
        {
            this.map = map;
            height = map.Count;
            width = map[0].Count;
            nodes = new List<List<PathNode>>();

            for (int y = 0; y < height; y++){
                List<PathNode> row = [];
                for (int x = 0; x < width; x++){
                    bool drivible = map[x][y] == 0;
                    row.Add(new PathNode(x, y, drivible));
                }
                nodes.Add(row);
            }
        }

        public PathNode getNode(int x, int y){
            if (0 <= x && x < width && 0 <= y && y < height){
                return nodes[x][y];
            }
            return null;
        }
    }
}