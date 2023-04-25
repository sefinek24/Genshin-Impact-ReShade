
#include "shared/cGraphics.fxh"
#include "shared/cImageProcessing.fxh"

uniform float _Radius <
    ui_label = "Convolution radius";
    ui_type = "drag";
    ui_min = 0.0;
> = 32.0;

uniform int _Samples <
    ui_label = "Convolution sample count";
    ui_type = "drag";
    ui_min = 0;
> = 8;

float GradientNoise(float2 Position)
{
    const float3 Numbers = float3(0.06711056f, 0.00583715f, 52.9829189f);
    return frac(Numbers.z * frac(dot(Position.xy, Numbers.xy)));
}

float4 PS_NoiseBlur(VS2PS_Quad Input) : SV_TARGET0
{
    float4 OutputColor = 0.0;

    const float2 PixelSize = 1.0 / int2(BUFFER_WIDTH, BUFFER_HEIGHT);
    float Noise = 2.0 * Pi * GradientNoise(Input.HPos.xy);

    float2 Rotation = 0.0;
    sincos(Noise, Rotation.y, Rotation.x);

    float2x2 RotationMatrix = float2x2(Rotation.x, Rotation.y,
                                      -Rotation.y, Rotation.x);

    for(int i = 0; i < _Samples; i++)
    {
        float2 SampleOffset = mul(SampleVogel(i, _Samples) * _Radius, RotationMatrix);
        OutputColor += tex2Dlod(CShade_SampleColorTex, float4(Input.Tex0 + (SampleOffset * PixelSize), 0.0, 0.0));
    }

    return OutputColor / _Samples;
}

technique CShade_NoiseBlur
{
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_NoiseBlur;
    }
}
