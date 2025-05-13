using System;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Traffic : Sprite{

        public float scale = 1;

        public float setSpeed;

        public int midpoint = 1280 / 2;

        public int lane = new Random().Next(0, 3);

        public override Rectangle Rect{
            get{
                return new Rectangle((int)pos.X, (int)pos.Y, (int)Math.Floor(300 * scale), (int)Math.Floor(300 * scale));
            }
        }

        public override Rectangle BackendRect{
            get{
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos, 70, 90);
            }
        }


        public Traffic(Texture2D texture, Vector2 pos) : base(texture, pos){
            this.midpoint = 1280 / 2;
            this.xPos = (lane - 1) * 500;
            this.backendColour = Color.Orange;
            this.speed = 60;
            setSpeed = 60;
            if (lane == 0){
                this.speed = 60;
                setSpeed = 60;
            }else if (lane == 1){
                this.speed = 70;
                setSpeed = 70;
            }else{
                this.speed = 80;
                setSpeed = 80;
            }

        }

        private void laneXpos(){
            if (lane == 0){
                midpoint -= 200;
                float curveFactor = (midpoint - (1280 / 2)) / (1280 / 2.0f);
                float curveStrength = 600;
                float yFactor = Math.Max(0, (pos.Y - 470) / 470.0f);

                pos.X = (int)Math.Floor(midpoint - (scale * 300 / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);

                this.speed = 60;
            }else if (lane == 1){
                float curveFactor = (midpoint - (1280 / 2)) / (1280 / 2.0f);
                float curveStrength = 550;
                float yFactor = Math.Max(0, (pos.Y - 470) / 470.0f);

                pos.X = (int)Math.Floor(midpoint - (scale * 300 / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);

                this.speed = 70;
            }else if (lane == 2){
                midpoint += 200;
                float curveFactor = (midpoint - 640) / (1280 / 2.0f);
                float curveStrength = 600;
                float yFactor = Math.Max(0, (pos.Y - 470) / 470.0f);

                pos.X = (int)Math.Floor(midpoint - (scale * 300 / 2.0) - curveFactor * Math.Pow(1 - yFactor, 3) * curveStrength);
                this.speed = 80;

            }
        }

        public override void updateObject(float time, float playerSpeed, float midPointX){
            this.midpoint = (int)midPointX + 640;
            this.pos.Y += (playerSpeed - speed) * time * (this.pos.Y / 480);
            this.yPos += (playerSpeed - speed) * time;
            //scale = (int)Math.Floor(((pos.Y) * 0.01));
            scale = Math.Max((pos.Y - 480) / 120f, 0f);
            laneXpos();
            if (speed > setSpeed){
                speed -= time * 2;
            }else{
                speed += time * 2;
            }
        }

        public override void moveX(float velocity){
            //this.xPos += velocity;
        }

        public override void setSpeedTo(float Speed){
            speed = Speed;
        }

    }


}