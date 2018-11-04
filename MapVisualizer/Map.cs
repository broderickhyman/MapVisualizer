using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapVisualizer
{
  public class Map : DrawableGameComponent
  {
    private VertexBuffer vertexBuffer;
    private VertexBuffer instanceBuffer;
    private IndexBuffer indexBuffer;
    private VertexBufferBinding[] bindings;
    private const int vertexCount = 8;
    private const int indexCount = 36;
    private const int primativeCount = 12;
    private readonly int instanceCount;
    private const int rows = 10;
    private const int columns = 10;
    private Effect effect;

    public Matrix View;

    public Matrix Projection;

    public Map(Game game) : base(game)
    {
      instanceCount = rows * columns;
    }

    public override void Initialize()
    {
      var vertices = new VertexPositionColor[vertexCount];
      var origin = new Vector3(-10, -10, -10);
      var width = new Vector3(20, 0, 0);
      var height = new Vector3(0, 20, 0);
      var depth = new Vector3(0, 0, 20);

      vertices[0] = new VertexPositionColor(origin, Color.Red);
      vertices[1] = new VertexPositionColor(origin + height, Color.Yellow);
      vertices[2] = new VertexPositionColor(origin + height + width, Color.Green);
      vertices[3] = new VertexPositionColor(origin + width, Color.Blue);
      vertices[4] = new VertexPositionColor(depth + origin, Color.Red);
      vertices[5] = new VertexPositionColor(depth + origin + height, Color.Yellow);
      vertices[6] = new VertexPositionColor(depth + origin + height + width, Color.Green);
      vertices[7] = new VertexPositionColor(depth + origin + width, Color.Blue);

      vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
      vertexBuffer.SetData(vertices);

      var indices = new short[indexCount];
      indices[0] = 0; indices[1] = 1; indices[2] = 2;
      indices[3] = 0; indices[4] = 2; indices[5] = 3;
      indices[6] = 4; indices[7] = 5; indices[8] = 6;
      indices[9] = 4; indices[10] = 6; indices[11] = 7;
      indices[12] = 0; indices[13] = 4; indices[14] = 5;
      indices[15] = 0; indices[16] = 1; indices[17] = 5;
      indices[18] = 2; indices[19] = 3; indices[20] = 6;
      indices[21] = 3; indices[22] = 6; indices[23] = 7;
      indices[24] = 0; indices[25] = 3; indices[26] = 4;
      indices[27] = 3; indices[28] = 4; indices[29] = 7;
      indices[30] = 1; indices[31] = 2; indices[32] = 5;
      indices[33] = 2; indices[34] = 5; indices[35] = 6;

      indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
      indexBuffer.SetData(indices);

      var instances = new VertexPositionColor[instanceCount];
      const int spacing = 30;
      const int rowOffset = spacing * rows / -2;
      const int colOffset = spacing * columns / -2;
      var rand = new Random();
      for (var r = 0; r < rows; r++)
      {
        for (var c = 0; c < columns; c++)
        {
          instances[(r * columns) + c] = new VertexPositionColor(new Vector3(rowOffset + (r * 30), -50, colOffset + (c * 30)), new Color(rand.Next(255), rand.Next(255), rand.Next(255)));
        }
      }
      //instances[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
      //instances[1] = new VertexPositionColor(new Vector3(30, 30, 0), Color.Blue);

      instanceBuffer = new VertexBuffer(GraphicsDevice, VertexPositionColor.VertexDeclaration, instanceCount, BufferUsage.WriteOnly);
      instanceBuffer.SetData(instances);

      bindings = new VertexBufferBinding[2];
      bindings[0] = new VertexBufferBinding(vertexBuffer);
      bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
      base.Initialize();
    }

    public override void Draw(GameTime gameTime)
    {
      effect.CurrentTechnique = effect.Techniques["BasicColorDrawing"];
      effect.Parameters["WorldViewProjection"].SetValue(this.View * this.Projection);
      GraphicsDevice.Indices = indexBuffer;

      foreach (var pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();
      }

      GraphicsDevice.SetVertexBuffers(bindings);
      GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, primativeCount, instanceCount);
      base.Draw(gameTime);
    }

    protected override void LoadContent()
    {
      effect = Game.Content.Load<Effect>("InstanceEffect");
      base.LoadContent();
    }
  }
}
