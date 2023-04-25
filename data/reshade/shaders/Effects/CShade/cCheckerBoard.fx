
#include "shared/cGraphics.fxh"

/*
    Construct options
*/

uniform float4 _Color1 <
    ui_min = 0.0;
    ui_label = "Color 1";
    ui_type = "color";
> = 1.0;

uniform float4 _Color2 <
    ui_min = 0.0;
    ui_label = "Color 2";
    ui_type = "color";
> = 0.0;

uniform bool _InvertCheckerboard <
    ui_type = "radio";
    ui_label = "Invert Checkerboard Pattern";
> = false;


float4 PS_Checkerboard(VS2PS_Quad Input) : SV_TARGET0
{
    float4 Checkerboard = frac(dot(Input.HPos.xy, 0.5)) * 2.0;
    Checkerboard = _InvertCheckerboard ? 1.0 - Checkerboard : Checkerboard;
    Checkerboard = Checkerboard == 1.0 ? _Color1 : _Color2;
    return Checkerboard;
}

technique cCheckerBoard
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif

        VertexShader = VS_Quad;
        PixelShader = PS_Checkerboard;
    }
}
