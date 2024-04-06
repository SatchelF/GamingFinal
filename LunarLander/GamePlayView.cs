using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using CS5410.Input;

namespace CS5410
{
    public class GamePlayView : GameStateView
    {
        #region Fields

        private SpriteFont m_font;
        private int screenHeight;
        private int screenWidth;
        private MyRandom profsRand = new MyRandom(); // Use your custom MyRandom class
        private ParticleSystem thrustParticleSystem;
        private ParticleSystem explosionParticleSystem;
        private ParticleSystemRenderer thrustRenderer;
        private ParticleSystemRenderer explosionRenderer;
        private KeyboardInput keyboardInput;
        private const string MESSAGE = "Game Play View";




        #endregion

        #region Game Loop
        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/menu");
            InitializeNewGame();
        }



        private void InitializeNewGame()
        {
            string settingsFilePath = "gameSettings.json";
            GameSettings settings = SettingsView.LoadSettings(settingsFilePath);

            // Create the KeyboardInput instance
            keyboardInput = new KeyboardInput();
            screenHeight = m_graphics.PreferredBackBufferHeight;
            screenWidth = m_graphics.PreferredBackBufferWidth;
            
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
        }

        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();

            Vector2 stringSize = m_font.MeasureString(MESSAGE);
            m_spriteBatch.DrawString(m_font, MESSAGE,
                new Vector2(m_graphics.PreferredBackBufferWidth / 2 - stringSize.X / 2, m_graphics.PreferredBackBufferHeight / 2 - stringSize.Y), Color.Yellow);

            m_spriteBatch.End();
        }

        #endregion

    }







}