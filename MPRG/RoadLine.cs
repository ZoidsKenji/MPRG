using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class RoadLine : Sprite{

        int scale  = 0;
        public int midpoint = 1280 / 2;
        public int LeftOrRight = 0;
        public override Rectangle Rect{
            get{
                return new Rectangle((int)xPos, (int)pos.Y, scale, Math.Max(scale / 40, 3));
            }
        }

        public RoadLine(Texture2D texture, Vector2 pos, int LorR) : base(texture, pos){ // Left or right, 1 == left
            this.midpoint = 1280 / 2;
            LeftOrRight = LorR;
            scale = (int)Math.Floor(((pos.Y - 480) * 6.0));
            xPos = (int)Math.Floor(this.midpoint - (scale / 2.0));
        }

        public override void moveMidPoint(float xMove){
            // if (LeftOrRight == 1){
            //     this.midpoint = (int)xMove + 400;
            // }else{
            //     this.midpoint = (int)xMove + 870;
            // }
            this.midpoint = (int)xMove + 640;
            
            scale = Math.Max(0, (int)Math.Floor((pos.Y - 480) * 1.75));

            float curveFactor = (midpoint - (1280 / 2)) / (1280 / 2.0f);
            float curveStrength = 600;
            float yFactor = Math.Max(0, (pos.Y - 470) / 470.0f);

            xPos = (int)Math.Floor(midpoint - (scale / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);
            

        }

        public override void updateObject(float time, float playerSpeed, float midPoint){
            float speedFactor = 6;
            this.pos.Y += time * playerSpeed * (this.pos.Y / 480) * speedFactor;
        }

    }


}