
namespace cBloom
{
    #include "shared/cMacros.fxh"
    #include "shared/cGraphics.fxh"

    /*
        Construct options
    */

    CREATE_OPTION(float, _Threshold, "Main", "Threshold", "drag", 1.0, 0.8)
    CREATE_OPTION(float, _Smooth, "Main", "Smoothing", "drag", 1.0, 0.5)
    CREATE_OPTION(float, _Saturation, "Main", "Saturation", "drag", 4.0, 1.0)
    CREATE_OPTION(float3, _ColorShift, "Main", "Color shift", "color", 1.0, 1.0)
    CREATE_OPTION(float, _Intensity, "Main", "Color Intensity", "drag", 4.0, 1.0)

    /*
        Construct textures and its samplers
    */

    CREATE_TEXTURE(Tex0, BUFFER_SIZE_0, RGB10A2, 1)
    CREATE_SAMPLER(SampleTex0, Tex0, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex1, BUFFER_SIZE_1, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex1, Tex1, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex2, BUFFER_SIZE_2, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex2, Tex2, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex3, BUFFER_SIZE_3, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex3, Tex3, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex4, BUFFER_SIZE_4, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex4, Tex4, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex5, BUFFER_SIZE_5, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex5, Tex5, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex6, BUFFER_SIZE_6, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex6, Tex6, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex7, BUFFER_SIZE_7, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex7, Tex7, LINEAR, CLAMP)

    CREATE_TEXTURE(Tex8, BUFFER_SIZE_8, RGBA16F, 1)
    CREATE_SAMPLER(SampleTex8, Tex8, LINEAR, CLAMP)

    /*
        Construct vertex shaders
    */

    struct VS2PS_Downscale
    {
        float4 HPos : SV_POSITION;
        float4 Tex0 : TEXCOORD0; // Quadrant
        float4 Tex1 : TEXCOORD1; // Left column
        float4 Tex2 : TEXCOORD2; // Center column
        float4 Tex3 : TEXCOORD3; // Right column
    };

