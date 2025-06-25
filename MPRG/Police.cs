using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Police : Sprite
    {

        public float scale = 1;
        public int midpoint = 1280 / 2;

        public float xSpeed;

        public PathFind pathfinder;

        public override Rectangle Rect
        {
            get
            {
                return new Rectangle((int)pos.X, (int)pos.Y, (int)Math.Floor(300 * scale), (int)Math.Floor(300 * scale));
            }
        }

        public override Rectangle BackendRect
        {
            get
            {
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos, 70, 90);
            }
        }

        public Rectangle DetectionRect
        {
            get
            {
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos - 90, 70, 90);
            }
        }

        public Police(Texture2D texture, Vector2 pos) : base(texture, pos)
        {
            this.midpoint = 1280 / 2;
            this.backendColour = Color.Blue;
            this.speed = 90;
            this.yPos = 1280;
            this.xSpeed = 0;

            // blank map (default)
            // List<List<int>> map = [new List<int>(), new List<int>(), new List<int>()];
            // for (int i = 0; i < 3; i++)
            // {
            //     for (int n = 0; n < 25; n++)
            //     {
            //         map[i].Add(0);
            //     }
            // }
            // this.pathfinder = new PathFind(map);
        }

        public override void updateObject(float time, float playerSpeed, float midPointX)
        {
            this.midpoint = (int)midPointX + 640;
            this.pos.Y += (playerSpeed - speed) * time * (this.pos.Y / 480);
            this.yPos += (playerSpeed - speed) * time;
            //scale = (int)Math.Floor(((pos.Y) * 0.01));
            scale = Math.Max((pos.Y - 480) / 120f, 0f);
            Console.WriteLine("police update" + " " + this.yPos + " " + this.speed);
        }

        public override void moveX(float velocity)
        {
            this.xPos += velocity;
        }

        public override void setSpeedTo(float Speed)
        {
            speed = Speed;
            Console.WriteLine($"police {this.speed}");
        }

        public void showPath(List<List<int>> map, List<(int x, int y)> path = null)
        {
            List<(int x, int y)> pathSet = new();
            if (path != null)
            {
                foreach ((int, int) road in path)
                {
                    pathSet.Add(road);
                }
            }

            for (int y = 0; y < map.Count(); y++)
                {
                    string row = "";
                    for (int x = 0; x < map[0].Count(); x++)
                    {
                        if (pathSet.Contains((y, x)))
                        {
                            if ((y, x) == path[0])
                            {
                                row += "3";
                            }
                            else if ((y, x) == path.Last())
                            {
                                row += "2";
                            }
                            else
                            {
                                row += "4";
                            }

                        }
                        else if (map[y][x] == 1)
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

        public void findPath(List<List<int>> map)
        {
            //showPath(map);
            pathfinder = new PathFind(map);

            var startPos = ItemPos(map, 3);
            var endPos = ItemPos(map, 2);

            List<(int, int)> path = pathfinder.findPath(startPos, endPos);
            showPath(map, path);
        }
        
    }
}