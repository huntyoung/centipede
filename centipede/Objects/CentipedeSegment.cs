using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace centipede.Objects
{
    class CentipedeSegment : AnimatedSprite
    {
        public bool m_isHead { get; set; }
        public CentipedeSegment m_childSegment { get; set; }
        public CentipedeSegment m_parentSegment { get; set; }

        public List<Mushroom> m_ignoredMushrooms = new List<Mushroom>();

        public bool m_goingRight { get; set; }
        private bool m_goingDown = true;
        private bool m_continueGoing = true;
        private int m_speed = 4;
        private int m_framesToFlip;  // number of frames it will take to go down and rotate based on speed
        private int m_frameCount = 0;

        private bool m_mushroomCollision = false;

        private int m_gameWidth = 512;
        private int m_gameHeight = 512;

        public Rectangle m_segmentRectangle { get; set; }


        public CentipedeSegment(Vector2 size, Vector2 center) : base(size, center)
        {
            m_rotation = MathHelper.ToRadians(180); // 180 degrees
            m_framesToFlip = (int)(m_size.X / m_speed);

            m_goingRight = true;
        }

        public void update(GameTime gameTime)
        {
            if (m_continueGoing)
            {
                if (m_goingRight)
                {
                    if (m_center.X + m_speed > m_gameWidth - m_size.X)
                    {
                        m_center.X = m_gameWidth - m_size.X;
                    }
                    else m_center.X += m_speed;
                }
                else
                {
                    if (m_center.X - m_speed < 0)
                    {
                        m_center.X = 0;
                    }
                    else m_center.X -= m_speed;
                }
            }

            if (m_center.Y == 0) m_goingDown = true;
            else if (m_center.Y == m_gameHeight - m_size.Y) m_goingDown = false;

            if (m_center.X <= 0 || (!m_goingRight && m_mushroomCollision)) // hits left wall or mushroom going left
            {
                if (m_frameCount < m_framesToFlip)
                {
                    if (m_goingDown)
                    {
                        m_rotation += MathHelper.ToRadians(-1 * (float)(180 / m_framesToFlip)); // flip 180 degrees over frame count of framesToFlip
                        m_center.Y += (float)(m_size.Y / m_framesToFlip);
                    }
                    else
                    {
                        m_rotation += MathHelper.ToRadians((float)(180 / m_framesToFlip)); // flip 180 degrees over frame count of framesToFlip
                        m_center.Y -= (float)(m_size.Y / m_framesToFlip);
                    }

                    m_frameCount++;
                    m_continueGoing = false;
                }
                else
                {
                    m_goingRight = true;
                    m_continueGoing = true;
                    m_mushroomCollision = false;
                    m_frameCount = 0;
                }
            }
            else if (m_center.X + m_size.X >= m_gameWidth || (m_goingRight && m_mushroomCollision)) // hits right wall
            {
                if (m_frameCount < m_framesToFlip)
                {
                    if (m_goingDown)
                    {
                        m_rotation += MathHelper.ToRadians((float)(180 / m_framesToFlip)); // flip 180 degrees over frame count of framesToFlip
                        m_center.Y += (float)(m_size.Y / m_framesToFlip);
                    }
                    else
                    {
                        m_rotation += MathHelper.ToRadians(-1 * (float)(180 / m_framesToFlip)); // flip 180 degrees over frame count of framesToFlip
                        m_center.Y -= (float)(m_size.Y / m_framesToFlip);
                    }

                    m_frameCount++;
                    m_continueGoing = false;
                }
                else
                {
                    m_goingRight = false;
                    m_continueGoing = true;
                    m_mushroomCollision = false;
                    m_frameCount = 0;
                }
            }

            updateRectangle();
        }

        public void updateRectangle()
        {
            m_segmentRectangle = new Rectangle(
                (int)(m_center.X + (m_size.X / 2)), (int)(m_center.Y + (m_size.Y / 2)),  // Rectangle Location
                (int)m_size.X, (int)m_size.Y  // Rectangle Dimensions
            );
            m_spriteRectangle = m_segmentRectangle;
        }


        public void segmentCollision(List<Mushroom> mushrooms)
        {   
            foreach (var mushroom in mushrooms)
            {
                Rectangle newMushroomRectangle = new Rectangle(
                    (int)(mushroom.m_xPos + (mushroom.m_mushroomSize.X / 2)) - 1, (int)(mushroom.m_yPos + (mushroom.m_mushroomSize.Y / 2)),
                    (int)mushroom.m_mushroomSize.X + 2, (int)mushroom.m_mushroomSize.Y
                );
                if (m_segmentRectangle.Intersects(newMushroomRectangle))
                {
                    if (m_continueGoing && !m_ignoredMushrooms.Contains(mushroom))
                    {
                        m_mushroomCollision = true;
                    } 
                    else
                    {
                        if (!m_ignoredMushrooms.Contains(mushroom))
                        {
                            m_ignoredMushrooms.Add(mushroom);
                        }
                    }
                    break;
                }
                else // not intersecting
                {
                    if (m_ignoredMushrooms.Contains(mushroom))
                    {
                        m_ignoredMushrooms.Remove(mushroom);
                    }
                }
                //if (m_segmentRectangle.Intersects(newMushroomRectangle) && m_goingRight)
                //{
                //    m_mushroomCollision = true;
                //    break;
                //}
            }
        }
    }
}
