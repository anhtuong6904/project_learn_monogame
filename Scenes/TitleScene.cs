using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;


namespace DungeonSlime.Scenes
{
    public class TitleScene : Scene
    {


        private const string DUNGEON_TEXT = "Dungeon";
        private const string SLIME_TEXT = "Slime";

        private const string PRESS_ENTER_TEXT = "Press Enter To Start";
        private SpriteFont _font;

        private SpriteFont _font5x;

        private Vector2 _dungeonTextPosition;

        private Vector2 _dungeonTextOrigin;

        private Vector2 _slimeTextPosition;

        private Vector2 _slimeTextOrigin;

        private Vector2 _pressEnterPos;

        private Vector2 _pressEnterOrigin;


        public override void Initialize()
        {
            base.Initialize();

            Core.ExitOnEscape = true;


            Vector2 size = _font5x.MeasureString(DUNGEON_TEXT);

            _dungeonTextPosition = new Vector2(640, 100);
            _dungeonTextOrigin = size * 0.5f;

            size = _font5x.MeasureString(SLIME_TEXT);
            _slimeTextOrigin = size * 0.5f;
            _slimeTextPosition = new Vector2(757, 207);

            size = _font.MeasureString(PRESS_ENTER_TEXT);
            _pressEnterOrigin = size * 0.5f;
            _pressEnterPos = new Vector2(640, 620);
        }


        public override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font/04B_30");
            _font5x = Content.Load<SpriteFont>("Font/04B_30_5x");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Core.Input.Keyboard.WasKeyPressed(Keys.Enter))
            {
                Core.ChangeScene(new GameScene());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Color dropShadowColor = Color.Black * 0.5f;

            Core.SpriteBatch.DrawString(
                _font5x,
                DUNGEON_TEXT,
                _dungeonTextPosition + new Vector2(10, 10),
                dropShadowColor,
                0.0f,
                _dungeonTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f
            );
            Core.SpriteBatch.DrawString(
                _font5x,
                DUNGEON_TEXT,
                _dungeonTextPosition,
                Color.White,
                0.0f,
                _dungeonTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f
            );
            Core.SpriteBatch.DrawString(
                _font5x,
                SLIME_TEXT,
                _slimeTextPosition + new Vector2(10, 10),
                dropShadowColor,
                0.0f,
                _slimeTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f
            );
            Core.SpriteBatch.DrawString(
                _font5x,
                SLIME_TEXT,
                _slimeTextPosition,
                Color.White,
                0.0f,
                _slimeTextOrigin,
                1.0f,
                SpriteEffects.None,
                1.0f
            );
            
            Core.SpriteBatch.DrawString(
                _font,
                PRESS_ENTER_TEXT,
                _pressEnterPos,
                Color.White,
                0.0f,
                _pressEnterOrigin,
                1.0f,
                SpriteEffects.None,
                0.0f
            );

            Core.SpriteBatch.End();
        }
    }
}