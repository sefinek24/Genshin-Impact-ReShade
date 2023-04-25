
/*
    MIT License

    Copyright (C) 2015-2017 Keijiro Takahashi

    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

#include "shared/cGraphics.fxh"

/*
    Construct options
*/

uniform int _Method <
    ui_label = "Edge Detection Method";
    ui_type = "combo";
    ui_items = " ddx(), ddy()\0 Bilinear 3x3 Sobel\0 Bilinear 5x5 Prewitt\0 Bilinear 5x5 Sobel\0 3x3 Prewitt\0 3x3 Scharr\0";
> = 0;

uniform float _Threshold <
    ui_label = "Threshold";
    ui_type = "slider";
    ui_min = 0.0;
    ui_max = 1.0;
> = 0.05f;

uniform float _InverseRange <
    ui_label = "Inverse Range";
    ui_type = "slider";
    ui_min = 0.0;
    ui_max = 1.0;
> = 0.05f;

uniform float _ColorSensitivity <
    ui_label = "Color Sensitivity";
    ui_type = "slider";
    ui_min = 0.0;
    ui_max = 1.0;
> = 0.0f;

uniform float4 _FrontColor <
    ui_label = "Front Color";
    ui_type = "color";
    ui_min = 0.0;
    ui_max = 1.0;
> = float4(1.0, 1.0, 1.0, 1.0);

uniform float4 _BackColor <
    ui_label = "Back Color";
    ui_type = "color";
    ui_min = 0.0;
    ui_max = 1.0;
> = float4(0.0, 0.0, 0.0, 0.0);

struct VS2PS_Grad
{
    float4 HPos : SV_POSITION;
    float4 Tex0 : TEXCOORD0;
    float4 Tex1 : TEXCOORD1;
    float4 Tex2 : TEXCOORD2;
    float4 Tex3 : TEXCOORD3;
};

