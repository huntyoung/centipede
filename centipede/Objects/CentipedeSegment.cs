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

        private bool m_goingRight = true;
        private bool m_continueGoing = true;
        private int m_speed = 3;
        private float m_framesToFlip;  // number of frames it will take to go down and rotate based on speed
        private float m_distanceGoneY = 0;
        private float m_rotationGone = 0;


        private int m_gameWidth = 512;
        private int m_gameHeight = 512;

        public CentipedeSegment(Vector2 size, Vector2 center) : base(size, center)
        {
            m_rotation = MathHelper.ToRadians(180); // 180 degrees
            m_framesToFlip = m_size.X / m_speed;
            //m_moveRate = moveRate;
            //m_rotateRate = rotateRate;
        }

        public void update(GameTime gameTime)
        {
            if (m_continueGoing)
            {
                if (m_goingRight) m_center.X += m_speed;
                else m_center.X -= m_speed;
            }
            
            if (m_center.X <= 0) // hits left wall
            {
                if (m_distanceGoneY < m_size.Y && m_rotationGone > MathHelper.ToRadians(-180))
                {
                    float rotationAmount = MathHelper.ToRadians(-1 * (float)(180 / m_framesToFlip)); // flip 180 degrees over frame count of framesToFlip
                    m_rotation += rotationAmount;
                    m_rotationGone += rotationAmount;

                    float yDistAmount = (float)(m_size.Y / m_framesToFlip);
                    m_center.Y += yDistAmount;
                    m_distanceGoneY += yDistAmount;
                    m_continueGoing = false;
                }
                else
                {
                    m_goingRight = true;
                    m_continueGoing = true;
                    m_rotationGone = 0;
                    m_distanceGoneY = 0;
                }
            }
            else if (m_center.X + m_size.X >= m_gameWidth) // hits right wall
            {
                if (m_distanceGoneY < m_size.Y && m_rotationGone < MathHelper.ToRadians(180))
                {
                    float rotationAmount = MathHelper.ToRadians((float)(180 / m_framesToFlip)); // flip 180 degrees over frame count of framesToFlip
                    m_rotation += rotationAmount;
                    m_rotationGone += rotationAmount;

                    float yDistAmount = (float)(m_size.Y / m_framesToFlip);
                    m_center.Y += yDistAmount;
                    m_distanceGoneY += yDistAmount;
                    m_continueGoing = false;
                }
                else
                {
                    m_goingRight = false;
                    m_continueGoing = true;
                    m_rotationGone = 0;
                    m_distanceGoneY = 0;
                }
            }
        }

    }
}
