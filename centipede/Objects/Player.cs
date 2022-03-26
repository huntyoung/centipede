using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace centipede.Objects
{
    class Player
    {
        private List<Laser> m_lasers = new List<Laser>();

        public Vector2 m_playerSize { get; set; }
        private int m_playerSpeed = 3;

        public int m_xPos { get; set; }
        public int m_yPos { get; set; }

        private int m_gameWidth = 512;
        private int m_gameHeight = 544;

        private bool shotAvailable = true;
        private float elapsedTime;

        private bool movingLeft = false;
        private bool movingRight = false;
        private bool movingUp = false;
        private bool movingDown = false;

        private SoundEffect m_explosion1;
        private SoundEffect m_explosion2;
        private SoundEffect m_punch;
        private Song m_gunShot;
        private Song m_mushroomHit;

        public bool m_isDead { get; set; }
        public bool m_isInvincible { get; set; }
        private float m_invincibleElapsedTime;
        public int m_score { get; set; }
        public int m_livesLeft { get; set; }

        public Rectangle m_playerRectangle { get; set; }

        public Player(Vector2 size, Vector2 position)
        {
            this.m_playerSize = size;
            this.m_xPos = (int)position.X;
            this.m_yPos = (int)position.Y;

            this.m_isDead = false;
            this.m_isInvincible = false;
            this.m_score = 0;
            this.m_livesLeft = 3;
        }

        public void loadAudio(SoundEffect explosion1, SoundEffect explosion2, SoundEffect punch, Song gunShot, Song mushroomHit)
        {
            this.m_explosion1 = explosion1;
            this.m_explosion2 = explosion2;
            this.m_punch = punch;
            this.m_gunShot = gunShot;
            this.m_mushroomHit = mushroomHit;
        }

        public void update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            movingUp = movingLeft = movingDown = movingRight = false;
            if (state.IsKeyDown(Keys.Up) && m_yPos > m_gameHeight - (m_gameHeight / 4))  // up
            {
                m_yPos -= m_playerSpeed;
                movingUp = true;
            }
            if (state.IsKeyDown(Keys.Left) && m_xPos > 0)  // left
            {
                m_xPos -= m_playerSpeed;
                movingLeft = true;
            }
            if (state.IsKeyDown(Keys.Down) && m_yPos < m_gameHeight - m_playerSize.Y)  // down
            {
                m_yPos += m_playerSpeed;
                movingDown = true;
            }
            if (state.IsKeyDown(Keys.Right) && m_xPos < m_gameWidth - m_playerSize.X)  // right
            {
                m_xPos += m_playerSpeed;
                movingRight = true;
            }
            if (state.IsKeyDown(Keys.Space) && shotAvailable)
            {
                MediaPlayer.Play(m_gunShot);
                m_lasers.Add(new Laser(
                    new Vector2(8, 16), // size
                    new Vector2(m_xPos + 8, m_yPos) // location
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

            if (m_isInvincible)
            {
                m_invincibleElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_invincibleElapsedTime >= 2f)  // 2 seconds of invinciblity after game starts again
                {
                    m_isInvincible = false;
                    m_invincibleElapsedTime = 0;
                }
            }

            m_playerRectangle = new Rectangle(
                (int)(m_xPos + (m_playerSize.X / 2)), (int)(m_yPos + (m_playerSize.Y / 2)),
                (int)m_playerSize.X, (int)m_playerSize.Y
            );
        }

        public void draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 locationOnSheet, Vector2 subImDimensions)
        {
            if (m_isDead) return;

            spriteBatch.Draw(
                spriteSheet,
                m_playerRectangle, // Destination rectangle
                new Rectangle((int)locationOnSheet.X, (int)locationOnSheet.Y, (int)subImDimensions.X, (int)subImDimensions.Y), // Source sub-texture
                Color.White,
                0, // Angular rotation
                new Vector2(subImDimensions.X / 2, subImDimensions.Y / 2), // Center point of rotation
                SpriteEffects.None, 0);

            foreach (var laser in m_lasers)
            {
                laser.draw(spriteBatch, spriteSheet, new Vector2(locationOnSheet.X + 10, locationOnSheet.Y), new Vector2(4, 8));
            }
        }

        public void laserCollision(List<CentipedeSegment> segments, List<Mushroom> mushrooms, List<Spider> spiders, List<Flea> fleas, List<Scorpion> scorpions)
        {
            foreach (var laser in m_lasers.ToList())
            {
                foreach (var mushroom in mushrooms.ToList())
                {
                    if (laser.m_laserRectangle.Intersects(mushroom.m_mushroomRectangle))
                    {
                        MediaPlayer.Play(m_mushroomHit);
                        m_score += 4;
                        m_lasers.Remove(laser);
                        mushroom.m_deteriorationState++;
                        if (mushroom.m_deteriorationState > 3) mushrooms.Remove(mushroom);
                        break;
                    }
                }
                foreach (var segment in segments.ToList())
                {
                    if (laser.m_laserRectangle.Intersects(segment.m_segmentRectangle))
                    {
                        m_explosion1.Play();
                        if (segment.m_isHead) m_score += 100;
                        else m_score += 10;
                        m_lasers.Remove(laser);
                        if (segment.m_childSegment != null) segment.m_childSegment.m_isHead = true;

                        int x;
                        if (segment.m_goingRight)  // if going right place mushroom to next square
                        {
                            x = (int)segment.Center.X;
                             if (x % (int)m_playerSize.X != 0)
                            {
                                int diffToNextSquareX = (int)m_playerSize.X - (x % (int)m_playerSize.X);
                                x = x + diffToNextSquareX;
                            }
                        }
                        else  // if going left place mushroom to previous square
                        {
                            x = (int)segment.Center.X;
                            x = x - (x % (int)m_playerSize.X);
                        }

                        int y = (int)segment.Center.Y;
                        if (y % (int)m_playerSize.Y != 0)
                        {
                            int diffToNextSquareY = (int)m_playerSize.Y - (y % (int)m_playerSize.Y);
                            y = y + diffToNextSquareY;
                        }

                        Mushroom newMushroom = new Mushroom(
                            segment.Size,
                            new Vector2(x, y)
                        );
                        mushrooms.Add(newMushroom);
                        if (segment.m_parentSegment != null)
                            segment.m_parentSegment.m_ignoredMushrooms.Add(newMushroom);

                        segments.Remove(segment);
                        break;
                    }
                }
                foreach (var spider in spiders.ToList()) {
                    if (laser.m_laserRectangle.Intersects(spider.m_spiderRectangle))
                    {
                        m_punch.Play();
                        m_score += 300;
                        m_lasers.Remove(laser);
                        spiders.Remove(spider);
                        break;
                    }
                }
                foreach (var flea in fleas.ToList())
                {
                    if (laser.m_laserRectangle.Intersects(flea.m_fleaRectangle))
                    {
                        m_punch.Play();
                        m_score += 200;
                        m_lasers.Remove(laser);
                        fleas.Remove(flea);
                        break;
                    }
                }
                foreach (var scorpion in scorpions.ToList())
                {
                    if (laser.m_laserRectangle.Intersects(scorpion.m_scorpionRectangle))
                    {
                        m_punch.Play();
                        m_score += 1000;
                        m_lasers.Remove(laser);
                        scorpions.Remove(scorpion);
                        break;
                    }
                }
            }
        }

        public void playerCollision(List<CentipedeSegment> segments, List<Mushroom> mushrooms, List<Spider> spiders, List<Flea> fleas, List<Scorpion> scorpions)
        {
            foreach (var mushroom in mushrooms.ToList())
            {
                if (m_playerRectangle.Intersects(mushroom.m_mushroomRectangle))  // intersecting
                {
                    Rectangle intersectingRectangle = Rectangle.Intersect(m_playerRectangle, mushroom.m_mushroomRectangle);

                    if (movingLeft) m_xPos += intersectingRectangle.Width;
                    else if (movingRight) m_xPos -= intersectingRectangle.Width;
                    else if (movingUp) m_yPos += intersectingRectangle.Height;
                    else if (movingDown) m_yPos -= intersectingRectangle.Height;
                }
            }

            foreach (var segment in segments.ToList())
            {
                if (m_playerRectangle.Intersects(segment.m_segmentRectangle) && !m_isInvincible)
                {
                    if (!m_isDead)
                    {
                        m_livesLeft--;
                        m_explosion2.Play();
                    }
                    m_isDead = true;
                    break;
                }
            }
            foreach (var spider in spiders.ToList())
            {
                if (m_playerRectangle.Intersects(spider.m_spiderRectangle) && !m_isInvincible)
                {
                    if (!m_isDead)
                    {
                        m_livesLeft--;
                        m_explosion2.Play();
                    }
                    m_isDead = true;
                    break;
                }
            }
            foreach (var flea in fleas.ToList())
            {
                if (m_playerRectangle.Intersects(flea.m_fleaRectangle) && !m_isInvincible)
                {
                    if (!m_isDead)
                    {
                        m_livesLeft--;
                        m_explosion2.Play();
                    }
                    m_isDead = true;
                    break;
                }
            }
            foreach (var scorpion in scorpions.ToList())
            {
                if (m_playerRectangle.Intersects(scorpion.m_scorpionRectangle) && !m_isInvincible)
                {
                    if (!m_isDead)
                    {
                        m_livesLeft--;
                        m_explosion2.Play();
                    }
                    m_isDead = true;

                    break;
                }
            }
        }
    }
}
