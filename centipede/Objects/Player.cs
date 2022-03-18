using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace centipede.Objects
{
    class Player
    {
        private List<Laser> m_lasers = new List<Laser>();
        private Vector2 m_playerSize;

        private int m_xPos;
        private int m_yPos;

        private int m_gameWidth = 512;
        private int m_gameHeight = 512;

        private bool shotAvailable = true;

        private float elapsedTime;

        public Player(Vector2 size, Vector2 position)
        {
            this.m_playerSize = size;
            this.m_xPos = (int)position.X;
            this.m_yPos = (int)position.Y;
        }

        public void update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.W) && m_yPos > m_gameHeight - (m_gameHeight / 4))
            {
                m_yPos -= 2;
            }
            if (state.IsKeyDown(Keys.A) && m_xPos > 0)
            {
                m_xPos -= 2;
            }
            if (state.IsKeyDown(Keys.S) && m_yPos < m_gameHeight - m_playerSize.Y)
            {
                m_yPos += 2;
            }
            if (state.IsKeyDown(Keys.D) && m_xPos < m_gameWidth - m_playerSize.X)
            {
                m_xPos += 2;
            }
            if (state.IsKeyDown(Keys.Space) && shotAvailable)
            {
                m_lasers.Add(new Laser(
                    new Vector2(16, 16), // size
                    new Vector2(m_xPos, m_yPos) // location
                ));

                shotAvailable = false;
                elapsedTime = 0;
            }

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= .3f)
            {
                shotAvailable = true;

                float carryOverTime = elapsedTime - .3f;
                elapsedTime = carryOverTime;
            }

            foreach (var laser in m_lasers)
            {
                laser.update(gameTime);
            }

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

            foreach (var laser in m_lasers)
            {
                laser.draw(spriteBatch, spriteSheet, new Vector2(locationOnSheet.X + 8, locationOnSheet.Y), subImDimensions);
            }
        }
    }
}
