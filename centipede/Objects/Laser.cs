using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace centipede.Objects
{
    class Laser
    {
        public Vector2 m_laserSize { get; set; }

        public int m_xPos { get; set; }
        public int m_yPos { get; set; }

        public Rectangle m_laserRectangle { get; set; }

        public Laser(Vector2 size, Vector2 position)
        {
            this.m_laserSize = size;
            this.m_xPos = (int)position.X;
            this.m_yPos = (int)position.Y;
        }

        public void update(GameTime gameTime)
        {
            m_yPos -= 8;
            m_laserRectangle = new Rectangle(
                (int)(m_xPos), (int)(m_yPos), 
                (int)m_laserSize.X, (int)m_laserSize.Y
            );
        }

        public void draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 locationOnSheet, Vector2 subImDimensions)
        {
            spriteBatch.Draw(
                spriteSheet,
                m_laserRectangle, // Destination rectangle
                new Rectangle((int)locationOnSheet.X, (int)locationOnSheet.Y, (int)subImDimensions.X, (int)subImDimensions.Y), // Source sub-texture
                Color.White,
                0, // Angular rotation
                new Vector2(subImDimensions.X / 2, subImDimensions.Y / 2), // Center point of rotation
                SpriteEffects.None, 0);
        }
    }
}
