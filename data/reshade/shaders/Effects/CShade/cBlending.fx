
#include "shared/cGraphics.fxh"

uniform int _Blend <
    ui_label = "Blend Mode";
    ui_type = "combo";
    ui_items = " Add\0 Subtract\0 Multiply\0 Min\0 Max\0 Screen\0";
> = 0;

uniform float3 _SrcFactor <
    ui_label = "Source Factor (RGB)";
    ui_type = "drag";
> = 1.0;

uniform float3 _DestFactor <
    ui_label = "Destination Factor (RGB)";
    ui_type = "drag";
> = 1.0;

// Output in cCopyBuffer
CREATE_TEXTURE(SrcTex, BUFFER_SIZE_0, RGBA8, 1)

// Inputs in cBlendBuffer
CREATE_SRGB_SAMPLER(SampleSrcTex, SrcTex, LINEAR, CLAMP)
CREATE_SRGB_SAMPLER(SampleDestTex, CShade_ColorTex, LINEAR, CLAMP)

// Pixel shaders

float4 PS_Copy(VS2PS_Quad Input) : SV_TARGET0
{
    return tex2D(CShade_SampleColorTex, Input.Tex0);
}

float4 PS_Blend(VS2PS_Quad Input) : SV_TARGET0
{
    float4 Src = tex2D(SampleSrcTex, Input.Tex0) * float4(_SrcFactor, 1.0);
    float4 Dest = tex2D(SampleDestTex, Input.Tex0) * float4(_DestFactor, 1.0);

    float4 OutputColor = 0.0;

    switch(_Blend)
    {
        case 0: // Add
            OutputColor = Src + Dest;
            break;
        case 1: // Subtract
            OutputColor = Src - Dest;
            break;
        case 2: // Multiply
            OutputColor = Src * Dest;
            break;
        case 3: // Min
            OutputColor = min(Src, Dest);
            break;
        case 4: // Max
            OutputColor = max(Src, Dest);
            break;
        case 5: // Screen
            OutputColor = (Src + Dest) - (Src * Dest);
            break;
        default:
            OutputColor = Dest;
            break;
    }

    return OutputColor;
}

technique CShade_CopyBuffer
{
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Copy;
        RenderTarget0 = SrcTex;
    }
}

technique CShade_BlendBuffer
{
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Blend;
    }
}
