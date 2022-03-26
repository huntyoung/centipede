using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace centipede.Objects
{
    class Flea : AnimatedSprite
    {
        public Rectangle m_fleaRectangle { get; set; }

        private int m_mushroomsDropped = 5;

        private Random rnd = new Random();
        private List<int> randomYLocations;

        public Flea(Vector2 size, Vector2 center) : base(size, center)
        {
            randomYLocations = new List<int>();
            for (int i=0; i<m_mushroomsDropped; i++)
            {
                int yLoc = rnd.Next(3, 33) * 16;
                if (!randomYLocations.Contains(yLoc))
                {
                    randomYLocations.Add(yLoc);
                }
                else m_mushroomsDropped++;
            }
        }

        public void update(GameTime gameTime, Random rnd, List<Mushroom> mushrooms)
        {
            m_center.Y += 2;

            if (m_center.Y % 16 == 0)
            {
                if (randomYLocations.Contains((int)m_center.Y))
                {
                    bool exists = false;
                    foreach (var mushroom in mushrooms)
                    {
                        if (mushroom.m_xPos == m_center.X && mushroom.m_yPos == m_center.Y)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        mushrooms.Add(new Objects.Mushroom(
                            new Vector2(16, 16),
                            new Vector2(m_center.X, m_center.Y)
                        ));
                    }
                }
            }

            updateRectangle();
        }
        public void updateRectangle()
        {
            m_fleaRectangle = new Rectangle(
                (int)(m_center.X + (m_size.X / 2)), (int)(m_center.Y + (m_size.Y / 2)),  // Rectangle Location
                (int)m_size.X, (int)m_size.Y  // Rectangle Dimensions
            );
            m_spriteRectangle = m_fleaRectangle;
        }
    }
}
