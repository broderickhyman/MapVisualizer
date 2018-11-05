using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace MapVisualizer
{
  public class Map : DrawableGameComponent
  {
    private VertexBuffer cubeVertexBuffer;
    private VertexBuffer instanceBuffer;
    private IndexBuffer cubeIndexBuffer;
    private IndexBuffer lineIndexBuffer;
    private VertexBufferBinding[] cubeBindings;
    private const int vertexCount = 8;
    private const int cubeIndexCount = 36;
    private const int lineIndexCount = 24;
    private const int cubePrimativeCount = 12;
    private const int linePrimativeCount = 12;
    private int instanceCount;
    private int rows;
    private int columns;
    private const int scale = 30;
    private Effect effect;
    private RasterizerState rasterizerState;
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

      SetDefault();
      //ReadFromFile();
      Camera.MovementSpeed = (rows + columns) / 2 * scale / 15;

      cubeBindings = new VertexBufferBinding[2];
      cubeBindings[0] = new VertexBufferBinding(cubeVertexBuffer);
      cubeBindings[1] = new VertexBufferBinding(instanceBuffer, 0, 1);
      base.Initialize();
    }

    private void ReadFromFile()
    {
      const string path = @"..\..\..\..\output.csv";

      var instances = new List<InstanceData>();
      var map = new List<List<int>>();
      foreach (var line in File.ReadLines(path))
      {
        var row = new List<int>();
        foreach (var split in line.Split(','))
        {
          var height = int.Parse(split);
          row.Add(height);// - (height % 25));
        }
        map.Add(row);
      }
      rows = map.Count;
      columns = map[0].Count;
      var rowOffset = scale * rows / -2;
      var colOffset = scale * columns / -2;
      var r = -1;
      foreach (var row in map)
      {
        r++;
        var c = -1;
        foreach (var height in row)
        {
          c++;
          var color = default(Color);
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
            Color = color,
            Scale = new Vector3(scale, height, scale)
          });
        }
      }
      instanceCount = instances.Count;
      Debug.WriteLine(instanceCount.ToString("N"));

      instanceBuffer = new VertexBuffer(GraphicsDevice, InstanceData.VertexDeclaration, instanceCount, BufferUsage.WriteOnly);
      instanceBuffer.SetData(instances.ToArray());
    }

    private void SetDefault()
    {
      const int spacing = scale;// + 10;
      rows = 2000;// 30;// 3000;
      columns = 2000;// 20;// 2000;
      instanceCount = rows * columns;
      Debug.WriteLine(instanceCount.ToString("N"));
      var instances = new InstanceData[instanceCount];
      var rowOffset = spacing * rows / -2;
      var colOffset = spacing * columns / -2;
      var rand = new Random();
      var centerX = columns / 2;
      var centerY = rows / 2;
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
            Position = new Vector3(rowOffset + (r * spacing), 0, colOffset + (c * spacing)),
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
          //instances[index].Scale.Y = (float)((1 - (distance / maxDistance)) * 250);
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

      cubeVertexBuffer = new VertexBuffer(GraphicsDevice, VertexPosition.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
      cubeVertexBuffer.SetData(vertices);
    }

    private void SetIndexBuffer()
    {
      var cubeIndices = new short[cubeIndexCount];
      cubeIndices[0] = 0; cubeIndices[1] = 1; cubeIndices[2] = 2;
      cubeIndices[3] = 0; cubeIndices[4] = 2; cubeIndices[5] = 3;
      cubeIndices[6] = 4; cubeIndices[7] = 6; cubeIndices[8] = 5;
      cubeIndices[9] = 4; cubeIndices[10] = 7; cubeIndices[11] = 6;
      cubeIndices[12] = 0; cubeIndices[13] = 4; cubeIndices[14] = 5;
      cubeIndices[15] = 0; cubeIndices[16] = 5; cubeIndices[17] = 1;
      cubeIndices[18] = 2; cubeIndices[19] = 6; cubeIndices[20] = 3;
      cubeIndices[21] = 3; cubeIndices[22] = 6; cubeIndices[23] = 7;
      cubeIndices[24] = 0; cubeIndices[25] = 3; cubeIndices[26] = 4;
      cubeIndices[27] = 3; cubeIndices[28] = 7; cubeIndices[29] = 4;
      cubeIndices[30] = 1; cubeIndices[31] = 5; cubeIndices[32] = 2;
      cubeIndices[33] = 2; cubeIndices[34] = 5; cubeIndices[35] = 6;

      cubeIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), cubeIndices.Length, BufferUsage.WriteOnly);
      cubeIndexBuffer.SetData(cubeIndices);

      var lineIndices = new short[lineIndexCount];
      lineIndices[0] = 0; lineIndices[1] = 1;
      lineIndices[2] = 1; lineIndices[3] = 2;
      lineIndices[4] = 2; lineIndices[5] = 3;
      lineIndices[6] = 3; lineIndices[7] = 0;
      lineIndices[8] = 4; lineIndices[9] = 5;
      lineIndices[10] = 5; lineIndices[11] = 6;
      lineIndices[12] = 6; lineIndices[13] = 7;
      lineIndices[14] = 7; lineIndices[15] = 4;
      lineIndices[16] = 0; lineIndices[17] = 4;
      lineIndices[18] = 1; lineIndices[19] = 5;
      lineIndices[20] = 2; lineIndices[21] = 6;
      lineIndices[22] = 3; lineIndices[23] = 7;

      lineIndexBuffer = new IndexBuffer(GraphicsDevice, typeof(short), lineIndices.Length, BufferUsage.WriteOnly);
      lineIndexBuffer.SetData(lineIndices);
    }

    public override void Draw(GameTime gameTime)
    {
      effect.CurrentTechnique = effect.Techniques["BasicColorDrawing"];
      effect.Parameters["WorldViewProjection"].SetValue(Game1.Camera.ViewMatrix * Projection);
      GraphicsDevice.RasterizerState = rasterizerState;
      GraphicsDevice.DepthStencilState = DepthStencilState.Default;

      foreach (var pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();
      }
      GraphicsDevice.SetVertexBuffers(cubeBindings);

      GraphicsDevice.Indices = cubeIndexBuffer;
      GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, cubePrimativeCount, instanceCount);

      effect.CurrentTechnique = effect.Techniques["BasicLineDrawing"];
      foreach (var pass in effect.CurrentTechnique.Passes)
      {
        pass.Apply();
      }
      GraphicsDevice.Indices = lineIndexBuffer;
      GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.LineList, 0, 0, linePrimativeCount, instanceCount);
      base.Draw(gameTime);
    }

    protected override void LoadContent()
    {
      effect = Game.Content.Load<Effect>("InstanceEffect");
      base.LoadContent();
    }
  }
}
