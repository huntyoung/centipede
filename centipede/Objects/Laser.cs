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
        private Vector2 m_playerSize;

        private int m_xPos;
        private int m_yPos;

        public Laser(Vector2 size, Vector2 position)
        {
            this.m_playerSize = size;
            this.m_xPos = (int)position.X;
            this.m_yPos = (int)position.Y;
        }

        public void update(GameTime gameTime)
        {
            m_yPos -= 5;
        }

        public void draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 locationOnSheet, Vector2 subImDimensions)
        {
            spriteBatch.Draw(
                spriteSheet,
                new Rectangle((int)(m_xPos + (m_playerSize.X / 2)), (int)(m_yPos + (m_playerSize.Y / 2)), (int)m_playerSize.X, (int)m_playerSize.Y), // Destination rectangle
                new Rectangle((int)locationOnSheet.X, (int)locationOnSheet.Y, (int)subImDimensions.X, (int)subImDimensions.Y), // Source sub-texture
                Color.White,
                0, // Angular rotation
                new Vector2(subImDimensions.X / 2, subImDimensions.Y / 2), // Center point of rotation
                SpriteEffects.None, 0);
        }
    }
}
