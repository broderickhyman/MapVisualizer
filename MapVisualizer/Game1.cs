using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MapVisualizer
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : Game
  {
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Vector3 camTarget;
    private Vector3 camPosition;
    private Matrix projectionMatrix;
    private Matrix viewMatrix;
    private Matrix worldMatrix;
    private BasicEffect basicEffect;
    private bool orbit;
    private readonly Map map;

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      map = new Map(this);
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      map.Initialize();
      camTarget = new Vector3(0f, 0f, 0f);
      camPosition = new Vector3(0f, 30f, -200f);
      projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
      map.Projection = projectionMatrix;
      viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, new Vector3(0f, 1f, 0f));// Y up
      worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
      map.View = viewMatrix;

      basicEffect = new BasicEffect(GraphicsDevice)
      {
        Alpha = 1f,
        //Lighting requires normal information which VertexPositionColor does not have
        //If you want to use lighting and VPC you need to create a custom def
        LightingEnabled = false,
        Projection = projectionMatrix,
        // Want to see the colors of the vertices, this needs to be on
        VertexColorEnabled = false,
        View = viewMatrix,
        World = worldMatrix
      };

      GraphicsDevice.RasterizerState = new RasterizerState
      {
        CullMode = CullMode.None
      };

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);
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
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      if (Keyboard.GetState().IsKeyDown(Keys.Left))
      {
        camPosition.X--;
        camTarget.X--;
      }
      if (Keyboard.GetState().IsKeyDown(Keys.Right))
      {
        camPosition.X++;
        camTarget.X++;
      }
      if (Keyboard.GetState().IsKeyDown(Keys.Up))
      {
        camPosition.Y--;
        camTarget.Y--;
      }
      if (Keyboard.GetState().IsKeyDown(Keys.Down))
      {
        camPosition.Y++;
        camTarget.Y++;
      }
      if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
      {
        camPosition.Z++;
      }
      if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
      {
        camPosition.Z--;
      }
      if (Keyboard.GetState().IsKeyDown(Keys.Space))
      {
        orbit = !orbit;
      }

      if (orbit)
      {
        var rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(1f));
        camPosition = Vector3.Transform(camPosition, rotationMatrix);
        viewMatrix = Matrix.CreateLookAt(camPosition, camTarget, Vector3.Up);
        basicEffect.View = viewMatrix;
        map.View = viewMatrix;
      }

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      map.Draw(gameTime);

      base.Draw(gameTime);
    }
  }
}
