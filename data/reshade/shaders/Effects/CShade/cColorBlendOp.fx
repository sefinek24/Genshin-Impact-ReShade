
#include "shared/cGraphics.fxh"

/*
    Construct options
*/

uniform float4 _Color <
    ui_min = 0.0;
    ui_label = "Color";
    ui_type = "color";
> = 1.0;

float4 PS_Color(VS2PS_Quad Input) : SV_TARGET0
{
    return _Color;
}

// Use BlendOp to multiple the backbuffer with this quad's color
technique CShade_ColorBlendOp
{
    pass
    {
        BlendEnable = TRUE;
        BlendOp = ADD;
        SrcBlend = DESTCOLOR;
        DestBlend = SRCALPHA;
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Color;
    }
}
