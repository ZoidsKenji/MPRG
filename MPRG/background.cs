using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG{
    internal class Background : Sprite{
        public int midpoint = 1280 / 2;

        public int xPos = 0;
        public int ypos = 480;
        public int width = 1280;
        public int height = 960;

        public override Rectangle Rect{
            get{
                return new Rectangle((int)xPos, (int)pos.Y, 0, 3);
            }
        }

        public override Rectangle BackendRect{
            get{
                return new Rectangle(xPos, ypos, width, height);
            }
        }

        public Background(Texture2D texture, Vector2 pos = new Vector2()) : base(texture, pos){
            this.midpoint = 1280 / 2;
            // width = (int)Math.Floor(((pos.Y - 480) * 6.0));
            // xPos = (int)Math.Floor(this.midpoint - (width / 2.0));
            this.backendColour = Color.Green;
            this.texture = texture;
            this.xPos = 0;

        }

    }


}