using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace DungeonSlime;

public class Game1 : Core
{
    //khoi tao cac Sprite

    private AnimatedSprite _slime;
    private SoundEffect BounceEffect;
    private SoundEffect CollectEffect;
    private Song _themeSong;
    private AnimatedSprite _bat;
    private Vector2 _slimePosition;
    private Vector2 _batPosition;
    private Vector2 _batVelocity;
    private Tilemap _tilemap;
    private const float MOVEMENT_SPEED = 5.0f;

    private Rectangle _roomBounds;


    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        Rectangle ScreenBounds = GraphicsDevice.PresentationParameters.Bounds;
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
        Audio.PlaySong(_themeSong);
    }

    protected override void LoadContent()
    {
        //load audio
        BounceEffect = Content.Load<SoundEffect>("Audio/bounce");
        CollectEffect = Content.Load<SoundEffect>("Audio/collect");
        _themeSong = Content.Load<Song>("Audio/theme");

        
        // load do hoa
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");
        
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _bat = atlas.CreateAnimatedSprite("bat-animation");

        _slime._scale = new Vector2(4.0f, 4.0f);
        _bat._scale = new Vector2(4.0f, 4.0f);
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);
    }

    protected override void Update(GameTime gameTime)
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
            Audio.PlaySoundEffect(BounceEffect);
        }
        else if (slimeBounds.Right > _roomBounds.Right)
        {
            _slimePosition.X = _roomBounds.Right - _slime.Width;
            Audio.PlaySoundEffect(BounceEffect);
        }
        if (slimeBounds.Top < _roomBounds.Top)
        {
            _slimePosition.Y = _roomBounds.Top;
            Audio.PlaySoundEffect(BounceEffect);
        }
        else if (slimeBounds.Bottom > _roomBounds.Bottom)
        {
            _slimePosition.Y = _roomBounds.Bottom - _slime.Height;
            Audio.PlaySoundEffect(BounceEffect);
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
            Audio.PlaySoundEffect(BounceEffect);
        }
        _batPosition = newBatPosition;
        if (slimeBounds.Intersects(batBounds))
        {
            // tinh tong so o tren mang hinh

            int Column = Random.Shared.Next(1, _tilemap.Columns - 2);
            int Row = Random.Shared.Next(1, _tilemap.Rows - 2);

            _batPosition = new Vector2(Column * _bat.Width, Row * _bat.Height);
            // phat SE Collect
            Audio.PlaySoundEffect(CollectEffect);
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

        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            _slimePosition.Y -= speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            _slimePosition.X -= speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            _slimePosition.Y += speed;
        }

        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            _slimePosition.X += speed;
        }

        if (Input.Keyboard.WasKeyPressed(Keys.OemMinus))
        {
            Audio.SoundEffectVolume -= 0.1f;
            Audio.SongVolume -= 0.1f;
        }
        if (Input.Keyboard.WasKeyPressed(Keys.OemPlus))
        {
            Audio.SoundEffectVolume += 0.1f;
            Audio.SongVolume += 0.1f;
        }
        if (Input.Keyboard.WasKeyPressed(Keys.M))
        {
            Audio.ToggleMute();
        }

    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        // render _slime
        _tilemap.Draw(SpriteBatch);
        _slime.Draw(SpriteBatch, _slimePosition);
        _bat.Draw(SpriteBatch, _batPosition);
        SpriteBatch.End();

        base.Draw(gameTime);
    }


}