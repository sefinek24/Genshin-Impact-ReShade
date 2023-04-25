
#include "shared/cGraphics.fxh"

/*
    Construct options
*/

uniform float _Time < source = "timer"; >;

uniform float _Speed <
    ui_label = "Speed";
    ui_type = "drag";
> = 2.0f;

uniform float _Variance <
    ui_label = "Variance";
    ui_type = "drag";
> = 0.5f;

uniform float _Intensity <
    ui_label = "Variance";
    ui_type = "drag";
> = 0.005f;

// Pixel shaders
// "Well ill believe it when i see it."
// Yoinked code by Luluco250 (RIP) [https://www.shadertoy.com/view/4t2fRz] [MIT]

float GaussianWeights(float x, float Sigma)
{
    const float Pi = 3.14159265359;
    Sigma = Sigma * Sigma;
    return rsqrt(Pi * Sigma) * exp(-((x * x) / (2.0 * Sigma)));
}

float4 PS_FilmGrain(VS2PS_Quad Input) : SV_TARGET0
{
    float Time = rcp(1e+3 / _Time) * _Speed;
    float Seed = dot(Input.HPos.xy, float2(12.9898, 78.233));
    float Noise = frac(sin(Seed) * 43758.5453 + Time);
    return GaussianWeights(Noise, _Variance) * _Intensity;
}

technique CShade_FilmGrain
{
    pass
    {
        // (Shader[Src] * SrcBlend) + (Buffer[Dest] * DestBlend)
        // This shader: (Shader[Src] * (1.0 - Buffer[Dest])) + Buffer[Dest]
        BlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = INVDESTCOLOR;
        DestBlend = ONE;
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_FilmGrain;
    }
}
