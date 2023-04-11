namespace cPyramidBlur
{
    #include "shared/cMacros.fxh"
    #include "shared/cGraphics.fxh"

    /*
        Construct options
    */

    uniform int _Downscale <
        ui_type = "combo";
        ui_items = " 2x2 Box\0 3x3 Tent\0 Jorge\0 Kawase\0";
        ui_label = "Downscale kernel";
    > = 0;

    uniform int _Upscale <
        ui_type = "combo";
        ui_items = " 2x2 Box\0 3x3 Tent\0 Jorge\0 Kawase\0";
        ui_label = "Upscale kernel";
    > = 0;

    CREATE_TEXTURE(Tex1, BUFFER_SIZE_1, RGBA8, 1)
    CREATE_SRGB_SAMPLER(SampleTex1, Tex1, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex2, BUFFER_SIZE_2, RGBA8, 1)
    CREATE_SRGB_SAMPLER(SampleTex2, Tex2, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex3, BUFFER_SIZE_3, RGBA8, 1)
    CREATE_SRGB_SAMPLER(SampleTex3, Tex3, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex4, BUFFER_SIZE_4, RGBA8, 1)
    CREATE_SRGB_SAMPLER(SampleTex4, Tex4, LINEAR, CLAMP)

    // Vertex shaders

    struct VS2PS_Scale
    {
        float4 HPos : SV_POSITION;
        float4 Tex0 : TEXCOORD0;
        float4 Tex1 : TEXCOORD1;
        float4 Tex2 : TEXCOORD2;
        float4 Tex3 : TEXCOORD3;
    };

    VS2PS_Scale GetVertexScale(APP2VS Input, float2 PixelSize, int ScaleMethod)
    {
        VS2PS_Quad FSQuad = VS_Quad(Input);

        VS2PS_Scale Output;

        Output.HPos = FSQuad.HPos;

        switch(ScaleMethod)
        {
            case 0: // 4x4 Box
                Output.Tex0 = FSQuad.Tex0.xyxy + (float4(-1.0, -1.0, 1.0, 1.0) * PixelSize.xyxy);
                break;
            case 1: // 6x6 Tent
                Output.Tex0 = FSQuad.Tex0.xyyy + (float4(-2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
                Output.Tex1 = FSQuad.Tex0.xyyy + (float4(0.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
                Output.Tex2 = FSQuad.Tex0.xyyy + (float4(2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
                break;
            case 2: // 6x6 Jorge
                Output.Tex0 = FSQuad.Tex0.xyxy + (float4(-1.0, -1.0, 1.0, 1.0) * PixelSize.xyxy);
                Output.Tex1 = FSQuad.Tex0.xyyy + (float4(-2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
                Output.Tex2 = FSQuad.Tex0.xyyy + (float4(0.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
                Output.Tex3 = FSQuad.Tex0.xyyy + (float4(2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
                break;
            case 3: // 4x4 Kawase
                Output.Tex0 = FSQuad.Tex0.xyxy + (float4(0.0, 0.0, 0.0, 0.0) * PixelSize.xyxy);
                Output.Tex1 = FSQuad.Tex0.xyxy + (float4(-1.0, -1.0, 1.0, 1.0) * PixelSize.xyxy);
                break;
        }

        return Output;
    }

    #define CREATE_VS_DOWNSCALE(METHOD_NAME, INV_BUFFER_SIZE) \
        VS2PS_Scale METHOD_NAME(APP2VS Input) \
        { \
            return GetVertexScale(Input, INV_BUFFER_SIZE, _Downscale); \
        }

    CREATE_VS_DOWNSCALE(VS_Downscale1, 1.0 / BUFFER_SIZE_0)
    CREATE_VS_DOWNSCALE(VS_Downscale2, 1.0 / BUFFER_SIZE_1)
    CREATE_VS_DOWNSCALE(VS_Downscale3, 1.0 / BUFFER_SIZE_2)
    CREATE_VS_DOWNSCALE(VS_Downscale4, 1.0 / BUFFER_SIZE_3)

    #define CREATE_VS_UPSCALE(METHOD_NAME, INV_BUFFER_SIZE) \
        VS2PS_Scale METHOD_NAME(APP2VS Input) \
        { \
            return GetVertexScale(Input, INV_BUFFER_SIZE, _Upscale); \
        }

    CREATE_VS_UPSCALE(VS_Upscale3, 1.0 / BUFFER_SIZE_3)
    CREATE_VS_UPSCALE(VS_Upscale2, 1.0 / BUFFER_SIZE_2)
    CREATE_VS_UPSCALE(VS_Upscale1, 1.0 / BUFFER_SIZE_1)
    CREATE_VS_UPSCALE(VS_Upscale0, 1.0 / BUFFER_SIZE_0)

    // Pixel Shaders
    // 1: https://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/
    // 2: http://www.iryoku.com/next-generation-post-processing-in-call-of-duty-advanced-warfare
    // 3: https://community.arm.com/cfs-file/__key/communityserver-blogs-components-weblogfiles/00-00-00-20-66/siggraph2015_2D00_mmg_2D00_marius_2D00_slides.pdf
    // More: https://github.com/powervr-graphics/Native_SDK

    float4 GetPixelScale(VS2PS_Scale Input, sampler2D SampleSource, int ScaleMethod)
    {
        float4 OutputColor = 0.0;

        float3 Weights = 0.0;

        switch(ScaleMethod)
        {
            case 0: // 2x2 Box
                Weights = float3(1.0, 0.0, 0.0) / 4.0;
                OutputColor += (tex2D(SampleSource, Input.Tex0.xw) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex0.zw) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex0.xy) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex0.zy) * Weights[0]);
                break;
            case 1: // 3x3 Tent
                // Sampler locations
                // A0 B0 C0
                // A1 B1 C1
                // A2 B2 C2
                Weights = float3(1.0, 2.0, 4.0) / 16.0;
                OutputColor += (tex2D(SampleSource, Input.Tex0.xy) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.xy) * Weights[1]);
                OutputColor += (tex2D(SampleSource, Input.Tex2.xy) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex0.xz) * Weights[1]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.xz) * Weights[2]);
                OutputColor += (tex2D(SampleSource, Input.Tex2.xz) * Weights[1]);
                OutputColor += (tex2D(SampleSource, Input.Tex0.xw) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.xw) * Weights[1]);
                OutputColor += (tex2D(SampleSource, Input.Tex2.xw) * Weights[0]);
                break;
            case 2: // Jorge
                // Sampler locations
                // A0    B0    C0
                //    D0    D1
                // A1    B1    C1
                //    D2    D3
                // A2    B2    C2
                float4 D0 = tex2D(SampleSource, Input.Tex0.xw);
                float4 D1 = tex2D(SampleSource, Input.Tex0.zw);
                float4 D2 = tex2D(SampleSource, Input.Tex0.xy);
                float4 D3 = tex2D(SampleSource, Input.Tex0.zy);

                float4 A0 = tex2D(SampleSource, Input.Tex1.xy);
                float4 A1 = tex2D(SampleSource, Input.Tex1.xz);
                float4 A2 = tex2D(SampleSource, Input.Tex1.xw);

                float4 B0 = tex2D(SampleSource, Input.Tex2.xy);
                float4 B1 = tex2D(SampleSource, Input.Tex2.xz);
                float4 B2 = tex2D(SampleSource, Input.Tex2.xw);

                float4 C0 = tex2D(SampleSource, Input.Tex3.xy);
                float4 C1 = tex2D(SampleSource, Input.Tex3.xz);
                float4 C2 = tex2D(SampleSource, Input.Tex3.xw);

                Weights = float3(0.125, 0.5, 0.0) / 4.0;
                OutputColor += ((D0 + D1 + D2 + D3) * Weights[1]);
                OutputColor += ((A0 + B0 + A1 + B1) * Weights[0]);
                OutputColor += ((B0 + C0 + B1 + C1) * Weights[0]);
                OutputColor += ((A1 + B1 + A2 + B2) * Weights[0]);
                OutputColor += ((B1 + C1 + B2 + C2) * Weights[0]);
                break;
            case 3: // Kawase
                Weights = float3(1.0, 4.0, 0.0) / 8.0;
                OutputColor += (tex2D(SampleSource, Input.Tex0.xy) * Weights[1]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.xw) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.zw) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.xy) * Weights[0]);
                OutputColor += (tex2D(SampleSource, Input.Tex1.zy) * Weights[0]);
                break;
        }

        OutputColor.a = 1.0;

        return OutputColor;
    }

    #define CREATE_PS_DOWNSCALE(METHOD_NAME, SAMPLER) \
        float4 METHOD_NAME(VS2PS_Scale Input) : SV_TARGET0 \
        { \
            return GetPixelScale(Input, SAMPLER, _Downscale); \
        }

    CREATE_PS_DOWNSCALE(PS_Downscale1, SampleColorTex)
    CREATE_PS_DOWNSCALE(PS_Downscale2, SampleTex1)
    CREATE_PS_DOWNSCALE(PS_Downscale3, SampleTex2)
    CREATE_PS_DOWNSCALE(PS_Downscale4, SampleTex3)

    #define CREATE_PS_UPSCALE(METHOD_NAME, SAMPLER) \
        float4 METHOD_NAME(VS2PS_Scale Input) : SV_TARGET0 \
        { \
            return GetPixelScale(Input, SAMPLER, _Upscale); \
        }

    CREATE_PS_UPSCALE(PS_Upscale3, SampleTex4)
    CREATE_PS_UPSCALE(PS_Upscale2, SampleTex3)
    CREATE_PS_UPSCALE(PS_Upscale1, SampleTex2)
    CREATE_PS_UPSCALE(PS_Upscale0, SampleTex1)

    #if BUFFER_COLOR_BIT_DEPTH == 8
        #define WRITE_SRGB TRUE
    #endif

    #define CREATE_PASS(VERTEX_SHADER, PIXEL_SHADER, RENDER_TARGET) \
        pass \
        { \
            VertexShader = VERTEX_SHADER; \
            PixelShader = PIXEL_SHADER; \
            RenderTarget0 = RENDER_TARGET; \
            SRGBWriteEnable = WRITE_SRGB; \
        }

    technique cDualFilter
    {
        CREATE_PASS(VS_Downscale1, PS_Downscale1, Tex1)
        CREATE_PASS(VS_Downscale2, PS_Downscale2, Tex2)
        CREATE_PASS(VS_Downscale3, PS_Downscale3, Tex3)
        CREATE_PASS(VS_Downscale4, PS_Downscale4, Tex4)

        CREATE_PASS(VS_Upscale3, PS_Upscale3, Tex3)
        CREATE_PASS(VS_Upscale2, PS_Upscale2, Tex2)
        CREATE_PASS(VS_Upscale1, PS_Upscale1, Tex1)

        pass
        {
            VertexShader = VS_Upscale0;
            PixelShader = PS_Upscale0;
            SRGBWriteEnable = WRITE_SRGB;
        }
    }
}