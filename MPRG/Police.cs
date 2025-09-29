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

        public float rpm = 800;
        public float rpmLimit = 6500;
        public float idleRpm = 800;
        public List<float> gearRatio = new List<float> { 3.230f, 1.913f, 1.258f, 0.918f, 0.731f };
        public List<float> torque = new List<float> {60, 70, 120, 160, 171, 170, 160, 130, 120 }; // for every 1000 rpm in Nm
        public float finalDriveRatio = 4.285f;
        public float tyreCircumference = 1.893f;
        public float gear = 1;
        public float mass = 1050;

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
            //this.xPos = 0;

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

        // ```
        // Name : updateObject
        // Parameter : float time, float camSpeed, float midPointX
        // Return : --
        // Purpose : it override from Sprite class, and updates the sprite
        // ```
        public override void updateObject(float time, float camSpeed, float midPointX)
        {
            this.midpoint = (int)midPointX + 640;
            this.pos.Y += (camSpeed - speed) * time * (this.pos.Y / 480);
            this.yPos += (camSpeed - speed) * time;
            //scale = (int)Math.Floor(((pos.Y) * 0.01));
            scale = Math.Max((pos.Y - 480) / 120f, 0f);
            Console.WriteLine("police update" + " yPos" + this.yPos + " speed" + this.speed);
            if (yPos > 1500)
            {
                yPos = 1500;
                speed = camSpeed;
            }
            else if (yPos < -1000)
            {
                yPos = -1000;
                speed = camSpeed;
            }

            if (iFrame > 0)
            {
                iFrame -= 1;
            }
            else
            {
                iFrame = 0;
            }

            if (rpm > idleRpm)
            {
                float momentOfInertia = 0.18f;
                float viscousDampingCoefficent = 0.05f;
                double pi = Math.PI;
                float viscousLoss = viscousDampingCoefficent * ((rpm * 2 * (float)pi) / 60); // (rpm * 2 * (float)pi) / 60 is the angular speed
                float netTorque = - viscousLoss;
                float angularAccel = netTorque / momentOfInertia;
                float rpmPerSec = angularAccel * 60 / (2 * (float)pi);
                rpm += rpmPerSec * time;
            }
            else
            {
                rpm = 810;
            }

            if (rpm >= rpmLimit && gear < gearRatio.Count)
            {
                gear += 1;
                rpm = (rpm * gearRatio[(int)gear - 1]) / gearRatio[(int)gear - 2];
            }
            else if (rpm < 1500 && gear > 1)
            {
                gear -= 1;
                rpm = (rpm * gearRatio[(int)gear - 1]) / gearRatio[(int)gear];
            }
        }

        // ```
        // Name : showPath
        // Parameter : List<List<int>> map, List<(int x, int y)> path = null
        // Return : --
        // Purpose : print out the path for debug
        // ```
        public void showPath(List<List<int>> map, List<(int x, int y)> path = null)
        {
            Console.WriteLine("Path:");
            List<(int x, int y)> pathSet = new();
            if (path != null)
            {
                foreach ((int, int) road in path)
                {
                    pathSet.Add(road);
                }
            }

            for (int y = map.Count() - 1; y > -1; y--)
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
            Console.WriteLine();
        }

        // ```
        // Name : findPath
        // Parameter : List<List<int>> map, float time, float playerSpeed
        // Return : --
        // Purpose : runs the A* path find algrithm and controls the police car
        // ```
        public void findPath(List<List<int>> map, float time, float playerSpeed)
        {

            var startPos = ItemPos(map, 3);
            var endPos = ItemPos(map, 2);

            List<(int, int)> path = new List<(int, int)>();

            if (startPos != (-1, -1) && endPos != (-1, -1))
            {
                List<List<int>> newMap = map;
                newMap[startPos.Item1][startPos.Item2] = 0;
                newMap[endPos.Item1][endPos.Item2] = 0;
                pathfinder = new PathFind(newMap);

                path = pathfinder.findPath(startPos, endPos);
            }
            else
            {
                path = null;
            }

            if (path != null)
            {
                showPath(map, path);
                //Console.WriteLine(path[1]);
                float sideSpeedDiv = 0.75f;
                float sideSlowSpeedDiv = 3;
                if (path[1].Item1 == 0)
                {
                    if (xPos > -360)
                    {
                        xSpeed = -speed / sideSpeedDiv * time;
                    }
                    else
                    {
                        xSpeed = +speed / sideSlowSpeedDiv * time;
                    }
                }
                else if (path[1].Item1 == 1)
                {
                    if (xPos < -200)
                    {
                        xSpeed = speed / sideSpeedDiv * time;
                    }
                    else if (xPos > 200)
                    {
                        xSpeed = -speed / sideSpeedDiv * time;
                    }
                    else
                    {
                        float direction = (xPos != 0) ? (xPos / Math.Abs(xPos)) : 1;
                        xSpeed = direction * -Math.Abs(speed / sideSlowSpeedDiv * time);
                    }
                }
                else if (path[1].Item1 == 2)
                {
                    if (xPos < 360)
                    {
                        xSpeed = speed / sideSpeedDiv * time;
                    }
                    else
                    {
                        xSpeed = -speed / sideSlowSpeedDiv * time;
                    }
                }

                if (path[1].Item2 < startPos.Item2)
                {
                    accelerate(40, time, 1);
                }
                else if (speed > (playerSpeed * 0.8))
                {
                    accelerate(-40, time, -1);
                }
                else
                {
                    accelerate(0, time, 0);
                }
            }
            moveX(xSpeed);
        }

        public void accelerate(float accel, float time, float throttle)
        {
            float momentOfInertia = 0.18f;
            float viscousDampingCoefficent = 0.05f;
            double pi = Math.PI;
            float rpmtorque = torque[(int)rpm / 1000] * throttle;
            float viscousLoss = viscousDampingCoefficent * ((rpm * 2 * (float)pi) / 60); // (rpm * 2 * (float)pi) / 60 is the angular speed
            float netTorque = rpmtorque - viscousLoss;
            float angularAccel = netTorque / momentOfInertia;
            float rpmPerSec = angularAccel * 60 / (2 * (float)pi);
            rpm += rpmPerSec * time;

            speed = ((rpm * tyreCircumference) / (gearRatio[(int)gear - 1] * finalDriveRatio * 60)) * 2.5f * 2.237f;
        }
        
    }
}