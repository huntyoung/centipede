using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;


namespace centipede
{
    public class CentipedeGame : Game
    {

        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;

        private List<Objects.Mushroom> m_mushrooms = new List<Objects.Mushroom>();
        private List<Objects.CentipedeSegment> m_segments = new List<Objects.CentipedeSegment>();
        private AnimatedSprite m_headRenderer;
        private AnimatedSprite m_segmentRenderer;
        private Objects.Player m_player;
        private Objects.Spider m_spider;
        private AnimatedSprite m_spiderRenderer;
        private Objects.Scorpion m_scorpion;
        private AnimatedSprite m_scorpionRenderer;
        private Objects.Flea m_flea;
        private AnimatedSprite m_fleaRenderer;

        private Texture2D m_spriteSheet;

        private Random rnd = new Random();

        private int m_gameWidth = 512;
        private int m_gameHeight = 512;

        public CentipedeGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            m_graphics.PreferredBackBufferWidth = m_gameWidth;
            m_graphics.PreferredBackBufferHeight = m_gameHeight;

            m_graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            m_spriteSheet = Content.Load<Texture2D>("images/spritesheet2");

            m_player = new Objects.Player(
                new Vector2(16, 16), // size
                new Vector2(m_gameWidth/2, m_gameHeight - 16) // location
            );

            for (int i=0; i < 30; i++)
            {
                bool exists = false;
                int x = rnd.Next(32) * 16;
                int y = rnd.Next(1, 28) * 16;
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

            Objects.CentipedeSegment prevSegment;
            for (int i=0; i<8; i++)
            {
                Objects.CentipedeSegment segment = new Objects.CentipedeSegment(
                    new Vector2(16, 16),  // size on screen
                    new Vector2(16 * i, 0) // location on screen
                );
                prevSegment = segment;

                if (i == 0) segment.m_childSegment = null;
                if (i > 0) segment.m_childSegment = prevSegment;
                if (i == 7) segment.m_isHead = true;

                m_segments.Add(segment);
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


            m_spider = new Objects.Spider(
                new Vector2(32, 16),  // size on screen
                new Vector2(-64, 450) // location on screen
            );

            m_spiderRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 60, 60, 60, 60, 60, 60, 60, 60 },
                new Vector2(0, 32), // starting location on sheet
                new Vector2(16, 8),  // width and height of subimage on sheet
                true
            );


            m_scorpion = new Objects.Scorpion(
                new Vector2(32, 16),  // size on screen
                new Vector2(200, 50) // location on screen
            );

            m_scorpionRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 80, 80, 80, 80 },
                new Vector2(0, 56), // starting location on sheet
                new Vector2(16, 8),  // width and height of subimage on sheet
                false
            );


            m_flea = new Objects.Flea(
                new Vector2(32, 16),  // size on screen
                new Vector2(200, 200) // location on screen
            );

            m_fleaRenderer = new AnimatedSprite(
                m_spriteSheet,
                new int[] { 80, 80, 80, 80 },
                new Vector2(64, 32), // starting location on sheet
                new Vector2(16, 8),  // width and height of subimage on sheet
                true
            );

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            m_headRenderer.update(gameTime);
            m_segmentRenderer.update(gameTime);
            m_spider.update(gameTime, rnd);
            m_spiderRenderer.update(gameTime);
            m_scorpionRenderer.update(gameTime);
            m_fleaRenderer.update(gameTime);

            m_player.update(gameTime);

            foreach (var segment in m_segments)
            {
                segment.update(gameTime);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            m_spriteBatch.Begin();

            m_spiderRenderer.draw(m_spriteBatch, m_spider);
            m_scorpionRenderer.draw(m_spriteBatch, m_scorpion);
            m_fleaRenderer.draw(m_spriteBatch, m_flea);

            m_player.draw(m_spriteBatch, m_spriteSheet, new Vector2(0, 80), new Vector2(8, 8));

            foreach (var mushroom in m_mushrooms)
            {
                mushroom.draw(m_spriteBatch, m_spriteSheet, new Vector2(64, 0), new Vector2(8,8));
            }
            foreach (var segment in m_segments)
            {
                if (segment.m_isHead) m_headRenderer.draw(m_spriteBatch, segment);
                else m_segmentRenderer.draw(m_spriteBatch, segment);
            }

            m_spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
