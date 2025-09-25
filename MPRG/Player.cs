using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class Player : Sprite
    {

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
        }

    }

}