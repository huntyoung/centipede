using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace centipede.Objects
{
    class Spider : AnimatedSprite
    {
        private bool goingRight = true;
        private int randX;
        private int randY;

        private int[] xLeft = new int[] { -1, 0 };
        private int[] xRight = new int[] { 0, 1 };
        private int[] y = new int[] { -1, 1 };

        private int m_gameWidth = 512;
        private int m_gameHeight = 512;

        private float elapsedTime;

        public Spider(Vector2 size, Vector2 center) : base(size, center)
        {
        }

        public void update(GameTime gameTime, Random rnd)
        {
            m_center.X += randX;

            if (m_center.Y < m_gameHeight - (m_gameHeight / 3)) randY = 1;
            else if (m_center.Y > m_gameHeight - 16) randY = -1;
            m_center.Y += randY;

            if (m_center.X == m_gameWidth + m_size.X) goingRight = false;
            else if (m_center.X == m_size.X * -2) goingRight = true;

            elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (elapsedTime >= 1.5f)
            {
                if (goingRight) randX = xRight[rnd.Next(xRight.Length)];
                else randX = xLeft[rnd.Next(xLeft.Length)];
                randY = y[rnd.Next(y.Length)];

                float carryOverTime = elapsedTime - 1.5f;
                elapsedTime = carryOverTime;
            }
        }
    }
}
