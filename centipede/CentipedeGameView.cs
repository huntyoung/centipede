using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System;
using System.IO;
using Microsoft.Xna.Framework.Content;


namespace centipede
{
    public class CentipedeGameView : GameStateView
    {

        private GraphicsDeviceManager m_graphics;
        //private SpriteBatch m_spriteBatch;

        private List<Objects.Mushroom> m_mushrooms;
        private List<Objects.CentipedeSegment> m_segments;
        private AnimatedSprite m_headRenderer;
        private AnimatedSprite m_segmentRenderer;
        private Objects.Player m_player;
        private List<Objects.Spider> m_spiders;
        private AnimatedSprite m_spiderRenderer;
        private List<Objects.Scorpion> m_scorpions;
        private AnimatedSprite m_scorpionRenderer;
        private List<Objects.Flea> m_fleas;
        private AnimatedSprite m_fleaRenderer;

        private Texture2D m_spriteSheet;
        private SpriteFont m_font1;
        private Texture2D m_headerBox;
        private SoundEffect m_explosion1;
        private SoundEffect m_explosion2;
        private SoundEffect m_punch;
        private Song m_gunShot;
        private Song m_mushroomHit;

        private Random rnd = new Random();

        // changing game screen dimensions is not dynamic. Changing this will mess up the code :/
        private int m_gameWidth = 512;
        private int m_gameHeight = 544;

        private int m_segmentCount = 12;
        private int m_mushroomCount = 30;
        private float m_spiderElapsedTime;
        private float m_fleaElapsedTime;
        private float m_scorpionElapsedTime;
        private float m_playerElapsedTime;

        private bool m_highScoreAdded = false;

        public override void loadContent(ContentManager contentManager)
        {
            m_mushrooms = new List<Objects.Mushroom>();
            m_segments = new List<Objects.CentipedeSegment>();
            m_spiders = new List<Objects.Spider>();
            m_scorpions = new List<Objects.Scorpion>();
            m_fleas = new List<Objects.Flea>();

            //m_spriteBatch = new SpriteBatch(GraphicsDevice);
            m_spriteSheet = contentManager.Load<Texture2D>("images/spritesheet2");
            m_explosion1 = contentManager.Load<SoundEffect>("audio/explosion_1");
            m_explosion2 = contentManager.Load<SoundEffect>("audio/explosion_1");
            m_punch = contentManager.Load<SoundEffect>("audio/punch");
            m_gunShot = contentManager.Load<Song>("audio/gun_shot");
            m_mushroomHit = contentManager.Load<Song>("audio/mushroom_hit");

            m_font1 = contentManager.Load<SpriteFont>("Fonts/font1");
            //m_headerBox = new Texture2D(m_graphics.GraphicsDevice, 1, 1);
            //m_headerBox.SetData(new Color[] { Color.Black });

            m_segmentCount = 12;
            m_mushroomCount = 30;
            m_spiderElapsedTime = 0;
            m_fleaElapsedTime = 0;
            m_scorpionElapsedTime = 0;
            m_playerElapsedTime = 0;

        m_player = new Objects.Player(
                new Vector2(16, 16), // size
                new Vector2((m_gameWidth/2 - 8), m_gameHeight - 16) // location
            );

            m_player.loadAudio(m_explosion1, m_explosion2, m_punch, m_gunShot, m_mushroomHit);

            for (int i=0; i < m_mushroomCount; i++)
            {
                bool exists = false;
                int x = rnd.Next(32) * 16;
                int y = rnd.Next(3, 30) * 16;
                foreach (var mushroom in m_mushrooms)
                {
                    if (mushroom.m_xPos == x && mushroom.m_yPos == y)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    m_mushrooms.Add(new Objects.Mushroom(
                        new Vector2(16, 16),
                        new Vector2(x, y)
                    ));
                }
            }

            m_headRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 40, 40, 40, 40, 40, 40, 40, 40 },
                new Vector2(0, 0), // starting location on sheet
                new Vector2(8, 8),  // width and height of subimage on sheet
                true // multiple rows in spritesheet
            );

