using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class Button
    {
        public Vector2 pos;
        public Vector2 size;
        public Texture2D texture;
        public Color colour;
        public Color textcolour;
        public string text;

        
        public virtual Rectangle Rect
        {
            get
            {
                return new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
            }
        }

        

        public Button(Texture2D texture, Vector2 pos, Vector2 size, string text, Color colour, Color textcolour)
        {
            this.texture = texture;
            this.pos = pos;
            this.size = size;
            this.text = text;
            this.colour = colour;
            this.textcolour = textcolour;
        }

        

    }

}