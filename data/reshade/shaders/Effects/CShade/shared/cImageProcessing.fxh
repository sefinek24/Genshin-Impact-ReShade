
#if !defined(CIMAGEPROCESSING_FXH)
    #define CIMAGEPROCESSING_FXH

    #include "shared/cGraphics.fxh"

    // Linear Gaussian blur based on https://www.rastergrid.com/blog/2010/09/efficient-Gaussian-blur-with-linear-sampling/

    float GetGaussianWeight(float SampleIndex, float Sigma)
    {
        const float Pi = 3.1415926535897932384626433832795f;
        float Output = rsqrt(2.0 * Pi * (Sigma * Sigma));
        return Output * exp(-(SampleIndex * SampleIndex) / (2.0 * Sigma * Sigma));
    }

    float GetGaussianOffset(float SampleIndex, float Sigma, out float LinearWeight)
    {
        float Offset1 = SampleIndex;
        float Offset2 = SampleIndex + 1.0;
        float Weight1 = GetGaussianWeight(Offset1, Sigma);
        float Weight2 = GetGaussianWeight(Offset2, Sigma);
        LinearWeight = Weight1 + Weight2;
        return ((Offset1 * Weight1) + (Offset2 * Weight2)) / LinearWeight;
    }

    #define BLUR9_KERNEL 10

    struct VS2PS_Blur
    {
        float4 HPos : SV_POSITION;
        float4 Tex0 : TEXCOORD0;
        float4 Tex1 : TEXCOORD1;
        float4 Tex2 : TEXCOORD2;
        float4 Tex3 : TEXCOORD3;
        float4 Tex4 : TEXCOORD4;
        float4 Tex5 : TEXCOORD5;
        float4 Tex6 : TEXCOORD6;
        float4 Tex7 : TEXCOORD7;
        float4 Tex8 : TEXCOORD8;
        float4 Tex9 : TEXCOORD9;
    };

    VS2PS_Blur GetVertexBlur(APP2VS Input, float2 PixelSize, float2 IsHorizontal)
    {
        VS2PS_Quad FSQuad = VS_Quad(Input);

        VS2PS_Blur Output;

        Output.HPos = FSQuad.HPos;

        const float BlurOffsets[BLUR9_KERNEL] =
        {
            0.0, 1.490652, 3.4781995, 5.465774, 7.45339,
            9.441065, 11.42881, 13.416645, 15.404578, 17.392626,
        };

        float4 BlurTex[BLUR9_KERNEL];
        float2 Direction = (IsHorizontal) ? float2(1.0, 0.0) : float2(0.0, 1.0);

        for(int i = 1; i < BLUR9_KERNEL; i++)
        {
            BlurTex[i].xy = FSQuad.Tex0 - ((BlurOffsets[i] * PixelSize) * Direction);
            BlurTex[i].zw = FSQuad.Tex0 + ((BlurOffsets[i] * PixelSize) * Direction);
        }

        Output.Tex0 = FSQuad.Tex0;
        Output.Tex1 = BlurTex[1];
        Output.Tex2 = BlurTex[2];
        Output.Tex3 = BlurTex[3];
        Output.Tex4 = BlurTex[4];
        Output.Tex5 = BlurTex[5];
        Output.Tex6 = BlurTex[6];
        Output.Tex7 = BlurTex[7];
        Output.Tex8 = BlurTex[8];
        Output.Tex9 = BlurTex[9];

        return Output;
    }

    float4 GetPixelBlur(VS2PS_Blur Input, sampler2D SampleSource)
    {
        float4 OutputColor = 0.0;

        float4 BlurTex[BLUR9_KERNEL] =
        {
            Input.Tex0, Input.Tex1, Input.Tex2, Input.Tex3, Input.Tex4,
            Input.Tex5, Input.Tex6, Input.Tex7, Input.Tex8, Input.Tex9,
        };

        const float BlurWeights[BLUR9_KERNEL] =
        {
            0.06299088, 0.122137636, 0.10790718, 0.08633988, 0.062565096,
            0.04105926, 0.024403222, 0.013135255, 0.006402994, 0.002826693
        };

        // Sample and weight center first to get even number sides
        float TotalWeight = BlurWeights[0];
        OutputColor = tex2D(SampleSource, BlurTex[0].xy) * BlurWeights[0];

        for(int i = 1; i < BLUR9_KERNEL; i++)
        {
            OutputColor += tex2D(SampleSource, BlurTex[i].xy) * BlurWeights[i];
            OutputColor += tex2D(SampleSource, BlurTex[i].zw) * BlurWeights[i];
            TotalWeight += BlurWeights[i] * 2.0;
        }

        // Normalize intensity to prevent altered output
        return OutputColor / TotalWeight;
    }

    // Vogel disk sampling
    // Repurposed Wojciech Sterna's shadow sampling code as a screen-space convolution
    // http://maxest.gct-game.net/content/chss.pdf
    // Vogel disk sampling: http://blog.marmakoide.org/?p=1
    // Rotated noise sampling: http://www.iryoku.com/next-generation-post-processing-in-call-of-duty-advanced-warfare (slide 123)

    float2 SampleVogel(int Index, int SamplesCount)
    {
        const float GoldenAngle = Pi * (3.0 - sqrt(5.0));
        float Radius = sqrt(float(Index) + 0.5) * rsqrt(float(SamplesCount));
        float Theta = float(Index) * GoldenAngle;

        float2 SinCosTheta = 0.0;
        SinCosTheta[0] = sin(Theta);
        SinCosTheta[1] = cos(Theta);
        return Radius * SinCosTheta;
    }

    // Color processing

    float2 NormalizeRGB(float3 Color)
    {
        return Color.xy / dot(Color, 1.0);
    }

    // 
    // RGB to saturation value.
    //
    // Golland, Polina, and Alfred M. Bruckstein. "Motion from color."
    // Computer Vision and Image Understanding 68, no. 3 (1997): 346-362.
    // http://www.cs.technion.ac.il/users/wwwb/cgi-bin/tr-get.cgi/1995/CIS/CIS9513.pdf
    //
    float SaturateRGB(float3 Color)
    {
        // Calculate min and max RGB
        float MinColor = min(min(Color.r, Color.g), Color.b);
        float MaxColor = max(max(Color.r, Color.g), Color.b);

        // Calculate normalized RGB
        float SatRGB = (MaxColor - MinColor) / MaxColor;
        SatRGB = (SatRGB != 0.0) ? SatRGB : 0.0;

        return SatRGB;
    }

    // Linear filtered Sobel filter

    struct VS2PS_Sobel
    {
        float4 HPos : SV_POSITION;
        float4 Tex0 : TEXCOORD0;
    };

    VS2PS_Sobel GetVertexSobel(APP2VS Input, float2 PixelSize)
    {
        VS2PS_Quad FSQuad = VS_Quad(Input);

        VS2PS_Sobel Output;
        Output.HPos = FSQuad.HPos;
        Output.Tex0 = FSQuad.Tex0.xyxy + (float4(-0.5, -0.5, 0.5, 0.5) * PixelSize.xyxy);
        return Output;
    }

    float2 GetPixelSobel(VS2PS_Sobel Input, sampler2D SampleSource)
    {
        float2 OutputColor0 = 0.0;
        float A = tex2D(SampleSource, Input.Tex0.xw).r * 4.0; // <-0.5, +0.5>
        float B = tex2D(SampleSource, Input.Tex0.zw).r * 4.0; // <+0.5, +0.5>
        float C = tex2D(SampleSource, Input.Tex0.xy).r * 4.0; // <-0.5, -0.5>
        float D = tex2D(SampleSource, Input.Tex0.zy).r * 4.0; // <+0.5, -0.5>
        OutputColor0.x = ((B + D) - (A + C)) / 4.0;
        OutputColor0.y = ((A + B) - (C + D)) / 4.0;
        return OutputColor0;
    }
#endif
