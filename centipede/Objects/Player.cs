using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace centipede.Objects
{
    class IntersectingMushroom
    {
        public Mushroom mushroom { get; set; }
        public string intersectDirection { get; set; }
        public IntersectingMushroom(Mushroom mushroom, string intersectDirection)
        {
            this.mushroom = mushroom;
            this.intersectDirection = intersectDirection;
        }
    }
    class Player
    {
        private List<Laser> m_lasers = new List<Laser>();
        private List<IntersectingMushroom> m_intersectingMushrooms = new List<IntersectingMushroom>();

        private Vector2 m_playerSize;
        private int m_playerSpeed = 3;

        private int m_xPos;
        private int m_yPos;

        private int m_gameWidth = 512;
        private int m_gameHeight = 512;

        private bool shotAvailable = true;
        private float elapsedTime;

        private bool movingLeft = false;
        private bool movingRight = false;
        private bool movingUp = false;
        private bool movingDown = false;

        private bool stoppedLeft = false;
        private bool stoppedRight = false;
        private bool stoppedTop = false;
        private bool stoppedBottom = false;

        public Rectangle m_playerRectangle { get; set; }


        public Player(Vector2 size, Vector2 position)
        {
            this.m_playerSize = size;
            this.m_xPos = (int)position.X;
            this.m_yPos = (int)position.Y;
        }

        public void update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            movingUp = movingLeft = movingDown = movingRight = false;
            if (state.IsKeyDown(Keys.W) && m_yPos > m_gameHeight - (m_gameHeight / 4) && !stoppedTop)  // up
            {
                m_yPos -= m_playerSpeed;
                movingUp = true;
            }
            if (state.IsKeyDown(Keys.A) && m_xPos > 0 && !stoppedLeft)  // left
            {
                m_xPos -= m_playerSpeed;
                movingLeft = true;
            }
            if (state.IsKeyDown(Keys.S) && m_yPos < m_gameHeight - m_playerSize.Y && !stoppedBottom)  // down
            {
                m_yPos += m_playerSpeed;
                movingDown = true;
            }
            if (state.IsKeyDown(Keys.D) && m_xPos < m_gameWidth - m_playerSize.X && !stoppedRight)  // right
            {
                m_xPos += m_playerSpeed;
                movingRight = true;
            }
            if (state.IsKeyDown(Keys.Space) && shotAvailable)
            {
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

            m_playerRectangle = new Rectangle(
                (int)(m_xPos + (m_playerSize.X / 2)), (int)(m_yPos + (m_playerSize.Y / 2)),
                (int)m_playerSize.X, (int)m_playerSize.Y
            );
        }

        public void draw(SpriteBatch spriteBatch, Texture2D spriteSheet, Vector2 locationOnSheet, Vector2 subImDimensions)
        {
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

        public void laserCollision(List<CentipedeSegment> segments, List<Mushroom> mushrooms, List<Spider> spiders)
        {
            foreach (var laser in m_lasers.ToList())
            {
                foreach (var mushroom in mushrooms.ToList())
                {
                    if (laser.m_laserRectangle.Intersects(mushroom.m_mushroomRectangle))
                    {
                        m_lasers.Remove(laser);
                        mushroom.deteriorationState++;
                        if (mushroom.deteriorationState > 3) mushrooms.Remove(mushroom);
                        break;
                    }
                }
                foreach (var segment in segments.ToList())
                {
                    if (laser.m_laserRectangle.Intersects(segment.m_segmentRectangle))
                    {
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
                        m_lasers.Remove(laser);
                        spiders.Remove(spider);
                        break;
                    }
                }
            }
        }

        public void playerCollision(List<CentipedeSegment> segments, List<Mushroom> mushrooms, List<Spider> spiders, List<Flea> fleas, List<Scorpion> scorpions)
        {
            foreach (var mushroom in mushrooms.ToList())
            {
                //Rectangle newMushroomRectangle = new Rectangle(
                //    (int)(mushroom.m_xPos + (mushroom.m_mushroomSize.X / 2)) - 2, (int)(mushroom.m_yPos + (mushroom.m_mushroomSize.Y / 2)) - 1,
                //    (int)mushroom.m_mushroomSize.X + 2, (int)mushroom.m_mushroomSize.Y + 4
                //);
                if (m_playerRectangle.Intersects(mushroom.m_mushroomRectangle))  // intersecting
                {

                    bool addMushroom = true;
                    foreach (var intersectingMushroom in m_intersectingMushrooms)
                    {
                        if (intersectingMushroom.mushroom == mushroom) {
                            addMushroom = false;
                            if (intersectingMushroom.intersectDirection == "r") stoppedRight = true;
                            else if (intersectingMushroom.intersectDirection == "l") stoppedLeft = true;
                            else if (intersectingMushroom.intersectDirection == "d") stoppedBottom = true;
                            else if (intersectingMushroom.intersectDirection == "u") stoppedTop = true;
                        }
                    }

                    if (addMushroom)  // not in intersectingMushrooms
                    {
                        string direction = "";
                        if (movingRight) direction = "r";
                        if (movingLeft) direction = "l";
                        if (movingDown) direction = "d";
                        if (movingUp) direction = "u";
                        m_intersectingMushrooms.Add(new IntersectingMushroom(mushroom, direction));
                    }
                }
                else  // not intersecting
                {
                    foreach (var intersectingMushroom in m_intersectingMushrooms.ToList())
                    {
                        if (intersectingMushroom.mushroom == mushroom)
                        {
                            if (intersectingMushroom.intersectDirection == "r") stoppedRight = false;
                            else if (intersectingMushroom.intersectDirection == "l") stoppedLeft = false;
                            else if (intersectingMushroom.intersectDirection == "d") stoppedBottom = false;
                            else if (intersectingMushroom.intersectDirection == "u") stoppedTop = false;

                            m_intersectingMushrooms.Remove(intersectingMushroom);
                        }
                    }
                }
            }

            foreach (var segment in segments.ToList())
            {
                if (m_playerRectangle.Intersects(segment.m_segmentRectangle))
                {
                    m_playerSize = new Vector2(0, 0);
                    break;
                }
            }
            foreach (var spider in spiders.ToList())
            {
                if (m_playerRectangle.Intersects(spider.m_spiderRectangle))
                {
                    m_playerSize = new Vector2(0, 0);
                    break;
                }
            }
            foreach (var flea in fleas.ToList())
            {
                if (m_playerRectangle.Intersects(flea.m_fleaRectangle))
                {
                    m_playerSize = new Vector2(0, 0);
                    break;
                }
            }
            foreach (var scorpion in scorpions.ToList())
            {
                if (m_playerRectangle.Intersects(scorpion.m_scorpionRectangle))
                {
                    m_playerSize = new Vector2(0, 0);
                    break;
                }
            }
        }
    }
}
