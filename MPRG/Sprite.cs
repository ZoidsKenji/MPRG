using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Sprite{

        //private static readonly float scale = 3f;
        public Texture2D texture;
        public Vector2 pos;
        public Color colour;
        public Color backendColour;

        public float speed;

        public float xPos = 0;
        public float yPos = 0;

        public virtual Rectangle Rect{
            get{
                return new Rectangle((int)pos.X, (int)pos.Y, 423, 285);
            }
        }

        public virtual Rectangle BackendRect{
            get{
                return new Rectangle((int)xPos, (int)yPos, 423, 285);
            }
        }

        public Sprite(Texture2D texture, Vector2 pos){
            Console.WriteLine("NewSprite");
            this.texture = texture;
            //this.BackendTexture = backendTexture;
            this.pos = pos;
            this.colour = Color.White;
            this.backendColour = Color.White;
            this.xPos = 0;
            this.yPos = 0;
            this.speed = 0;
        }

        public virtual void moveX(float velocity){
            this.xPos += velocity;
        }

        public virtual void moveMidPoint(float xPos){

        }

        public virtual void updateObject(float time, float playerSpeed, float midPoint){

        }

        public virtual void setSpeedTo(float Speed){
            speed = Speed;
        }
    }

}