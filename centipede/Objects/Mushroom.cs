using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace centipede.Objects
{
    class Mushroom
    {
        public Vector2 m_mushroomSize { get; set; }
        public int m_xPos { get; set; }
        public int m_yPos { get; set; }

        public int m_deteriorationState { get; set; }
        public bool m_isPoisoned { get; set; }

        public Rectangle m_mushroomRectangle { get; set; }


        public Mushroom(Vector2 size, Vector2 position)
        {
            this.m_mushroomSize = size;
            this.m_xPos = (int)position.X;
            this.m_yPos = (int)position.Y;
            this.m_deteriorationState = 0;
            this.m_isPoisoned = false;

            m_mushroomRectangle = new Rectangle(
                (int)(m_xPos + (m_mushroomSize.X / 2)), (int)(m_yPos + (m_mushroomSize.Y / 2)),
                (int)m_mushroomSize.X, (int)m_mushroomSize.Y
            );
        }

        public void update(GameTime gameTime)
        {
        }

        public void draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 locationOnSheet, Vector2 subImDimensions)
        {
            spriteBatch.Draw(
                spriteSheet,
                m_mushroomRectangle, // Destination rectangle
                new Rectangle(
                    (int)locationOnSheet.X + (m_deteriorationState * 8), 
                    m_isPoisoned ? (int)locationOnSheet.Y + 8 : (int)locationOnSheet.Y, 
                    (int)subImDimensions.X, 
                    (int)subImDimensions.Y
                ), // Source sub-texture
                Color.White,
                0, // Angular rotation
                new Vector2(subImDimensions.X / 2, subImDimensions.Y / 2), // Center point of rotation
                SpriteEffects.None, 0);
        }
    }
}
