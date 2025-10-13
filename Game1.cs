using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace DungeonSlime;

public class Game1 : Core
{
    //khoi tao cac Sprite

    private AnimatedSprite _slime;
    private AnimatedSprite _bat;
    private Vector2 _slimePosition;
    private Vector2 _batPosition;
    private Vector2 _batVelocity;
    private const float MOVEMENT_SPEED = 5.0f;


    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();
        // TODO: Add your initialization logic here
        _batPosition = new Vector2(_slime.Width + 10, 0);
        AssignRadomBatVelocity();
    }

    protected override void LoadContent()
    {
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _bat = atlas.CreateAnimatedSprite("bat-animation");

        _slime._scale = new Vector2(4.0f, 4.0f);
        _bat._scale = new Vector2( 4.0f, 4.0f); 
    }

    protected override void Update(GameTime gameTime)
    {



        // TODO: Add your update logic here
        //update animation sprite
        _slime.update(gameTime);
        _bat.update(gameTime);
        
        //kiem tra dau vao cua ban phim truoc khi thuc hien 
        CheckKeyboardInput();

        Rectangle ScreenBounds = new Rectangle(
            0,
            0,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        Circle slimeBounds = new Circle(
            (int)(_slimePosition.X + (_slime.Width * 0.5f)),
            (int)(_slimePosition.Y + (_slime.Height * 0.5f)),
            (int)(_slime.Width * 0.5f)
        );
        

        if (slimeBounds.Left < ScreenBounds.Left)
        {
            _slimePosition.X = ScreenBounds.Left;
        }
        else if (slimeBounds.Right > ScreenBounds.Right)
        {
            _slimePosition.X = ScreenBounds.Right - _slime.Width;
        }
        if (slimeBounds.Top < ScreenBounds.Top)
        {
            _slimePosition.Y = ScreenBounds.Top;
        }
        else if (slimeBounds.Bottom > ScreenBounds.Bottom)
        {
            _slimePosition.Y = ScreenBounds.Bottom - _slime.Height;
        }
        Vector2 newBatPosition = _batPosition + _batVelocity;

        Circle batBounds = new Circle(
            (int)(newBatPosition.X + (_bat.Width * 0.5f)),
            (int)(newBatPosition.Y + (_bat.Height * 0.5f)),
            (int)(_bat.Width * 0.5f)
        );
        
        Vector2 Normal = Vector2.Zero;
        if (batBounds.Left < ScreenBounds.Left)
        {
            Normal.X = Vector2.UnitX.X;
            _batPosition.X = ScreenBounds.Left;
        }
        else if (batBounds.Right > ScreenBounds.Right)
        {
            Normal.X = -Vector2.UnitX.X;
            _batPosition.X = ScreenBounds.Right - _bat.Width;
        }
        if (batBounds.Top < ScreenBounds.Top)
        {
            Normal.Y = Vector2.UnitY.Y;
            _batPosition.Y = ScreenBounds.Top;
        }
        else if (batBounds.Bottom > ScreenBounds.Bottom)
        {
            Normal.Y = -Vector2.UnitY.Y;
            _batPosition.Y = ScreenBounds.Bottom - _bat.Height;
        }
        if (Normal != Vector2.Zero)
        {
            Normal.Normalize();
            _batVelocity = Vector2.Reflect(_batVelocity, Normal);
        }
        _batPosition = newBatPosition;
        if (slimeBounds.Intersects(batBounds))
        {
            // tinh tong so o tren mang hinh

            int totalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)(_bat.Width);
            int totalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)(_bat.Height);

            int Column = Random.Shared.Next(0, totalColumns);
            int Row = Random.Shared.Next(0, totalRows);

            _batPosition = new Vector2(Column * _bat.Width, Row * _bat.Height);
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

    }

    protected override void Draw(GameTime gameTime)
    {
        // Clear the back buffer.
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        // render _slime
        _slime.Draw(SpriteBatch, _slimePosition);

        _bat.Draw(SpriteBatch, _batPosition);
        SpriteBatch.End();

        base.Draw(gameTime);
    }


}