using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace centipede
{
    public class AnimatedSprite
    {
        private Texture2D m_spriteSheet;
        private int[] m_spriteTime;

        private TimeSpan m_animationTime;
        private int m_subImageIndex;
        private int m_column;
        private Vector2 m_locationOnSheet;
        private Vector2 m_subImDimensions;

        private Rectangle m_spriteRectangle;

        private bool down = false;
        private bool multipleRows;


        public AnimatedSprite(Texture2D spriteSheet, int[] spriteTime, Vector2 locationOnSheet, Vector2 subImDimensions, bool multipleRows)
        {
            this.m_spriteSheet = spriteSheet;
            this.m_spriteTime = spriteTime;

            m_locationOnSheet = locationOnSheet;
            m_subImDimensions = subImDimensions;

            this.multipleRows = multipleRows;
        }

        public void update(GameTime gameTime)
        {
            m_animationTime += gameTime.ElapsedGameTime;
            if (m_animationTime.TotalMilliseconds >= m_spriteTime[m_subImageIndex])
            {
                m_animationTime -= TimeSpan.FromMilliseconds(m_spriteTime[m_subImageIndex]);
                m_subImageIndex++;
                m_subImageIndex = m_subImageIndex % m_spriteTime.Length;

                if (multipleRows)
                {
                    m_column = m_subImageIndex / 2;
                    down = !down;
                }
                else
                {
                    m_column = m_subImageIndex;
                }
            }
        }

        public void draw(SpriteBatch spriteBatch, Objects.AnimatedSprite model)
        {
            spriteBatch.Draw(
                m_spriteSheet,
                model.SpriteRectangle,
                new Rectangle((int)(m_locationOnSheet.X + (m_column * m_subImDimensions.X)), down ? (int)m_locationOnSheet.Y + (int)m_subImDimensions.Y : (int)m_locationOnSheet.Y, (int)m_subImDimensions.X, (int)m_subImDimensions.Y), // Source sub-texture
                Color.White,
                model.Rotation, // Angular rotation
                new Vector2(m_subImDimensions.X / 2, m_subImDimensions.Y / 2), // Center point of rotation
                SpriteEffects.None, 0);
        }
    }
}
