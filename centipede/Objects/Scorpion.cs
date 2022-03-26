using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace centipede.Objects
{
    class Scorpion : AnimatedSprite
    {
        public Rectangle m_scorpionRectangle { get; set; }

        public Scorpion(Vector2 size, Vector2 center) : base(size, center)
        {
        }

        public void update(GameTime gameTime)
        {
            m_center.X += 2;

            updateRectangle();
        }
        public void updateRectangle()
        {
            m_scorpionRectangle = new Rectangle(
                (int)(m_center.X + (m_size.X / 2)), (int)(m_center.Y + (m_size.Y / 2)),  // Rectangle Location
                (int)m_size.X, (int)m_size.Y  // Rectangle Dimensions
            );
            m_spriteRectangle = m_scorpionRectangle;
        }

        public void scorpionCollision(List<Mushroom> mushrooms)
        {
            foreach (var mushroom in mushrooms)
            {
                if (m_scorpionRectangle.Intersects(mushroom.m_mushroomRectangle))
                {
                    mushroom.m_isPoisoned = true;
                    break;
                }
            }
        }
    }
}
