using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    private int instanceCount;
    private const int rows = 20;
    private const int columns = 20;
    private Effect effect;
    private RasterizerState rasterizerState;
    public Matrix View;
    public Matrix Projection;

    private static Color red = new Color(145, 17, 17);
    private static Color blue = new Color(19, 19, 220);
    private static Color brown = new Color(50, 30, 20);
    private static Color green = new Color(17, 163, 17);
    private static Color tree = new Color(13, 91, 13);
    private const int firstLevel = 25;
    private const int secondLevel = 75;
    private const int thirdLevel = 155;
    private const int fourthLevel = 240;

    public Map(Game game) : base(game)
    {

    }

    [StructLayout(LayoutKind.Sequential)]
    private struct InstanceData : IVertexType
    {
      public static readonly VertexDeclaration VertexDeclaration;
      public Vector3 Position;
      public Vector3 Scale;
      public Color Color;

      VertexDeclaration IVertexType.VertexDeclaration
      {
        get { return VertexDeclaration; }
      }

      static InstanceData()
      {
        var elements = new VertexElement[]
        {
          new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
          new VertexElement(sizeof(float)*3, VertexElementFormat.Vector3, VertexElementUsage.Position, 2),
          new VertexElement(sizeof(float)*6, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        };
        VertexDeclaration = new VertexDeclaration(elements);
      }
    }

    public override void Initialize()
    {
      rasterizerState = new RasterizerState
      {
        CullMode = CullMode.CullClockwiseFace
      };
      SetVertexBuffer();
      SetIndexBuffer();

      //SetDefault();
      ReadFromFile();

      bindings = new VertexBufferBinding[2];
      bindings[0] = new VertexBufferBinding(vertexBuffer);
      bindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
      base.Initialize();
    }

    private void ReadFromFile()
    {
      const string path = @"C:\Users\Broderick\Code\MapBuilder\MapBuilder\bin\Debug\output.csv";
      const int scale = 20;
      const int rowOffset = scale * rows / -2;
      const int colOffset = scale * columns / -2;
      var rand = new Random();

      var instances = new List<InstanceData>();
      var r = -1;
      foreach (var line in File.ReadLines(path))
      {
        r++;
        var c = -1;
        var color = default(Color);
        foreach (var split in line.Split(','))
        {
          c++;
          var height = int.Parse(split);
          if (height <= firstLevel)
          {
            color = blue;
          }
          else if (height > firstLevel && height <= secondLevel)
          {
            color = green;
          }
          else if (height > secondLevel && height <= thirdLevel)
          {
            color = tree;
          }
          else if (height > thirdLevel && height <= fourthLevel)
          {
            color = brown;
          }
          else
          {
            color = red;
          }
          instances.Add(new InstanceData
          {
            Position = new Vector3(rowOffset + (r * scale), 0, colOffset + (c * scale)),
            //Color = new Color(rand.Next(255), rand.Next(255), rand.Next(255)),
            Color = color,
            Scale = new Vector3(scale, height, scale)
          });
        }
      }
      instanceCount = instances.Count;

      instanceBuffer = new VertexBuffer(GraphicsDevice, InstanceData.VertexDeclaration, instanceCount, BufferUsage.WriteOnly);
      instanceBuffer.SetData(instances.ToArray());
    }

    private void SetDefault()
    {
      instanceCount = rows * columns;
      Debug.WriteLine(instanceCount.ToString("N"));
      var instances = new InstanceData[instanceCount];
      const int scale = 20;
      const int rowOffset = scale * rows / -2;
      const int colOffset = scale * columns / -2;
      var rand = new Random();
      const int centerX = columns / 2;
      const int centerY = rows / 2;
      var maxDistance = Math.Sqrt(Math.Pow(centerX, 2) + Math.Pow(centerY, 2));
      Debug.WriteLine(centerX);
      Debug.WriteLine(centerY);
      Debug.WriteLine(maxDistance);
      for (var r = 0; r < rows; r++)
      {
        for (var c = 0; c < columns; c++)
        {
          //Debug.WriteLine("");
          //Debug.WriteLine($"r: {r} c: {c}");
          var index = (r * columns) + c;
          instances[index] = new InstanceData
          {
            Position = new Vector3(rowOffset + (r * scale), 0, colOffset + (c * scale)),
            Color = new Color(rand.Next(255), rand.Next(255), rand.Next(255)),
            //Scale = new Vector3(scale, rand.Next(scale, 250), scale)
            Scale = new Vector3(scale, 10, scale)
          };
          var deltaX = Math.Abs(c - centerX);
          var deltaY = Math.Abs(r - centerY);
          var distance = Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
          //Debug.WriteLine(deltaX);
          //Debug.WriteLine(deltaY);
          //Debug.WriteLine(distance);
          var colorValue = (float)((1 - (distance / maxDistance)) * 150);
          //Debug.WriteLine(colorValue);

          //instances[index].Color = new Color((int)colorValue + 100, 0, 0, 255);
          instances[index].Scale.Y = (float)((1 - (distance / maxDistance)) * 250);
        }
      }
      //instances[1].Scale.Y *= 2;
      //instances[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Red);
      //instances[1] = new VertexPositionColor(new Vector3(30, 30, 0), Color.Blue);

      instanceBuffer = new VertexBuffer(GraphicsDevice, InstanceData.VertexDeclaration, instanceCount, BufferUsage.WriteOnly);
      instanceBuffer.SetData(instances);
    }

    private void SetVertexBuffer()
    {
      var vertices = new VertexPosition[vertexCount];
      var origin = new Vector3(0, 0, 0);
      var width = new Vector3(1f, 0, 0);
      var height = new Vector3(0, 1f, 0);
      var depth = new Vector3(0, 0, 1f);

      vertices[0] = new VertexPosition(origin);
      vertices[1] = new VertexPosition(origin + height);
      vertices[2] = new VertexPosition(origin + height + width);
      vertices[3] = new VertexPosition(origin + width);
      vertices[4] = new VertexPosition(depth + origin);
      vertices[5] = new VertexPosition(depth + origin + height);
      vertices[6] = new VertexPosition(depth + origin + height + width);
      vertices[7] = new VertexPosition(depth + origin + width);

      vertexBuffer = new VertexBuffer(GraphicsDevice, VertexPosition.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
      vertexBuffer.SetData(vertices);
    }

    private void SetIndexBuffer()
    {
      var indices = new short[indexCount];
      indices[0] = 0; indices[1] = 1; indices[2] = 2;
      indices[3] = 0; indices[4] = 2; indices[5] = 3;
      indices[6] = 4; indices[7] = 6; indices[8] = 5;
      indices[9] = 4; indices[10] = 7; indices[11] = 6;
      indices[12] = 0; indices[13] = 4; indices[14] = 5;
      indices[15] = 0; indices[16] = 5; indices[17] = 1;
      indices[18] = 2; indices[19] = 6; indices[20] = 3;
      indices[21] = 3; indices[22] = 6; indices[23] = 7;
      indices[24] = 0; indices[25] = 3; indices[26] = 4;
      indices[27] = 3; indices[28] = 7; indices[29] = 4;
      indices[30] = 1; indices[31] = 5; indices[32] = 2;
      indices[33] = 2; indices[34] = 5; indices[35] = 6;

      indexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
      indexBuffer.SetData(indices);
    }

    public override void Draw(GameTime gameTime)
    {
      effect.CurrentTechnique = effect.Techniques["BasicColorDrawing"];
      effect.Parameters["WorldViewProjection"].SetValue(this.View * this.Projection);
      GraphicsDevice.Indices = indexBuffer;
      GraphicsDevice.RasterizerState = rasterizerState;
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

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
