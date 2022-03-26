using Microsoft.Xna.Framework;

namespace centipede.Objects
{
    public class AnimatedSprite
    {
        protected Vector2 m_size;
        protected Vector2 m_center;
        protected float m_rotation = 0;
        protected Rectangle m_spriteRectangle;

        public AnimatedSprite(Vector2 size, Vector2 center)
        {
            m_size = size;
            m_center = center;   
        }

        public Rectangle SpriteRectangle
        {
            get { return m_spriteRectangle; }
            set { m_spriteRectangle = value; }
        }

        public Vector2 Size
        {
            get { return m_size; }
        }

        public Vector2 Center
        {
            get { return m_center; }
        }

        public float Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }
    }
}
