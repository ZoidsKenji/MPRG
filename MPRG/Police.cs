using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Police : Sprite{

        public float scale = 1;
        public int midpoint = 1280 / 2;

        public float xSpeed;

        public override Rectangle Rect
        {
            get
            {
                return new Rectangle((int)pos.X, (int)pos.Y, (int)Math.Floor(300 * scale), (int)Math.Floor(300 * scale));
            }
        }

        public override Rectangle BackendRect{
            get{
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos, 70, 90);
            }
        }

        public Rectangle DetectionRect{
            get{
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos - 90, 70, 90);
            }
        }

        public Police(Texture2D texture, Vector2 pos) : base(texture, pos)
        {
            this.midpoint = 1280 / 2;
            this.backendColour = Color.Blue;
            this.speed = 90;
            this.yPos = 1280;
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
    }
}