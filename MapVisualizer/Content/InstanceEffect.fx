#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
  float4 Position : SV_POSITION0;
};

struct InstanceInput
{
  float3 Position : POSITION1;
  float3 Scale : POSITION2;
  float4 Color : COLOR0;
};

struct VertexShaderOutput
{
  float4 Position : SV_POSITION0;
  float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input, float4 instanceTransform : POSITION1, float4 cubeColor : COLOR0, float3 scale : POSITION2)
{
  VertexShaderOutput output = (VertexShaderOutput)0;

  float4 pos = input.Position;
  pos.x *= scale.x;
  pos.y *= scale.y;
  pos.z *= scale.z;
  pos += instanceTransform;
  pos = mul(pos, WorldViewProjection);
  output.Position = pos;
  output.Color = cubeColor;
  return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
  //return float4(1, 0, 0, 1);
  return input.Color;
}

technique BasicColorDrawing
{
  pass P0
  {
    VertexShader = compile VS_SHADERMODEL MainVS();
    PixelShader = compile PS_SHADERMODEL MainPS();
  }
};