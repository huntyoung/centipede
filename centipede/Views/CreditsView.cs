using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace centipede
{
    class CreditsView : GameStateView
    {
        private SpriteFont m_font1;
        public override void loadContent(ContentManager contentManager)
        {
            m_font1 = contentManager.Load<SpriteFont>("Fonts/font1");
        }
        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.Credits;
        }
        public override void update(GameTime gameTime)
        {

        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            string text = "Credit for this game goes to\n Hunter Young and Dean Mathias";
            Vector2 stringSize = m_font1.MeasureString(text);
            m_spriteBatch.DrawString(
                m_font1,
                text,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y / 2),
                Color.White);

            m_spriteBatch.End();
        }
        
    }
}