VS2PS_Grad VS_Grad(APP2VS Input)
{
	const float2 PixelSize = float2(BUFFER_RCP_WIDTH, BUFFER_RCP_HEIGHT);

    VS2PS_Quad FSQuad = VS_Quad(Input);

    VS2PS_Grad Output;

    Output.HPos = FSQuad.HPos;
    Output.Tex0 = FSQuad.Tex0.xyxy;

    switch(_Method)
    {
        case 0: // fwidth()
            Output.Tex1 = FSQuad.Tex0.xyxy;
            break;
        case 1: // Bilinear 3x3 Sobel
            Output.Tex1 = FSQuad.Tex0.xyxy + (float4(-0.5, -0.5, 0.5, 0.5) * PixelSize.xyxy);
            break;
        case 2: // Bilinear 5x5 Prewitt
            Output.Tex1 = FSQuad.Tex0.xyyy + (float4(-1.5, 1.5, 0.0, -1.5) * PixelSize.xyyy);
            Output.Tex2 = FSQuad.Tex0.xyyy + (float4( 0.0, 1.5, 0.0, -1.5) * PixelSize.xyyy);
            Output.Tex3 = FSQuad.Tex0.xyyy + (float4( 1.5, 1.5, 0.0, -1.5) * PixelSize.xyyy);
            break;
        case 3: // Bilinear 5x5 Sobel
            Output.Tex1 = FSQuad.Tex0.xxyy + (float4(-1.5, 1.5, -0.5, 0.5) * PixelSize.xxyy);
            Output.Tex2 = FSQuad.Tex0.xxyy + (float4(-0.5, 0.5, -1.5, 1.5) * PixelSize.xxyy);
            break;
        case 4: // 3x3 Prewitt
            Output.Tex1 = FSQuad.Tex0.xyyy + (float4(-1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
            Output.Tex2 = FSQuad.Tex0.xyyy + (float4(0.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
            Output.Tex3 = FSQuad.Tex0.xyyy + (float4(1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
            break;
        case 5: // 3x3 Scharr
            Output.Tex1 = FSQuad.Tex0.xyyy + (float4(-1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
            Output.Tex2 = FSQuad.Tex0.xyyy + (float4(0.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
            Output.Tex3 = FSQuad.Tex0.xyyy + (float4(1.0, 1.0, 0.0, -1.0) * PixelSize.xyyy);
            break;
    }

    return Output;
}

struct Grad
{
    float4 Ix;
    float4 Iy;
};

Grad GetGrad(VS2PS_Grad Input, sampler2D SampleSource)
{
    Grad Output;

    float4 A0, B0, C0;
    float4 A1, B1, C1;
    float4 A2, B2, C2;

    switch(_Method)
    {
        case 0: // ddx(), ddy()
            A0 = tex2D(SampleSource, Input.Tex1.xy).rgb;
            Output.Ix = ddx(A0);
            Output.Iy = ddy(A0);
            break;
        case 1: // Bilinear 3x3 Sobel
            A0 = tex2D(SampleSource, Input.Tex1.xw).rgb * 4.0; // <-0.5, +0.5>
            C0 = tex2D(SampleSource, Input.Tex1.zw).rgb * 4.0; // <+0.5, +0.5>
            A2 = tex2D(SampleSource, Input.Tex1.xy).rgb * 4.0; // <-0.5, -0.5>
            C2 = tex2D(SampleSource, Input.Tex1.zy).rgb * 4.0; // <+0.5, -0.5>
            Output.Ix = ((C0 + C2) - (A0 + A2));
            Output.Iy = ((A0 + C0) - (A2 + C2));
            break;
        case 2: // Bilinear 5x5 Prewitt
            // Sampler locations:
            // A0 B0 C0
            // A1    C1
            // A2 B2 C2
            A0 = tex2D(SampleSource, Input.Tex1.xy) * 4.0; // <-1.5, +1.5>
            A1 = tex2D(SampleSource, Input.Tex1.xz) * 2.0; // <-1.5,  0.0>
            A2 = tex2D(SampleSource, Input.Tex1.xw) * 4.0; // <-1.5, -1.5>
            B0 = tex2D(SampleSource, Input.Tex2.xy) * 2.0; // < 0.0, +1.5>
            B2 = tex2D(SampleSource, Input.Tex2.xw) * 2.0; // < 0.0, -1.5>
            C0 = tex2D(SampleSource, Input.Tex3.xy) * 4.0; // <+1.5, +1.5>
            C1 = tex2D(SampleSource, Input.Tex3.xz) * 2.0; // <+1.5,  0.0>
            C2 = tex2D(SampleSource, Input.Tex3.xw) * 4.0; // <+1.5, -1.5>
            Output.Ix = (C0 + C1 + C2) - (A0 + A1 + A2);
            Output.Iy = (A0 + B0 + C0) - (A2 + B2 + C2);
            break;
        case 3: // Bilinear 5x5 Sobel by CeeJayDK
            // Sampler locations:
            //   B1 B2
            // A0     A1
            // A2     B0
            //   C0 C1
            A0 = tex2D(SampleSource, Input.Tex1.xw) * 4.0; // <-1.5, +0.5>
            A1 = tex2D(SampleSource, Input.Tex1.yw) * 4.0; // <+1.5, +0.5>
            A2 = tex2D(SampleSource, Input.Tex1.xz) * 4.0; // <-1.5, -0.5>
            B0 = tex2D(SampleSource, Input.Tex1.yz) * 4.0; // <+1.5, -0.5>
            B1 = tex2D(SampleSource, Input.Tex2.xw) * 4.0; // <-0.5, +1.5>
            B2 = tex2D(SampleSource, Input.Tex2.yw) * 4.0; // <+0.5, +1.5>
            C0 = tex2D(SampleSource, Input.Tex2.xz) * 4.0; // <-0.5, -1.5>
            C1 = tex2D(SampleSource, Input.Tex2.yz) * 4.0; // <+0.5, -1.5>
            Output.Ix = (B2 + A1 + B0 + C1) - (B1 + A0 + A2 + C0);
            Output.Iy = (A0 + B1 + B2 + A1) - (A2 + C0 + C1 + B0);
            break;
        case 4: // 3x3 Prewitt
            A0 = tex2D(SampleSource, Input.Tex1.xy) * 1.0; // <-1.0, 1.0>
            A1 = tex2D(SampleSource, Input.Tex1.xz) * 1.0; // <-1.0, 0.0>
            A2 = tex2D(SampleSource, Input.Tex1.xw) * 1.0; // <-1.0, -1.0>
            B0 = tex2D(SampleSource, Input.Tex2.xy) * 1.0; // <0.0, 1.0>
            B2 = tex2D(SampleSource, Input.Tex2.xw) * 1.0; // <0.0, -1.0>
            C0 = tex2D(SampleSource, Input.Tex3.xy) * 1.0; // <1.0, 1.0>
            C1 = tex2D(SampleSource, Input.Tex3.xz) * 1.0; // <1.0, 0.0>
            C2 = tex2D(SampleSource, Input.Tex3.xw) * 1.0; // <1.0, -1.0> 
            Output.Ix = (C0 + C1 + C2) - (A0 + A1 + A2);
            Output.Iy = (A0 + B0 + C0) - (A2 + B2 + C2);
            break;
        case 5: // 3x3 Scharr
            A0 = tex2D(SampleSource, Input.Tex1.xy) * 3.0;  // <-1.0, 1.0>
            A1 = tex2D(SampleSource, Input.Tex1.xz) * 10.0; // <-1.0, 0.0>
            A2 = tex2D(SampleSource, Input.Tex1.xw) * 3.0;  // <-1.0, -1.0>
            B0 = tex2D(SampleSource, Input.Tex2.xy) * 10.0; // <0.0, 1.0>
            B2 = tex2D(SampleSource, Input.Tex2.xw) * 10.0; // <0.0, -1.0>
            C0 = tex2D(SampleSource, Input.Tex3.xy) * 3.0;  // <1.0, 1.0>
            C1 = tex2D(SampleSource, Input.Tex3.xz) * 10.0; // <1.0, 0.0>
            C2 = tex2D(SampleSource, Input.Tex3.xw) * 3.0;  // <1.0, -1.0> 
            Output.Ix = (C0 + C1 + C2) - (A0 + A1 + A2);
            Output.Iy = (A0 + B0 + C0) - (A2 + B2 + C2);
            break;
    }

    return Output;
}

float3 PS_Grad(VS2PS_Grad Input) : SV_TARGET0
{
    const float GradWeights[6] = 
    {
        1.0 / 1.0, // ddx(), ddy()
        1.0 / 4.0, // Bilinear 3x3 Sobel
        1.0 / 10.0, // Bilinear 5x5 Prewitt
        1.0 / 12.0, // Bilinear 5x5 Sobel by CeeJayDK
        1.0 / 3.0, // 3x3 Prewitt
        1.0 / 16.0, // 3x3 Scharr
    };

    Grad Grad = GetGrad(Input, SampleColorTex);
    Grad.Ix = Grad.Ix * GradWeights[_Method];
    Grad.Iy = Grad.Iy * GradWeights[_Method];

    float4 I = sqrt(dot(Grad.Ix.rgb, Grad.Ix.rgb) + dot(Grad.Iy.rgb, Grad.Iy.rgb));


    // Thresholding
    I = I * _ColorSensitivity;

    float3 Base = tex2D(SampleColorTex, Input.Tex0.xy).rgb;
    I = saturate((I - _Threshold) * _InverseRange);
    float3 BackgoundColor = lerp(Base.rgb, _BackColor.rgb, _BackColor.a);
    return lerp(BackgoundColor, _FrontColor.rgb, I.a * _FrontColor.a);
}

technique kContour
{
    pass
    {
        #if BUFFER_COLOR_BIT_DEPTH == 8
            SRGBWriteEnable = TRUE;
        #endif
        
        VertexShader = VS_Grad;
        PixelShader = PS_Grad;
    }
}
