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

        public void DecisionMaking(List<Sprite> cars)
        {
            (float, float, float) radar = radarDetection(cars);
            float left = radar.Item1;
            float right = radar.Item2;
            float front = radar.Item3;

            float horDec = (DNA[0] * left) + (DNA[1] * right) + (DNA[2] * front);
            float verDec = (DNA[3] * front) + (DNA[4] * speed);

            if (horDec > 0.5f)
            {
                moveX(speed / 10);
            }
            else if (horDec < -0.5f)
            {
                moveX(-speed / 10);
            }

            if (verDec > 0.5f)
            {
                accelerate(30);
            }
            else if (verDec < -0.5f)
            {
                accelerate(50);
            }
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

        public override void updateObject(float time, float camSpeed, float midPointX)
        {
            score += 1 * time;
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

        public override void accelerate(float accel)
        {
            speed += accel;
            Console.WriteLine("aiSpeed" + speed + " aiHealth" + health + " aiXpos" + xPos + " aiXspeed " + Xspeed);
            // if (speed < 0)
            // {
            //     speed = 10;
            // }
        }
    }
}