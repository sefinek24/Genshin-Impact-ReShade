
namespace cDiscBlur
{
    #include "shared/cMacros.fxh"
    #include "shared/cGraphics.fxh"
    #include "shared/cImageProcessing.fxh"

    /*
        Construct options
    */

    uniform float _Offset <
        ui_label = "Sample offset";
        ui_type = "drag";
        ui_min = 0.0;
    > = 0.0;

    uniform float _Radius <
        ui_label = "Convolution radius";
        ui_type = "drag";
        ui_min = 0.0;
    > = 64.0;

    uniform int _Samples <
        ui_label = "Convolution sample count";
        ui_type = "drag";
        ui_min = 0;
    > = 16;

    CREATE_TEXTURE(Tex1, BUFFER_SIZE_1, RGBA8, 8)

    CREATE_SRGB_SAMPLER(SampleTex1, Tex1, LINEAR, CLAMP)

    // Pixel shaders
    // Repurposed Wojciech Sterna's shadow sampling code as a screen-space convolution
    // http://maxest.gct-game.net/content/chss.pdf

    float4 PS_GenMipLevels(VS2PS_Quad Input) : SV_TARGET0
    {
        return tex2D(SampleColorTex, Input.Tex0);
    }

    float4 PS_VogelBlur(VS2PS_Quad Input) : SV_TARGET0
    {
        // Initialize variables we need to accumulate samples and calculate offsets
        float4 OutputColor = 0.0;

        // LOD calculation to fill in the gaps between samples
        const float Pi = 3.1415926535897932384626433832795;
        float SampleArea = Pi * (_Radius * _Radius) / float(_Samples);
        float LOD = max(0.0, 0.5 * log2(SampleArea));

        // Offset and weighting attributes
        float2 ScreenSize = int2(BUFFER_WIDTH / 2, BUFFER_HEIGHT / 2);
        float2 PixelSize = 1.0 / ldexp(ScreenSize, -LOD);
        float Weight = 1.0 / (float(_Samples));

        for(int i = 0; i < _Samples; i++)
        {
            float2 Offset = SampleVogel(i, _Samples);
            OutputColor += tex2Dlod(SampleTex1, float4(Input.Tex0 + (Offset * PixelSize), 0.0, LOD)) * Weight;
        }

        return OutputColor;
    }

    technique cBlur
    {
        pass GenMipLevels
        {
            #if BUFFER_COLOR_BIT_DEPTH == 8
                SRGBWriteEnable = TRUE;
            #endif

            VertexShader = VS_Quad;
            PixelShader = PS_GenMipLevels;
            RenderTarget0 = Tex1;
        }

        pass VogelBlur
        {
            #if BUFFER_COLOR_BIT_DEPTH == 8
                SRGBWriteEnable = TRUE;
            #endif

            VertexShader = VS_Quad;
            PixelShader = PS_VogelBlur;
        }
    }
}