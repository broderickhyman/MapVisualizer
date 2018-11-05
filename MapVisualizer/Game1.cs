using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MapVisualizer
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : Game
  {
    public static float WindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 1.2f;
    public static float WindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.25f;
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont spriteFont;
    private RasterizerState rasterizerState;
    public static Camera Camera;

    private KeyboardState oldState;

    private readonly Map map;
    private int frameRate;
    private int frameCounter;
    private TimeSpan elapsedTime = TimeSpan.Zero;

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this)
      {
        PreferredBackBufferWidth = (int)WindowWidth,
        PreferredBackBufferHeight = (int)WindowHeight
      };
      Window.Position = new Point((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2) - (graphics.PreferredBackBufferWidth / 2), (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2) - (graphics.PreferredBackBufferHeight / 2));
      Content.RootDirectory = "Content";
      map = new Map(this);
      Components.Add(map);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      map.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);


      rasterizerState = new RasterizerState
      {
        CullMode = CullMode.None
      };

      GraphicsDevice.RasterizerState = rasterizerState;
      oldState = Keyboard.GetState();

      Camera = new Camera(new Vector3(0f, 150f, -300f), Vector3.Forward, new Vector3(0, 1, 0));

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);
      spriteFont = Content.Load<SpriteFont>("Main");
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {

    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      var keyboardState = Keyboard.GetState();
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
      {
        Exit();
      }

      var movement = 100f * (float)gameTime.ElapsedGameTime.TotalSeconds;
      var radial = 50f * (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (keyboardState.IsKeyDown(Keys.W))
      {
        Camera.Thrust(movement);
      }
      if (keyboardState.IsKeyDown(Keys.S))
      {
        Camera.Thrust(-movement);
      }
      if (keyboardState.IsKeyDown(Keys.A))
      {
        Camera.StrafeHorz(movement);
      }
      if (keyboardState.IsKeyDown(Keys.D))
      {
        Camera.StrafeHorz(-movement);
      }
      if (keyboardState.IsKeyDown(Keys.LeftShift))
      {
        Camera.StrafeVert(-movement);
      }
      if (keyboardState.IsKeyDown(Keys.Space))
      {
        Camera.StrafeVert(movement);
      }
      if (keyboardState.IsKeyDown(Keys.Up))
      {
        Camera.Pitch(-radial);
      }
      if (keyboardState.IsKeyDown(Keys.Down))
      {
        Camera.Pitch(radial);
      }
      if (keyboardState.IsKeyDown(Keys.Right))
      {
        Camera.Yaw(-radial * 2);
      }
      if (keyboardState.IsKeyDown(Keys.Left))
      {
        Camera.Yaw(radial * 2);
      }
      if (keyboardState.IsKeyDown(Keys.E))
      {
        Camera.Roll(radial);
      }
      if (keyboardState.IsKeyDown(Keys.Q))
      {
        Camera.Roll(-radial);
      }

      elapsedTime += gameTime.ElapsedGameTime;
      if (elapsedTime > TimeSpan.FromSeconds(1))
      {
        elapsedTime -= TimeSpan.FromSeconds(1);
        frameRate = frameCounter;
        frameCounter = 0;
      }

      oldState = keyboardState;
      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      frameCounter++;
      GraphicsDevice.Clear(Color.CornflowerBlue);

      var fps = string.Format("FPS: {0}", frameRate);

      spriteBatch.Begin();
      spriteBatch.DrawString(spriteFont, fps, new Vector2(0, 0), Color.Black, 0, new Vector2(0, 0), 1, SpriteEffects.None, 1);
      spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
