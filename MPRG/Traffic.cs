using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Traffic : Sprite{

        float scale  = 1;
        int xpos = 0;

        public int speed = 60;

        public int midpoint = 1280 / 2;

        public int lane = new Random().Next(0, 3);

        public override Rectangle Rect{
            get{
                return new Rectangle((int)xpos, (int)pos.Y, (int)Math.Floor(300 * scale), (int)Math.Floor(300 * scale));
            }
        }

        public Traffic(Texture2D texture, Vector2 pos) : base(texture, pos){
            this.midpoint = 1280 / 2;
            this.xPos = (lane - 1) * 500;
        }

        private void laneXpos(){
            if (lane == 0){
                float curveFactor = (this.midpoint - (1280 / 2)) / (1280 / 2.0f);
                float curveStrength = 600;
                float yFactor = Math.Max(0, (pos.Y - 480) / 480.0f);

                xPos = (int)Math.Floor(this.midpoint - ((scale * 300) / 2.0) - (curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength));

                speed = 50;
            }else if (lane == 1){
                float curveFactor = (this.midpoint - (1280 / 2.0f)) / (1280 / 2.0f);
                float curveStrength = 0;
                float yFactor = Math.Max(0, (pos.Y - 480) / 480.0f);

                xPos = (int)Math.Floor(this.midpoint - ((scale * 300) / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);

                speed = 60;
            }else if (lane == 2){
                float curveFactor = (this.midpoint - (1280 / 2)) / (1280 / 2.0f);
                float curveStrength = -600;
                float yFactor = Math.Max(0, (pos.Y - 480) / 480.0f);

                xPos = (int)Math.Floor(this.midpoint - ((scale * 300) / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);
                speed = 70;

            }
        }

        public override void updateObject(float time, float playerSpeed, float midPointX){
            this.midpoint = (int)midPointX + 640;
            this.pos.Y += (playerSpeed - speed) * time * (this.pos.Y / 480);
            //scale = (int)Math.Floor(((pos.Y) * 0.01));
            scale = Math.Max((pos.Y - 480) / 120f, 0f);
            laneXpos();
        }

    }


}