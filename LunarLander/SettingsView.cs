﻿using CS5410.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization.Json;

namespace CS5410
{
    public class SettingsView : GameStateView
    {
        private SpriteFont m_font;
        private SpriteFont m_font2;
        private const string MESSAGE = "Press Enter to change the control, ESC to return, and up and down arrows to navigate";
        private Dictionary<string, Keys> keyBindings;
        private enum SettingsState { Viewing, Changing }
        private SettingsState currentState = SettingsState.Viewing;
        private string changingControl = "";
        private List<string> controlNames; // To iterate through controls
        private int selectedControlIndex = 0;
        private KeyboardInput keyboardInput;
        private KeyboardState oldState;
        private GameSettings gameSettings;



        public SettingsView()
        {
            string settingsFilePath = "gameSettings.json";
            gameSettings = LoadSettings(settingsFilePath);

            // Initialize keyBindings based on loaded settings
            keyBindings = new Dictionary<string, Keys>();
            foreach (var pair in gameSettings.KeyBindings)
            {
                keyBindings[pair.Key] = (Keys)Enum.Parse(typeof(Keys), pair.Value);
            }

            controlNames = new List<string>(keyBindings.Keys);
            keyboardInput = new KeyboardInput();
        }

        public override void loadContent(ContentManager contentManager)
        {
            m_font = contentManager.Load<SpriteFont>("Fonts/main-menu");
            m_font2 = contentManager.Load<SpriteFont>("Fonts/menu");
            keyboardInput.registerCommand(Keys.Up, true, NavigateUp);
            keyboardInput.registerCommand(Keys.Down, true, NavigateDown);
            keyboardInput.registerCommand(Keys.Enter, true, SelectControl);

        }



        public override GameStateEnum processInput(GameTime gameTime)
        {
            keyboardInput.Update(gameTime); // Update keyboard input system

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) && currentState == SettingsState.Viewing)
            {
                return GameStateEnum.MainMenu;
            }

            if (currentState == SettingsState.Changing)
            {
                var keys = keyboardState.GetPressedKeys();
                if (keys.Length > 0)
                {
                    var key = keys[0];
                    // Exclude the Enter and Escape from being assignable
                    if (key != Keys.Enter && key != Keys.Escape)
                    {
                        
                        keyBindings[changingControl] = key;
                        gameSettings.KeyBindings[changingControl] = key.ToString();

                        // Save the updated settings
                        SaveSettings("gameSettings.json");

                        currentState = SettingsState.Viewing;
                        changingControl = "";
                    }
                }
            }
            else if (currentState == SettingsState.Viewing)
            {
                if (keyboardState.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
                {
                    NavigateUp(gameTime, 0);  
                }
                else if (keyboardState.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
                {
                    NavigateDown(gameTime, 0);  
                }


            }

            oldState = keyboardState; // Remember the old keyboard state for the next frame
            return GameStateEnum.Settings;
        }






        private void NavigateDown(GameTime gameTime, float value)
        {
            if (currentState == SettingsState.Viewing)
            {
                selectedControlIndex--;
                if (selectedControlIndex < 0) selectedControlIndex = controlNames.Count - 1;
            }
        }

        private void NavigateUp(GameTime gameTime, float value)
        {
            if (currentState == SettingsState.Viewing)
            {
                selectedControlIndex++;
                if (selectedControlIndex >= controlNames.Count) selectedControlIndex = 0;
            }
        }


        private void SelectControl(GameTime gameTime, float value)
        {
            if (currentState == SettingsState.Viewing)
            {
                currentState = SettingsState.Changing;
                changingControl = controlNames[selectedControlIndex];
            }

        }




        public override void render(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            int totalTextHeight = controlNames.Count * 100;
            Vector2 position = new Vector2(m_graphics.PreferredBackBufferWidth / 3, (m_graphics.PreferredBackBufferHeight - totalTextHeight) / 2);


            for (int i = 0; i < controlNames.Count; i++)
            {
                Color textColor = Color.White;
                string text = $"{controlNames[i]}: {keyBindings[controlNames[i]]}";

                // Change text color to red if it's the currently selected control for editing
                if (i == selectedControlIndex && currentState == SettingsState.Viewing)
                {
                    textColor = Color.Red;
                }
                else if (changingControl == controlNames[i] && currentState == SettingsState.Changing)
                {
                    text += " (press new key)";
                    textColor = Color.Red; //ake it red if it's currently being changed
                }

                m_spriteBatch.DrawString(m_font, text, position, textColor);
                position.Y += 100; 
            }

            Vector2 stringSize = m_font.MeasureString(MESSAGE);
            m_spriteBatch.DrawString(m_font2, MESSAGE, new Vector2(m_graphics.PreferredBackBufferWidth - stringSize.X / 2, m_graphics.PreferredBackBufferHeight - 100), Color.Yellow);

            m_spriteBatch.End();
        }

        public void SaveSettings(string fileName)
        {
            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Create, isolatedStorage))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GameSettings));
                        serializer.WriteObject(stream, gameSettings);
                    }
                }
                catch (Exception ex)
                {
                    
                    Debug.WriteLine("Error saving settings: " + ex.Message);
                }
            }
        }


        public static GameSettings LoadSettings(string fileName)
        {
            using (IsolatedStorageFile isolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorage.FileExists(fileName))
                {
                    try
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, isolatedStorage))
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GameSettings));
                            return (GameSettings)serializer.ReadObject(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new GameSettings(); // Default settings if loading fails
                    }
                }
            }
            return new GameSettings(); // Return default settings if the file does not exist or an error occurred
        }




        public override void update(GameTime gameTime)
        {
            
        }
    }
}