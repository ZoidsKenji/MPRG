using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Player : Sprite{

        public override Rectangle Rect{
            get{
                return new Rectangle((int)pos.X, (int)pos.Y, 300, 300);
            }
        }

        public override Rectangle BackendRect{
            get{
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), 435, 70, 90);
            }
        }

        public Player(Texture2D texture, Vector2 pos) : base(texture, pos){
            this.backendColour = Color.LightGray;
            this.yPos = 435;
            this.speed = 55;
        }

        public void accelerate(float accel){
            speed += accel;
            Console.WriteLine(speed);
        }

        public override void setSpeedTo(float Speed){
            speed = Speed;
        }

    }


}