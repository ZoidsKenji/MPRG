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
            height = map[0].Count;
            width = map.Count;
            nodes = new List<List<PathNode>>();

            for (int x = 0; x < width; x++)
            {
                List<PathNode> row = new List<PathNode>();
                for (int y = 0; y < height; y++)
                {
                    bool drivible = map[x][y] == 0;
                    row.Add(new PathNode(x, y, drivible));
                }
                nodes.Add(row);
            }
        }

        // ```
        // Name : getNode
        // Parameter : (int, int) pos
        // Return : --
        // Purpose : check if the other is the same path
        // ```
        public PathNode getNode((int, int) pos)
        {
            if (0 <= pos.Item1 && pos.Item1 < width && 0 <= pos.Item2 && pos.Item2 < height)
            {
                return nodes[pos.Item1][pos.Item2];
            }
            return null;
        }

        public List<PathNode> getNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();

            List<List<int>> directions = [
                new List<int> {-1, 0},
                new List<int> {0, -1}, new List<int> {0, 1},
                new List<int> {1, 0}
            ];

            foreach (List<int> dirHash in directions)
            {
                int dx = dirHash[0];
                int dy = dirHash[1];

                (int, int) pos = (node.x + dx, node.y + dy);
                PathNode neighbor = getNode(pos);

                if (neighbor != null && neighbor.drivible)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public int calDistance(PathNode nodeA, PathNode nodeB)
        {
            int dx = Math.Abs(nodeA.x - nodeB.x);
            int dy = Math.Abs(nodeA.y - nodeB.y);

            return dx + dy;
        }

        public int calHeuristic(PathNode node, PathNode target)
        {
            return Math.Abs(node.x - target.x) + Math.Abs(node.y - target.y);
        }

        public List<(int, int)> reconstructPath(PathNode endNode)
        {
            List<(int, int)> path = new List<(int, int)>();
            PathNode current = endNode;

            while (current != null)
            {
                path.Add((current.x, current.y));
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        public List<(int, int)> findPath((int, int) start, (int, int) end)
        {

            PathNode startNode = getNode(start);
            PathNode endNode = getNode(end);

            if (startNode == null || endNode == null)
            {
                Console.WriteLine("No startpoint or endpoint");
                return null;
            }

            // if (!startNode.drivible || !endNode.drivible)
            // {
            //     Console.WriteLine("Not drivible");
            //     return null;
            // }

            startNode.dCost = 0;
            startNode.hCost = calHeuristic(startNode, endNode);
            startNode.tCost = startNode.dCost + startNode.hCost;

            List<PathNode> openSet = [startNode];
            List<PathNode> closedSet = new List<PathNode>();

            while (openSet.Count != 0)
            {
                openSet.Sort((a, b) => a.tCost.CompareTo(b.tCost));
                PathNode currentNode = openSet[0];
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode.Equal(endNode))
                {
                    return reconstructPath(currentNode);
                }

                foreach (PathNode neighbor in getNeighbors(currentNode))
                {
                    if (closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newDCost = currentNode.dCost + calDistance(currentNode, neighbor);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (newDCost >= neighbor.dCost)
                    {
                        continue;
                    }

                    neighbor.parent = currentNode;
                    neighbor.dCost = newDCost;
                    neighbor.hCost = calHeuristic(neighbor, endNode);
                    neighbor.tCost = neighbor.dCost + neighbor.hCost;
                }
            }
            Console.WriteLine("No path");
            return null;
        }

    }
}