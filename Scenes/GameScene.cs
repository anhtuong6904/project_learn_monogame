using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes
{
    public class GameScene : Scene
    {
        private SpriteFont _font;

        private int _score;
        private Vector2 _scoreTextPosition;
        private Vector2 _scoreTextOrigin;

        //khoi tao cac Sprite

        private AnimatedSprite _slime;
        private SoundEffect BounceEffect;
        private SoundEffect CollectEffect;
        private AnimatedSprite _bat;
        private Vector2 _slimePosition;
        private Vector2 _batPosition;
        private Vector2 _batVelocity;
        private Tilemap _tilemap;
        private const float MOVEMENT_SPEED = 5.0f;

        private Rectangle _roomBounds;

        public override void Initialize()
        {
            base.Initialize();
            Core.ExitOnEscape = false;
            Rectangle ScreenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
            _roomBounds = new Rectangle(
                (int)_tilemap.tileWidth,
                (int)_tilemap.tileHeight,
                (int)ScreenBounds.Width - (int)_tilemap.tileWidth * 2,
                (int)ScreenBounds.Height - (int)_tilemap.tileHeight * 2

            );
            //set slime position
            int centerColumn = (int)_tilemap.Columns / 2;
            int centerRow = (int)_tilemap.Rows / 2;



            _slimePosition = new Vector2(centerColumn * (int)_tilemap.tileWidth, centerRow * (int)_tilemap.tileHeight);
            //set bat posittion
            _batPosition = new Vector2(_roomBounds.Left, _roomBounds.Top);
            //tao vector di chuyen cho bat
            AssignRadomBatVelocity();
            //Position score 
            _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.tileHeight * 0.5f);
            float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
            _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            //load audio
            BounceEffect = Content.Load<SoundEffect>("Audio/bounce");
            CollectEffect = Content.Load<SoundEffect>("Audio/collect");
            // load do hoa
            TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

            _slime = atlas.CreateAnimatedSprite("slime-animation");
            _bat = atlas.CreateAnimatedSprite("bat-animation");

            _slime._scale = new Vector2(4.0f, 4.0f);
            _bat._scale = new Vector2(4.0f, 4.0f);
            _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
            _tilemap.Scale = new Vector2(4.0f, 4.0f);
            _font = Content.Load<SpriteFont>("Font/04B_30");
        }


        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            //update animation sprite
            _slime.update(gameTime);
            _bat.update(gameTime);

            //kiem tra dau vao cua ban phim truoc khi thuc hien 
            CheckKeyboardInput();

            Circle slimeBounds = new Circle(
                (int)(_slimePosition.X + (_slime.Width * 0.5f)),
                (int)(_slimePosition.Y + (_slime.Height * 0.5f)),
                (int)(_slime.Width * 0.5f)
            );


            if (slimeBounds.Left < _roomBounds.Left)
            {
                _slimePosition.X = _roomBounds.Left;
            }
            else if (slimeBounds.Right > _roomBounds.Right)
            {
                _slimePosition.X = _roomBounds.Right - _slime.Width;

            }
            if (slimeBounds.Top < _roomBounds.Top)
            {
                _slimePosition.Y = _roomBounds.Top;

            }
            else if (slimeBounds.Bottom > _roomBounds.Bottom)
            {
                _slimePosition.Y = _roomBounds.Bottom - _slime.Height;

            }
            Vector2 newBatPosition = _batPosition + _batVelocity;

            Circle batBounds = new Circle(
                (int)(newBatPosition.X + (_bat.Width * 0.5f)),
                (int)(newBatPosition.Y + (_bat.Height * 0.5f)),
                (int)(_bat.Width * 0.5f)
            );

            Vector2 Normal = Vector2.Zero;
            if (batBounds.Left < _roomBounds.Left)
            {
                Normal.X = Vector2.UnitX.X;
                _batPosition.X = _roomBounds.Left;
            }
            else if (batBounds.Right > _roomBounds.Right)
            {
                Normal.X = -Vector2.UnitX.X;
                _batPosition.X = _roomBounds.Right - _bat.Width;
            }
            if (batBounds.Top < _roomBounds.Top)
            {
                Normal.Y = Vector2.UnitY.Y;
                _batPosition.Y = _roomBounds.Top;
            }
            else if (batBounds.Bottom > _roomBounds.Bottom)
            {
                Normal.Y = -Vector2.UnitY.Y;
                _batPosition.Y = _roomBounds.Bottom - _bat.Height;
            }
            if (Normal != Vector2.Zero)
            {
                Normal.Normalize();
                _batVelocity = Vector2.Reflect(_batVelocity, Normal);
                Core.Audio.PlaySoundEffect(BounceEffect);
            }
            _batPosition = newBatPosition;
            if (slimeBounds.Intersects(batBounds))
            {
                // tinh tong so o tren mang hinh

                int Column = Random.Shared.Next(1, _tilemap.Columns - 2);
                int Row = Random.Shared.Next(1, _tilemap.Rows - 2);

                _batPosition = new Vector2(Column * _bat.Width, Row * _bat.Height);
                //update Score
                _score += 100;
                // phat SE Collect
                Core.Audio.PlaySoundEffect(CollectEffect);
                AssignRadomBatVelocity();
            }


            base.Update(gameTime);
        }
        private void AssignRadomBatVelocity()
        {
            // Generate a random angle.
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

            // Convert angle to a direction vector.
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);

            // Multiply the direction vector by the movement speed.
            _batVelocity = direction * MOVEMENT_SPEED;
        }

        private void CheckKeyboardInput()
        {
            float speed = MOVEMENT_SPEED;

            if (Core.Input.Keyboard.IsKeyDown(Keys.Space))
            {
                speed *= 1.5f;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.W) || Core.Input.Keyboard.IsKeyDown(Keys.Up))
            {
                _slimePosition.Y -= speed;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.A) || Core.Input.Keyboard.IsKeyDown(Keys.Left))
            {
                _slimePosition.X -= speed;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.S) || Core.Input.Keyboard.IsKeyDown(Keys.Down))
            {
                _slimePosition.Y += speed;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.D) || Core.Input.Keyboard.IsKeyDown(Keys.Right))
            {
                _slimePosition.X += speed;
            }

            if (Core.Input.Keyboard.WasKeyPressed(Keys.OemMinus))
            {
                Core.Audio.SoundEffectVolume -= 0.1f;
                Core.Audio.SongVolume -= 0.1f;
            }
            if (Core.Input.Keyboard.WasKeyPressed(Keys.OemPlus))
            {
                Core.Audio.SoundEffectVolume += 0.1f;
                Core.Audio.SongVolume += 0.1f;
            }
            if (Core.Input.Keyboard.WasKeyPressed(Keys.M))
            {
                Core.Audio.ToggleMute();
            }
        }
        
        public override void Draw(GameTime gameTime)
        {
            // Clear the back buffer.
            Core.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Begin the sprite batch to prepare for rendering.
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            // Draw the score


            // render _slime
            _tilemap.Draw(Core.SpriteBatch);
            Core.SpriteBatch.DrawString(
                _font,              // spriteFont
                $"Score: {_score}", // text
                _scoreTextPosition, // position
                Color.White,        // color
                0.0f,               // rotation
                _scoreTextOrigin,   // origin
                1.0f,               // scale
                SpriteEffects.None, // effects
                0.0f                // layerDepth
            );
            _slime.Draw(Core.SpriteBatch, _slimePosition);
            _bat.Draw(Core.SpriteBatch, _batPosition);
            Core.SpriteBatch.End();

            base.Draw(gameTime);
        }

    }
}