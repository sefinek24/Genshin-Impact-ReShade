
#if !defined(CGRAPHICS_FXH)
    #define CGRAPHICS_FXH

    #include "cMacros.fxh"

    static const float Pi = 3.1415926535897932384626433832795;

    texture2D CShade_ColorTex : COLOR;

    sampler2D CShade_SampleColorTex
    {
        Texture = CShade_ColorTex;
        MagFilter = LINEAR;
        MinFilter = LINEAR;
        MipFilter = LINEAR;
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBTexture = TRUE;
        #endif
    };

    struct APP2VS
    {
        uint ID : SV_VERTEXID;
    };

    struct VS2PS_Quad
    {
        float4 HPos : SV_POSITION;
        float2 Tex0 : TEXCOORD0;
    };

    VS2PS_Quad VS_Quad(APP2VS Input)
    {
        VS2PS_Quad Output;
        Output.Tex0.x = (Input.ID == 2) ? 2.0 : 0.0;
        Output.Tex0.y = (Input.ID == 1) ? 2.0 : 0.0;
        Output.HPos = float4(Output.Tex0 * float2(2.0, -2.0) + float2(-1.0, 1.0), 0.0, 1.0);
        return Output;
    }

    float4 GetBlit(VS2PS_Quad Input, sampler2D SampleSource)
    {
        return tex2D(SampleSource, Input.Tex0);
    }
#endif