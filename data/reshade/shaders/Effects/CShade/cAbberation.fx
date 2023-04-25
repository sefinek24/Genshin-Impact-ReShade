
#include "shared/cGraphics.fxh"

/*
    Construct options
*/

uniform float2 _ShiftRed <
    ui_type = "drag";
> = -1.0;

uniform float2 _ShiftGreen <
    ui_type = "drag";
> = 0.0;

uniform float2 _ShiftBlue <
    ui_type = "drag";
> = 1.0;

float4 PS_Abberation(VS2PS_Quad Input) : SV_TARGET0
{
    const float2 PixelSize = float2(BUFFER_RCP_WIDTH, BUFFER_RCP_HEIGHT);

    float4 OutputColor = 0.0;

    // Shift red channel
    OutputColor.r = tex2D(CShade_SampleColorTex, Input.Tex0 + _ShiftRed * PixelSize).r;
    // Keep green channel to the center
    OutputColor.g = tex2D(CShade_SampleColorTex, Input.Tex0 + _ShiftGreen * PixelSize).g;
    // Shift blue channel
    OutputColor.b = tex2D(CShade_SampleColorTex, Input.Tex0 + _ShiftBlue * PixelSize).b;
    // Write alpha value
    OutputColor.a = 1.0;

    return OutputColor;
}

technique CShade_Abberation
{
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Abberation;
    }
}
