﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;


namespace centipede.Objects
{
    class Flea : AnimatedSprite
    {
        public Rectangle m_fleaRectangle { get; set; }

        public Flea(Vector2 size, Vector2 center) : base(size, center)
        {
            //m_moveRate = moveRate;
            //m_rotateRate = rotateRate;
        }

        public void update(GameTime gameTime)
        {
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
