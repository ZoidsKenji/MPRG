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

        protected PathFind pathfinder;

        // rs6
        public float rpm = 800;
        protected float redLine = 6800;
        protected float rpmLim = 7000;
        protected float idleRpm = 700;
        protected List<float> gearRatio = new List<float> { 4.714f, 3.143f, 2.106f, 1.667f, 1.285f, 1, 0.839f, 0.667f};
        protected List<float> torque = new List<float> {300, 400, 550, 700, 750, 750, 700, 650, 0, 0, 0, 0, 0, 0, 0}; // for every 1000 rpm in Nm
        protected float finalDriveRatio = 3.204f;
        protected float tyreCircumference = 2.10f;
        public float gear = 1;

        protected float dragCoefficient = 0.28f;
        protected float frontalArea = 1.8f;

        protected float rollingResistanceCoefficient = 0.007f;
        protected float brakingForce = 200;
        public (float, float, float) radar = (1f, 1f, 1f);

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
            this.yPos = 900;
            this.xSpeed = 0;
            this.mass = 2075;
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
        public override void updateObject(float time, float camSpeed, float midPointX, float playerY, float playerX)
        {
            this.midpoint = (int)midPointX + 640;
            this.yPos += (camSpeed - speed) * time;

            float playerYPercentage = yPos / (playerY - renderDistance);
            float frontEnd = 180 + (playerYPercentage * (750 - 480));
            if (yPos > playerY - renderDistance)
            {
                this.pos.Y = frontEnd;
            }
            else
            {
                this.pos.Y = 0;
            }

            //scale = (int)Math.Floor(((pos.Y) * 0.01));
            scale = Math.Max((pos.Y - 480) / 120f, 0f);
            Console.WriteLine("police update" + " yPos" + this.yPos + " speed" + this.speed + " gear" + this.gear + " rpm" + this.rpm);
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
                // float momentOfInertia = 0.18f;
                // float viscousDampingCoefficent = 0.05f;
                // double pi = Math.PI;
                // float viscousLoss = viscousDampingCoefficent * ((rpm * 2 * (float)pi) / 60); // (rpm * 2 * (float)pi) / 60 is the angular speed
                // float netTorque = - viscousLoss;
                // float angularAccel = netTorque / momentOfInertia;
                // float rpmPerSec = angularAccel * 60 / (2 * (float)pi);
                // rpm += rpmPerSec * time;
            }
            else
            {
                rpm = idleRpm + 10;
            }

            if (rpm > rpmLim)
            {
                rpm = rpmLim;
            }

            if (rpm >= redLine && gear < gearRatio.Count)
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
                        accelerate(-1, time, 0, 0.5f);
                    }
                }
                else if (path[1].Item1 == 1)
                {
                    if (xPos < -200)
                    {
                        xSpeed = speed / sideSpeedDiv * time;
                        accelerate(-1, time, 0, 0.5f);
                    }
                    else if (xPos > 200)
                    {
                        xSpeed = -speed / sideSpeedDiv * time;
                        accelerate(-1, time, 0, 0.5f);
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
                        accelerate(-1, time, 0, 0.5f);
                    }
                    else
                    {
                        xSpeed = -speed / sideSlowSpeedDiv * time;
                    }
                }

                if (path[1].Item2 < startPos.Item2)
                {
                    accelerate(1, time, 1, 0);
                }
                else if (path[1].Item2 > startPos.Item2)
                {
                    accelerate(-1, time, 0, 1);
                }
                else
                {
                    accelerate(-1, time, 0, 0);
                }
            }
            moveX(xSpeed);
        }

        public virtual void accelerate(float accel, float time, float throttle, float brake)
        {

            // Engine & Gear
            float momentOfInertia = 0.18f;
            double pi = Math.PI;
            float engineDriveTorque = 0;
            if (torque.Count > (int)rpm/1000)
            {
                engineDriveTorque = torque[(int)rpm / 1000] * throttle;
            }
            else
            {
                engineDriveTorque = 0;
            }

            // braking
            brakingForce = 1.2f * mass;
            float ForceOnBrake = brakingForce * brake; // the resistance force from braking

            // drag and rolling resistance
            float rollingResistance = rollingResistanceCoefficient * mass * 9.81f;
            float dragForce = 0.5f * 1.225f * dragCoefficient * frontalArea * (speed / (2.237f * 3)) * (speed / (2.237f * 3)) * radar.Item3; // radar.Item3 is front radar for the air stream thingy
            float netResisForce = dragForce + rollingResistance + ForceOnBrake;

            float wheelResisTorque = netResisForce * (tyreCircumference / (2 * (float)pi)); // the resistance torque at the wheels caused by drag, rolling resistance and braking
            // net torque at engine
            float engineResistTorque = wheelResisTorque / (gearRatio[(int)gear - 1] * finalDriveRatio * 0.97f); // 0.97 is drivetrain efficiency
            float engineNetTorque = engineDriveTorque - engineResistTorque;

            //rpm change
            float angularAccel = engineNetTorque / momentOfInertia;
            float rpmPerSec = angularAccel * 60 / (2 * (float)pi);
            rpm += rpmPerSec * time;

            speed = (rpm * tyreCircumference) / (gearRatio[(int)gear - 1] * finalDriveRatio * 60) * 3f * 2.237f; // the 2.237 makes it mph
        }
        
        public void radarDetect(List<Sprite> cars)
        {
            float left = 1f;
            float right = 1f;
            float front = 1f;

            foreach (Sprite car in cars)
            {
                if (car == this)
                {
                    continue;
                }
                //front
                if ((yPos - car.yPos) < 360 && yPos > car.yPos && Math.Abs(xPos - car.xPos) < 60)
                {
                    front = Math.Min(front, 1 - ((yPos - car.yPos) / 270));
                }
                //side
                if (Math.Abs(yPos + 45 - car.yPos + 45) < 100 && Math.Abs(xPos - car.xPos) > 70)
                {
                    if (car.xPos < xPos)
                    {
                        left = Math.Min(left, 1 - ((xPos - car.xPos) / 300));
                    }
                    else
                    {
                        right = Math.Min(right, 1 - (Math.Abs(xPos - car.xPos) / 300));
                    }
                }

            }

            radar = (left, right, front);
        }
    }
}