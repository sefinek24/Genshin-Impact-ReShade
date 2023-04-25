
#include "shared/cGraphics.fxh"

uniform float2 _Scale <
    ui_min = 0.0;
    ui_label = "Scale";
    ui_type = "drag";
> = float2(1.0, 0.8);

float4 PS_Letterbox(VS2PS_Quad Input) : SV_TARGET0
{
    // Output a rectangle
    const float2 Scale = -_Scale * 0.5 + 0.5;
    float2 Shaper  = step(Scale, Input.Tex0);
           Shaper *= step(Scale, 1.0 - Input.Tex0);
    return Shaper.xxxx * Shaper.yyyy;
}

technique CShade_LetterBox
{
    pass
    {
        // Blend the rectangle with the backbuffer
        ClearRenderTargets = FALSE;
        BlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = DESTCOLOR;
        DestBlend = ZERO;
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Letterbox;
    }
}
