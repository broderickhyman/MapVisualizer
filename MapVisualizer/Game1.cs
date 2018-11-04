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
    public static float WindowWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2f;
    public static float WindowHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1.25f;
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private SpriteFont spriteFont;
    private RasterizerState rasterizerState;
    private Vector3 camTarget;
    private Vector3 camPosition;
    private Matrix projectionMatrix;
    private Matrix viewMatrix;
    private Matrix worldMatrix;
    private bool orbit = true;

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
      camTarget = new Vector3(0f, 50f, 0f);
      camPosition = new Vector3(0f, 150f, -300f);
      projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 100000f);
      map.Projection = projectionMatrix;
      viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, new Vector3(0f, 1f, 0f));
      worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
      map.View = viewMatrix;

      rasterizerState = new RasterizerState
      {
        CullMode = CullMode.None
      };

      GraphicsDevice.RasterizerState = rasterizerState;
      oldState = Keyboard.GetState();

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
        Exit();
      const float speed = 3f;

      if (keyboardState.IsKeyDown(Keys.Left))
      {
        camPosition.X -= speed;
        camTarget.X -= speed;
      }
      if (keyboardState.IsKeyDown(Keys.Right))
      {
        camPosition.X += speed;
        camTarget.X += speed;
      }
      if (keyboardState.IsKeyDown(Keys.Up))
      {
        camPosition.Y -= speed;
        camTarget.Y -= speed;
      }
      if (keyboardState.IsKeyDown(Keys.Down))
      {
        camPosition.Y += speed;
        camTarget.Y += speed;
      }
      if (keyboardState.IsKeyDown(Keys.OemPlus))
      {
        camPosition.Z += speed;
      }
      if (keyboardState.IsKeyDown(Keys.OemMinus))
      {
        camPosition.Z -= speed;
      }
      if (keyboardState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
      {
        orbit = !orbit;
      }

      if (orbit)
      {
        var rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
        camPosition = Vector3.Transform(camPosition, rotationMatrix);
      }
      viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
      map.View = viewMatrix;
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
