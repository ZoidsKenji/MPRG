using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class Player : Sprite
    {

        public float rpm = 800;
        public float rpmLimit = 7800;
        public List<float> gearRatio = new List<float> { 0.3f, 0.4f, 0.5f, 0.6f, 0.7f };
        public float gear = 1;

        public override Rectangle Rect
        {
            get
            {
                return new Rectangle((int)pos.X, (int)pos.Y, 300, 300);
            }
        }

        public override Rectangle BackendRect
        {
            get
            {
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos, 70, 90);
            }
        }

        public Player(Texture2D texture, Vector2 pos) : base(texture, pos)
        {
            this.backendColour = Color.LightGray;
            this.yPos = 435;
            this.speed = 80;
        }

        public virtual void accelerate(float accel)
        {
            speed += accel;
            if (rpm < rpmLimit)
            {
                rpm += accel * 200;
            }
            Console.WriteLine("playerSpeed" + speed + " playerHealth" + health + " playerXpos" + xPos);
            // if (speed < 0)
            // {
            //     speed = 10;
            // }
        }

        public override void setSpeedTo(float Speed)
        {
            speed = Speed;
            if (speed < 0)
            {
                speed = 10;
            }
        }

        // ```
        // Name : updateObject
        // Parameter : float time, float camSpeed, float midPointX
        // Return : --
        // Purpose : it override from Sprite class, and updates the sprite
        // ```
        public override void updateObject(float time, float camSpeed, float midPoint)
        {
            this.yPos += (camSpeed - speed) * time;
            if (health < 1)
            {
                //this.yPos = 1001;
                speed = 0;
            }

            if (iFrame > 0)
            {
                iFrame -= 1;
            }
            else
            {
                iFrame = 0;
            }

            if (rpm > 800)
            {
                rpm -= time * 150;
            }

            if (rpm >= rpmLimit && gear < gearRatio.Count)
            {
                gear += 1;
                rpm = rpmLimit * gearRatio[(int)gear - 1];
            }
            else if (rpm < (rpmLimit * gearRatio[(int)gear - 1] - 100) && gear > 1)
            {
                gear -= 1;
                rpm = rpmLimit * (1 - gearRatio[(int)gear - 1]);
            }
        }

    }

}