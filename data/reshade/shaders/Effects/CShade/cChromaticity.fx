
#include "shared/cGraphics.fxh"

/*
    Construct options
*/

uniform int _Select <
    ui_type = "combo";
    ui_items = " Length (RG)\0 Length (RGB)\0 Average (RG)\0 Average (RGB)\0 Sum (RG)\0 Sum (RGB)\0";
    ui_label = "Method";
    ui_tooltip = "Select Chromaticity";
> = 0;

// Pixel shaders

float4 PS_Chromaticity(VS2PS_Quad Input) : SV_TARGET0
{
    float3 Color = tex2D(CShade_SampleColorTex, Input.Tex0).rgb;
    float Sum = 0.0;
    float3 Chromaticity = 0.0;

    switch(_Select)
    {
        case 0: // Length (RG)
            Sum = length(Color.rgb);
            Chromaticity.rg = saturate(Color.rg / Sum);
            Chromaticity.rg = (Sum != 0.0) ? Chromaticity.rg : 1.0 / sqrt(3.0);
            break;
        case 1: // Length (RGB)
            Sum = length(Color.rgb);
            Chromaticity.rgb = saturate(Color.rgb / Sum);
            Chromaticity.rgb = (Sum != 0.0) ? Chromaticity.rgb : 1.0 / sqrt(3.0);
            break;
        case 2: // Average (RG)
            Sum = dot(Color.rgb, 1.0 / 3.0);
            Chromaticity.rg = saturate(Color.rg / Sum);
            Chromaticity.rg = (Sum != 0.0) ? Chromaticity.rg : 1.0;
            break;
        case 3: // Average (RGB)
            Sum = dot(Color.rgb, 1.0 / 3.0);
            Chromaticity.rgb = saturate(Color.rgb / Sum);
            Chromaticity.rgb = (Sum != 0.0) ? Chromaticity.rgb : 1.0;
            break;
        case 4: // Sum (RG)
            Sum = dot(Color.rgb, 1.0);
            Chromaticity.rg = saturate(Color.rg / Sum);
            Chromaticity.rg = (Sum != 0.0) ? Chromaticity.rg : 1.0 / 3.0;
            break;
        case 5: // Sum (RGB)
            Sum = dot(Color.rgb, 1.0);
            Chromaticity.rgb = saturate(Color.rgb / Sum);
            Chromaticity.rgb = (Sum != 0.0) ? Chromaticity.rgb : 1.0 / 3.0;
            break;
        default: // No Chromaticity
            Chromaticity.rgb = 0.0;
            break;
    }

    return float4(Chromaticity, 1.0);
}

technique CShade_Chromaticity
{
    pass
    {
        VertexShader = VS_Quad;
        PixelShader = PS_Chromaticity;
    }
}