    VS2PS_Downscale GetVertexDownscale(APP2VS Input, float2 PixelSize)
    {
        // Get fullscreen texcoord and vertex position
        VS2PS_Quad FSQuad = VS_Quad(Input);

        VS2PS_Downscale Output;
        Output.HPos = FSQuad.HPos;
        Output.Tex0 = FSQuad.Tex0.xyxy + (float4(-1.0, -1.0, 1.0, 1.0) * PixelSize.xyxy);
        Output.Tex1 = FSQuad.Tex0.xyyy + (float4(-2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
        Output.Tex2 = FSQuad.Tex0.xyyy + (float4(0.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
        Output.Tex3 = FSQuad.Tex0.xyyy + (float4(2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
        return Output;
    }

    #define CREATE_VS_DOWNSCALE(METHOD_NAME, INV_BUFFER_SIZE) \
        VS2PS_Downscale METHOD_NAME(APP2VS Input) \
        { \
            return GetVertexDownscale(Input, INV_BUFFER_SIZE); \
        } \

    CREATE_VS_DOWNSCALE(VS_Downscale1, 1.0 / BUFFER_SIZE_0)
    CREATE_VS_DOWNSCALE(VS_Downscale2, 1.0 / BUFFER_SIZE_1)
    CREATE_VS_DOWNSCALE(VS_Downscale3, 1.0 / BUFFER_SIZE_2)
    CREATE_VS_DOWNSCALE(VS_Downscale4, 1.0 / BUFFER_SIZE_3)
    CREATE_VS_DOWNSCALE(VS_Downscale5, 1.0 / BUFFER_SIZE_4)
    CREATE_VS_DOWNSCALE(VS_Downscale6, 1.0 / BUFFER_SIZE_5)
    CREATE_VS_DOWNSCALE(VS_Downscale7, 1.0 / BUFFER_SIZE_6)
    CREATE_VS_DOWNSCALE(VS_Downscale8, 1.0 / BUFFER_SIZE_7)

    struct VS2PS_Upscale
    {
        float4 HPos : SV_POSITION;
        float4 Tex0 : TEXCOORD0; // Left column
        float4 Tex1 : TEXCOORD1; // Center column
        float4 Tex2 : TEXCOORD2; // Right column
    };

    VS2PS_Upscale GetVertexUpscale(APP2VS Input, float2 PixelSize)
    {
        // Get fullscreen texcoord and vertex position
        VS2PS_Quad FSQuad = VS_Quad(Input);

        VS2PS_Upscale Output;
        Output.HPos = FSQuad.HPos;
        Output.Tex0 = FSQuad.Tex0.xyyy + (float4(-2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
        Output.Tex1 = FSQuad.Tex0.xyyy + (float4(0.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
        Output.Tex2 = FSQuad.Tex0.xyyy + (float4(2.0, 2.0, 0.0, -2.0) * PixelSize.xyyy);
        return Output;
    }

    #define CREATE_VS_UPSCALE(METHOD_NAME, INV_BUFFER_SIZE) \
        VS2PS_Upscale METHOD_NAME(APP2VS Input) \
        { \
            return GetVertexUpscale(Input, INV_BUFFER_SIZE); \
        } \

    CREATE_VS_UPSCALE(VS_Upscale7, 1.0 / BUFFER_SIZE_7)
    CREATE_VS_UPSCALE(VS_Upscale6, 1.0 / BUFFER_SIZE_6)
    CREATE_VS_UPSCALE(VS_Upscale5, 1.0 / BUFFER_SIZE_5)
    CREATE_VS_UPSCALE(VS_Upscale4, 1.0 / BUFFER_SIZE_4)
    CREATE_VS_UPSCALE(VS_Upscale3, 1.0 / BUFFER_SIZE_3)
    CREATE_VS_UPSCALE(VS_Upscale2, 1.0 / BUFFER_SIZE_2)
    CREATE_VS_UPSCALE(VS_Upscale1, 1.0 / BUFFER_SIZE_1)

    /*
        Construct pixel shaders
    */

    // Pixel shaders
    // Thresholding: https://github.com/keijiro/Kino [MIT]
    // Tonemapping: https://github.com/TheRealMJP/BakingLab [MIT]

    float Med3(float x, float y, float z)
    {
        return max(min(x, y), min(max(x, y), z));
    }

    float4 PS_Prefilter(VS2PS_Quad Input) : SV_TARGET0
    {
        const float Knee = mad(_Threshold, _Smooth, 1e-5);
        const float3 Curve = float3(_Threshold - Knee, Knee * 2.0, 0.25 / Knee);
        float4 Color = tex2D(SampleColorTex, Input.Tex0);

        // Under-threshold
        float Brightness = Med3(Color.r, Color.g, Color.b);
        float Response_Curve = clamp(Brightness - Curve.x, 0.0, Curve.y);
        Response_Curve = Curve.z * Response_Curve * Response_Curve;

        // Combine and apply the brightness response curve
        Color = Color * max(Response_Curve, Brightness - _Threshold) / max(Brightness, 1e-10);
        Brightness = Med3(Color.r, Color.g, Color.b);
        return float4(saturate(lerp(Brightness, Color.rgb, _Saturation)) * _ColorShift, 1.0);
    }

    float4 GetPixelDownscale(VS2PS_Downscale Input, sampler2D SampleSource)
    {
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

        const float2 Weights = float2(0.5, 0.125) / 4.0;
        float4 OutputColor = 0.0;
        OutputColor += (D0 + D1 + D2 + D3) * Weights[0];
        OutputColor += (A0 + B0 + A1 + B1) * Weights[1];
        OutputColor += (B0 + C0 + B1 + C1) * Weights[1];
        OutputColor += (A1 + B1 + A2 + B2) * Weights[1];
        OutputColor += (B1 + C1 + B2 + C2) * Weights[1];
        return OutputColor;
    }

    #define CREATE_PS_DOWNSCALE(METHOD_NAME, SAMPLER) \
        float4 METHOD_NAME(VS2PS_Downscale Input) : SV_TARGET0 \
        { \
            return GetPixelDownscale(Input, SAMPLER); \
        }

    CREATE_PS_DOWNSCALE(PS_Downscale1, SampleTex0)
    CREATE_PS_DOWNSCALE(PS_Downscale2, SampleTex1)
    CREATE_PS_DOWNSCALE(PS_Downscale3, SampleTex2)
    CREATE_PS_DOWNSCALE(PS_Downscale4, SampleTex3)
    CREATE_PS_DOWNSCALE(PS_Downscale5, SampleTex4)
    CREATE_PS_DOWNSCALE(PS_Downscale6, SampleTex5)
    CREATE_PS_DOWNSCALE(PS_Downscale7, SampleTex6)
    CREATE_PS_DOWNSCALE(PS_Downscale8, SampleTex7)

    float4 GetPixelUpscale(VS2PS_Upscale Input, sampler2D SampleSource)
    {
        // A0 B0 C0
        // A1 B1 C1
        // A2 B2 C2

        float4 A0 = tex2D(SampleSource, Input.Tex0.xy);
        float4 A1 = tex2D(SampleSource, Input.Tex0.xz);
        float4 A2 = tex2D(SampleSource, Input.Tex0.xw);

        float4 B0 = tex2D(SampleSource, Input.Tex1.xy);
        float4 B1 = tex2D(SampleSource, Input.Tex1.xz);
        float4 B2 = tex2D(SampleSource, Input.Tex1.xw);

        float4 C0 = tex2D(SampleSource, Input.Tex2.xy);
        float4 C1 = tex2D(SampleSource, Input.Tex2.xz);
        float4 C2 = tex2D(SampleSource, Input.Tex2.xw);

        float3 Weights = float3(1.0, 2.0, 4.0) / 16.0;
        float4 OutputColor = 0.0;
        OutputColor += ((A0 + C0 + A2 + C2) * Weights[0]);
        OutputColor += ((A1 + B0 + C1 + B2) * Weights[1]);
        OutputColor += (B1 * Weights[2]);
        return OutputColor;
    }

    #define CREATE_PS_UPSCALE(METHOD_NAME, SAMPLER) \
        float4 METHOD_NAME(VS2PS_Upscale Input) : SV_TARGET0 \
        { \
            return GetPixelUpscale(Input, SAMPLER); \
        }

    CREATE_PS_UPSCALE(PS_Upscale7, SampleTex8)
    CREATE_PS_UPSCALE(PS_Upscale6, SampleTex7)
    CREATE_PS_UPSCALE(PS_Upscale5, SampleTex6)
    CREATE_PS_UPSCALE(PS_Upscale4, SampleTex5)
    CREATE_PS_UPSCALE(PS_Upscale3, SampleTex4)
    CREATE_PS_UPSCALE(PS_Upscale2, SampleTex3)
    CREATE_PS_UPSCALE(PS_Upscale1, SampleTex2)

    // ACES Filmic tonemap operator
    // https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
    float3 ToneMapACESFilmic(float3 x)
    {
        float a = 2.51f;
        float b = 0.03f;
        float c = 2.43f;
        float d = 0.59f;
        float e = 0.14f;
        return saturate((x * (a * x + b)) / (x * (c * x + d) + e));
    }

    float4 PS_Composite(VS2PS_Quad Input) : SV_TARGET0
    {
        float3 BaseColor = tex2D(SampleColorTex, Input.Tex0).rgb;
        float3 BloomColor = tex2D(SampleTex1, Input.Tex0).rgb;

        float4 Color = 1.0;
        Color.rgb = ToneMapACESFilmic(BaseColor + (BloomColor* _Intensity));
        return Color;
    }

    #define CREATE_PASS(VERTEX_SHADER, PIXEL_SHADER, RENDER_TARGET, IS_ADDITIVE) \
        pass \
        { \
            ClearRenderTargets = FALSE; \
            BlendEnable = IS_ADDITIVE; \
            BlendOp = ADD; \
            SrcBlend = ONE; \
            DestBlend = ONE; \
            VertexShader = VERTEX_SHADER; \
            PixelShader = PIXEL_SHADER; \
            RenderTarget0 = RENDER_TARGET; \
        }

    technique cBloom
    {
        CREATE_PASS(VS_Quad, PS_Prefilter, Tex0, FALSE)

        CREATE_PASS(VS_Downscale1, PS_Downscale1, Tex1, FALSE)
        CREATE_PASS(VS_Downscale2, PS_Downscale2, Tex2, FALSE)
        CREATE_PASS(VS_Downscale3, PS_Downscale3, Tex3, FALSE)
        CREATE_PASS(VS_Downscale4, PS_Downscale4, Tex4, FALSE)
        CREATE_PASS(VS_Downscale5, PS_Downscale5, Tex5, FALSE)
        CREATE_PASS(VS_Downscale6, PS_Downscale6, Tex6, FALSE)
        CREATE_PASS(VS_Downscale7, PS_Downscale7, Tex7, FALSE)
        CREATE_PASS(VS_Downscale8, PS_Downscale8, Tex8, FALSE)

        CREATE_PASS(VS_Upscale7, PS_Upscale7, Tex7, TRUE)
        CREATE_PASS(VS_Upscale6, PS_Upscale6, Tex6, TRUE)
        CREATE_PASS(VS_Upscale5, PS_Upscale5, Tex5, TRUE)
        CREATE_PASS(VS_Upscale4, PS_Upscale4, Tex4, TRUE)
        CREATE_PASS(VS_Upscale3, PS_Upscale3, Tex3, TRUE)
        CREATE_PASS(VS_Upscale2, PS_Upscale2, Tex2, TRUE)
        CREATE_PASS(VS_Upscale1, PS_Upscale1, Tex1, TRUE)

        pass
        {
            ClearRenderTargets = FALSE;
            #if BUFFER_COLOR_BIT_DEPTH == 8
                SRGBWriteEnable = TRUE;
            #endif
            VertexShader = VS_Quad;
            PixelShader = PS_Composite;
        }
    }
}