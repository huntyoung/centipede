using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace centipede
{
    class ControlsView : GameStateView
    {
        public override void loadContent(ContentManager contentManager)
        {

        }
        public override GameStateEnum processInput(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                return GameStateEnum.MainMenu;
            }

            return GameStateEnum.Controls;
        }
        public override void render(GameTime gameTime)
        {

        }
        public override void update(GameTime gameTime)
        {

        }
    }
}
