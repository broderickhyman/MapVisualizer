using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MapVisualizer
{
  public class Camera : GameComponent
  {
    public static float MovementSpeed = 200f;
    public static float VerticalMovementSpeed = 400f;
    public static float RadialSpeed = 50f;

    public Vector3 Position { get; private set; }
    public Vector3 Up { get; private set; }
    public Vector3 Forward { get; private set; }

    /// <summary>
    /// Construct a view matrix corresponding to this camera.
    /// </summary>
    public Matrix ViewMatrix
    {
      get
      {
        return Matrix.CreateLookAt(Position, Forward + Position, Up);
      }
    }

    public Camera(Game game, Vector3 position, Vector3 forward, Vector3 up) : base(game)
    {
      Position = position;
      Forward = forward;
      Up = up;
    }

    /// <summary>
    /// Move forward with respect to camera on a plane
    /// </summary>
    /// <param name="amount"></param>
    public void Thrust(float amount)
    {
      Forward.Normalize();
      var change = Forward * amount;
      change.Y = 0;
      Position += change;
    }

    /// <summary>
    /// Strafe left with respect to camera
    /// </summary>
    /// <param name="amount"></param>
    public void StrafeHorz(float amount)
    {
      var left = Vector3.Cross(Up, Forward);
      left.Normalize();
      Position += left * amount;
    }

    /// <summary>
    /// Strafe up along world Up axis
    /// </summary>
    /// <param name="amount"></param>
    public void StrafeVert(float amount)
    {
      Position += Vector3.Up * amount;
    }

    /// <summary>
    /// Yaw (turn/steer around world up vector) to the left
    /// </summary>
    /// <param name="amount">Angle in degrees</param>
    public void Yaw(float amount)
    {
      Forward.Normalize();
      Up.Normalize();

      Forward = Vector3.Transform(Forward, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(amount)));
      Up = Vector3.Transform(Up, Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(amount)));
    }

    /// <summary>
    /// Pitch (around leftward axis) upward
    /// </summary>
    /// <param name="amount"></param>
    public void Pitch(float amount)
    {
      Forward.Normalize();
      var left = Vector3.Cross(Up, Forward);
      left.Normalize();

      Forward = Vector3.Transform(Forward, Matrix.CreateFromAxisAngle(left, MathHelper.ToRadians(amount)));
      Up = Vector3.Transform(Up, Matrix.CreateFromAxisAngle(left, MathHelper.ToRadians(amount)));
    }

    public override void Update(GameTime gameTime)
    {
      var keyboardState = Keyboard.GetState();
      var movement = MovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
      var verticalMovement = VerticalMovementSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
      var radial = RadialSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (keyboardState.IsKeyDown(Keys.W))
      {
        Thrust(movement);
      }
      if (keyboardState.IsKeyDown(Keys.S))
      {
        Thrust(-movement);
      }
      if (keyboardState.IsKeyDown(Keys.A))
      {
        StrafeHorz(movement);
      }
      if (keyboardState.IsKeyDown(Keys.D))
      {
        StrafeHorz(-movement);
      }
      if (keyboardState.IsKeyDown(Keys.LeftShift))
      {
        StrafeVert(-verticalMovement);
      }
      if (keyboardState.IsKeyDown(Keys.Space))
      {
        StrafeVert(verticalMovement);
      }
      if (keyboardState.IsKeyDown(Keys.Up))
      {
        Pitch(-radial);
      }
      if (keyboardState.IsKeyDown(Keys.Down))
      {
        Pitch(radial);
      }
      if (keyboardState.IsKeyDown(Keys.Right))
      {
        Yaw(-radial * 2);
      }
      if (keyboardState.IsKeyDown(Keys.Left))
      {
        Yaw(radial * 2);
      }

      base.Update(gameTime);
    }
  }
}
