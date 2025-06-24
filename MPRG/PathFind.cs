using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
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

            for (int y = 0; y < height; y++)
            {
                List<PathNode> row = [];
                for (int x = 0; x < width; x++)
                {
                    bool drivible = map[x][y] == 0;
                    row.Add(new PathNode(x, y, drivible));
                }
                nodes.Add(row);
            }
        }

        public PathNode getNode(int x, int y)
        {
            if (0 <= x && x < width && 0 <= y && y < height)
            {
                return nodes[x][y];
            }
            return null;
        }

        public List<PathNode> getNeighbors(PathNode node)
        {
            List<PathNode> neighbors = null;

            List<HashSet<int>> directions = [
                new HashSet<int> {-1, -1}, new HashSet<int> {-1, 0}, new HashSet<int> {-1, 1},
                new HashSet<int> {0, -1}, new HashSet<int> {0, 1},
                new HashSet<int> {1, -1}, new HashSet<int> {1, 0}, new HashSet<int> {1, 1}
            ];

            foreach (HashSet<int> dirHash in directions)
            {
                List<int> dirList = dirHash.ToList();
                int dx = dirList[0];
                int dy = dirList[1];

                PathNode neighbor = getNode(node.x + dx, node.y + dy);

                if (neighbor != null && neighbor.drivible)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public float calDistance(PathNode nodeA, PathNode nodeB)
        {
            int dx = Math.Abs(nodeA.x - nodeB.x);
            int dy = Math.Abs(nodeA.y - nodeB.y);

            return (float)Math.Sqrt((dx * dx) + (dy * dy));
        }

        public int calHeuristic(PathNode node, PathNode target)
        {
            return Math.Abs(node.x - target.x) + Math.Abs(node.y - target.y);
        }

        public List<HashSet<int>> reconstructPath(PathNode endNode)
        {
            List<HashSet<int>> path = [];
            PathNode current = endNode;

            while (current != null)
            {
                path.Add(new HashSet<int> { current.x, current.y });
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        public List<HashSet<int>> findPath(HashSet<int> start, HashSet<int> end)
        {
            List<int> startList = start.ToList();
            List<int> endList = end.ToList();

            PathNode startNode = getNode(startList[0], startList[1]);
            PathNode endNode = getNode(endList[0], endList[1]);

            if (startNode == null || endNode == null)
            {
                return null;
            }

            if (!startNode.drivible || !endNode.drivible)
            {
                return null;
            }

            startNode.dCost = 0;
            startNode.hCost = calHeuristic(startNode, endNode);
            startNode.tCost = startNode.dCost + startNode.hCost;

            List<PathNode> openSet = [startNode];
            List<PathNode> closedSet = [];

            while (openSet != null)
            {
                openSet.Sort((a, b) => a.tCost.CompareTo(b.tCost));
                PathNode currentNode = openSet[0];
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    return reconstructPath(currentNode);
                }

                foreach (PathNode neighbor in getNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    float newDCost = currentNode.dCost + calDistance(currentNode, neighbor);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (newDCost > neighbor.dCost)
                    {
                        continue;
                    }

                    neighbor.parent = currentNode;
                    neighbor.dCost = newDCost;
                    neighbor.hCost = calHeuristic(neighbor, endNode);
                    neighbor.tCost = neighbor.dCost + neighbor.dCost;
                }
            }
            return null;
        }

        public void showPath(List<List<int>> map, List<HashSet<int>> path = null)
        {
            List<HashSet<int>> pathSet = null;
            foreach (HashSet<int> road in path)
            {
                pathSet.Add(road);
            }

            for (int y = 0; y < map.Count(); y++)
            {
                string row = "";
                for (int x = 0; x < map[0].Count(); x++)
                {
                    HashSet<int> grid = new HashSet<int> { x, y };
                    if (pathSet.Contains(grid))
                    {
                        if (grid == path[0])
                        {
                            row += "3";
                        }
                        else if (grid == path[-1])
                        {
                            row += "2";
                        }
                        else
                        {
                            row += "4";
                        }

                    }
                    else if (map[x][y] == 1)
                    {
                        row += "1";
                    }
                    else
                    {
                        row += "0";
                    }
                }
                Console.WriteLine(row);

            }
        }
        
        
    }
}