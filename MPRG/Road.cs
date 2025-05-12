using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Road : Sprite{

        int width  = 0;
        public int midpoint = 1280 / 2;
        public override Rectangle Rect{
            get{
                return new Rectangle((int)xPos, (int)pos.Y, width, 3);
            }
        }

        public override Rectangle BackendRect{
            get{
                return new Rectangle(0, 0, 300, 960);
            }
        }

        public Road(Texture2D texture, Vector2 pos) : base(texture, pos){
            this.midpoint = 1280 / 2;
            width = (int)Math.Floor(((pos.Y - 480) * 6.0));
            xPos = (int)Math.Floor(this.midpoint - (width / 2.0));
            this.backendColour = Color.DarkGray;

        }

        public override void moveMidPoint(float xMove){
            this.midpoint = (int)xMove + 640;
            width = (int)Math.Floor((pos.Y - 480) * 6.0);

            float curveFactor = (midpoint - (1280 / 2)) / (1280 / 2.0f);
            float curveStrength = 600;
            float yFactor = Math.Max(0, (pos.Y - 480) / 480.0f);

            xPos = (int)Math.Floor(midpoint - (width / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);

        }

        // public override void updateObject(float time, float playerSpeed){
        //     this.pos.Y -= playerSpeed * time;
        //     width = (int)Math.Floor((pos.Y) * 10);
        //     Console.WriteLine(width);
        // }

        public override void moveX(float velocity){
            //this.xPos += velocity;
        }

    }


}