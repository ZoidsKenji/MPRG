using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DXGI;


namespace MPRG
{
    internal class AiOpponent : Player
    {
        public float Xspeed = 0;
        public float[] DNA;
        public float score;


        public List<Sprite> allCarsNear;

        public AiOpponent(Texture2D texture, Vector2 pos, float[] genes = null) : base(texture, pos)
        {
            this.backendColour = Color.LimeGreen;
            DNA = genes ?? genRandGenes();
            allCarsNear = new List<Sprite>();

            //s2k
            this.rpm = 800;
            this.redLine = 6800;
            this.rpmLim = 8800;
            this.idleRpm = 800;
            this.gearRatio = new List<float> { 3.133f, 2.045f, 1.481f, 1.161f, 0.970f, 0.810f };
            this.torque = new List<float> {30, 50, 100, 150, 180, 200, 210, 208, 200, 0, 0, 0, 0, 0, 0 }; // for every 1000 rpm in Nm
            this.finalDriveRatio = 4.1f;
            this.tyreCircumference = 2.1f;
            this.gear = 1;
            this.mass = 1270; // in kg
        }

        public float[] genRandGenes()
        {
            int genesLength = 8;
            float[] genes = new float[genesLength];
            Random random = new Random();

            for (int i = 0; i < genesLength; i++)
            {
                genes[i] = (float)random.NextDouble() * (1 - -1) - 1;
            }

            return genes;
        }

        public void DecisionMaking(List<Sprite> cars, float time)
        {
            float fraction = 10f;

            (float, float, float) radar = radarDetection(cars);
            float left = radar.Item1;
            float right = radar.Item2;
            float front = radar.Item3;

            float horDec = (DNA[0] * left) + (DNA[1] * right) + (DNA[2] * front);
            float verDec = (DNA[3] * front) + (DNA[4] * speed);

            if (horDec > 0.5f)
            {
                Xspeed += (speed / 10) * time;
            }
            else if (horDec < -0.5f)
            {
                Xspeed -= (speed / 10) * time;
            }

            if (Xspeed > 0)
            {
                Xspeed -= time * fraction;
            }
            else if (Xspeed < 0)
            {
                Xspeed += time * fraction;
            }

            if (verDec > 0.5f)
            {
                accelerate(30 * time, time, 1);
            }
            else if (verDec < -0.5f)
            {
                accelerate(50 * time, time, -1);
            }

            moveX(Xspeed);
        }

        public (float, float, float) radarDetection(List<Sprite> cars)
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

            return (left, right, front);
        }

        public override void updateObject(float time, float camSpeed, float midPointX, float playerY)
        {
            score += 1 * time;
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

            if (yPos < -1000)
            {
                yPos = -1000;
                speed = camSpeed;
            }
            else if (yPos > 1800)
            {
                health -= (int)(20 * time);
                if (yPos > 2000)
                {
                    health = 0;
                }
            }

            if (health <= 0)
            {
                this.backendColour = Color.DarkGreen;
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

        // overtaking
        public void updateCarsPos(List<Sprite> cars)
        {
            foreach (Sprite all in cars)
            {
                if (Math.Abs(all.yPos - yPos) < 10 && all.yPos < yPos && !allCarsNear.Contains(all))
                {
                    allCarsNear.Add(all);
                }
                else if (Math.Abs(all.yPos - yPos) > 10 && all.yPos < yPos && allCarsNear.Contains(all))
                {
                    allCarsNear.Remove(all);
                }
                else if (allCarsNear.Contains(all) && all.yPos < yPos && Math.Abs(all.yPos - yPos) > 90)
                {
                    if (all is Player)
                    {
                        score += 50;
                    }
                    else
                    {
                        score += 10;
                    }
                    allCarsNear.Remove(all);
                }
            }
        }

        public override void accelerate(float accel, float time, float throttle)
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

            speed = ((rpm * tyreCircumference) / (gearRatio[(int)gear - 1] * finalDriveRatio * 60)) * 3f * 2.237f;
            //Console.WriteLine("aiSpeed" + speed + " aiHealth" + health + " aiXpos" + xPos + " aiXspeed " + Xspeed);
            // if (speed < 0)
            // {
            //     speed = 10;
            // }
        }
    }
}