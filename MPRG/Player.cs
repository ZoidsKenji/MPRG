using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Player : Sprite{

        public float speed = 50;

        public override Rectangle Rect{
            get{
                return new Rectangle((int)pos.X, (int)pos.Y, 300, 300);
            }
        }

        public override Rectangle BackendRect{
            get{
                return new Rectangle((int)(xPos * 3 / 10) + (150 - 35), 435, 70, 90);
            }
        }

        public Player(Texture2D texture, Vector2 pos) : base(texture, pos){
            this.backendColour = Color.LightGray;

        }

        public void accelerate(float accel){
            speed += accel;
            //Console.WriteLine(speed);
        }

    }


}