            m_segmentRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 40, 40, 40, 40, 40, 40, 40, 40 },
                new Vector2(0, 16), // starting location on sheet
                new Vector2(8, 8),  // width and height of subimage on sheet
                true // multiple rows in spritesheet
            );


            m_spiders.Add(new Objects.Spider(
                new Vector2(32, 16),  // size on screen
                new Vector2(-64, 450) // location on screen
            ));

            m_spiderRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 60, 60, 60, 60, 60, 60, 60, 60 },
                new Vector2(0, 32), // starting location on sheet
                new Vector2(16, 8),  // width and height of subimage on sheet
                true
            );

            m_scorpionRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 80, 80, 80, 80 },
                new Vector2(0, 56), // starting location on sheet
                new Vector2(16, 8),  // width and height of subimage on sheet
                false
            );

            m_fleaRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 80, 80, 80, 80 },
                new Vector2(64, 32), // starting location on sheet
                new Vector2(16, 8),  // width and height of subimage on sheet
                true
            );
        }

        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.GamePlay;
        }

        public override void update(GameTime gameTime)
        {
            m_headRenderer.update(gameTime);
            m_segmentRenderer.update(gameTime);
            m_spiderRenderer.update(gameTime);
            m_scorpionRenderer.update(gameTime);
            m_fleaRenderer.update(gameTime);

            if (!m_player.m_isDead && m_player.m_livesLeft > 0)
            {
                foreach (var spider in m_spiders) spider.update(gameTime, rnd);
                foreach (var flea in m_fleas.ToList())
                {
                    flea.update(gameTime, rnd, m_mushrooms);
                    if (flea.Center.Y > m_gameHeight + flea.Size.Y) m_fleas.Remove(flea);
                }
                foreach (var scorpion in m_scorpions.ToList())
                {
                    scorpion.scorpionCollision(m_mushrooms);
                    scorpion.update(gameTime);
                    if (scorpion.Center.X > m_gameWidth + scorpion.Size.X) m_scorpions.Remove(scorpion);
                }
                foreach (var segment in m_segments)
                {
                    segment.segmentCollision(m_mushrooms);
                    segment.update(gameTime);
                }
                m_player.update(gameTime);
                m_player.laserCollision(m_segments, m_mushrooms, m_spiders, m_fleas, m_scorpions);
                m_player.playerCollision(m_segments, m_mushrooms, m_spiders, m_fleas, m_scorpions);

                if (m_segments.Count == 0)
                {
                    Objects.CentipedeSegment prevSegment = null;
                    for (int i = 0; i < m_segmentCount; i++)
                    {
                        Objects.CentipedeSegment segment = new Objects.CentipedeSegment(
                            new Vector2(16, 16),  // size on screen
                            new Vector2((16 * i) - (16 * (m_segmentCount * 2)), 32) // location on screen
                        );

                        segment.m_childSegment = prevSegment;
                        if (prevSegment != null) prevSegment.m_parentSegment = segment;
                        if (i == m_segmentCount - 1)
                        {
                            segment.m_isHead = true;
                            segment.m_parentSegment = null;
                        }

                        prevSegment = segment;

                        m_segments.Add(segment);
                    }
                }

                if (m_spiders.Count == 0)
                {
                    m_spiderElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (m_spiderElapsedTime >= 5f)  // spider respawns after 5 seconds
                    {
                        m_spiders.Add(new Objects.Spider(
                            new Vector2(32, 16),  // size on screen
                            new Vector2(-64, 450) // location on screen
                        ));
                        m_spiderElapsedTime = 0;
                    }
                }


                m_fleaElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_fleaElapsedTime >= 10f) // flea comes every 10 seconds
                {
                    int x = rnd.Next(32) * 16;
                    m_fleas.Add(new Objects.Flea(
                        new Vector2(32, 16),  // size on screen
                        new Vector2(x, -16) // location on screen
                    ));
                    float carryOverTime = m_fleaElapsedTime - 10f;
                    m_fleaElapsedTime = carryOverTime;
                }

                m_scorpionElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_scorpionElapsedTime >= 20f)  // scorpion comes every 20 sec
                {
                    int y = rnd.Next(3, 28) * 16;
                    m_scorpions.Add(new Objects.Scorpion(
                        new Vector2(32, 16),  // size on screen
                        new Vector2(-32, y) // location on screen
                    ));
                    float carryOverTime = m_scorpionElapsedTime - 20f;
                    m_scorpionElapsedTime = carryOverTime;
                }
            } 
            else
            {
                m_playerElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (m_playerElapsedTime >= 2f)  // player comes back after 2 seconds
                {
                    m_player.m_xPos = (m_gameWidth / 2) - 8;
                    m_player.m_yPos = m_gameHeight - (int)m_player.m_playerSize.Y;
                    m_playerElapsedTime = 0;
                    m_player.m_isDead = false;
                    m_player.m_isInvincible = true;
                }
            }
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            if (m_player.m_livesLeft > 0)
            {
                m_player.draw(m_spriteBatch, m_spriteSheet, new Vector2(0, 80), new Vector2(8, 8));

                foreach (var segment in m_segments)
                {
                    if (segment.m_isHead) m_headRenderer.draw(m_spriteBatch, segment);
                    else m_segmentRenderer.draw(m_spriteBatch, segment);
                }
                foreach (var mushroom in m_mushrooms)
                {
                    mushroom.draw(m_spriteBatch, m_spriteSheet, new Vector2(64, 0), new Vector2(8, 8));
                }
                foreach (var flea in m_fleas) m_fleaRenderer.draw(m_spriteBatch, flea);
                foreach (var spider in m_spiders) m_spiderRenderer.draw(m_spriteBatch, spider);
                foreach (var scorpion in m_scorpions) m_scorpionRenderer.draw(m_spriteBatch, scorpion);

                //m_spriteBatch.Draw(m_headerBox, new Rectangle(0, 0, m_gameWidth, 32), Color.Black);
                m_spriteBatch.DrawString(m_font1, "SCORE: " + m_player.m_score.ToString(), new Vector2(5, 0), Color.White);

                Vector2 stringSize = m_font1.MeasureString("LIVES: " + m_player.m_livesLeft.ToString());
                m_spriteBatch.DrawString(m_font1, "LIVES: " + m_player.m_livesLeft.ToString(),
                    new Vector2(m_gameWidth - stringSize.X - 5, 0), Color.White);
            } 
            else
            {
                string gameOver = "GAME OVER";
                Vector2 stringSize = m_font1.MeasureString(gameOver);
                m_spriteBatch.DrawString(m_font1, gameOver,
                    new Vector2(m_gameWidth / 2 - stringSize.X / 2, (m_gameHeight / 2 - stringSize.Y / 2) - stringSize.Y), Color.Red);
                string score = "SCORE: " + m_player.m_score.ToString();
                Vector2 scoreSize = m_font1.MeasureString(score);
                m_spriteBatch.DrawString(m_font1, score,
                    new Vector2(m_gameWidth / 2 - scoreSize.X / 2, (m_gameHeight / 2 - scoreSize.Y / 2)), Color.White);


                //if (!m_highScoreAdded)
                //{
                //    using (StreamWriter sw = new StreamWriter("C:\Users\hunty\CS5410--VideoGameDev\centipede\centipede\highScores.txt"))
                //    {
                //        sw.WriteLine(m_player.m_score.ToString());
                //    }
                //    m_highScoreAdded = true;
                //}
                
            }

            m_spriteBatch.End();
        }
    }
}
