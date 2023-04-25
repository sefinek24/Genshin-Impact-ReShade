
/*
    MIT License

    Copyright (C) 2015 Keijiro Takahashi

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

uniform float _Divisor <
    ui_type = "drag";
> = 0.05f;

uniform float _Offset <
    ui_type = "drag";
> = 0.05f;

uniform float _Roll <
    ui_type = "drag";
> = 0.0f;

uniform bool _Symmetry <
> = true;

float4 PS_Mirror(VS2PS_Quad Input) : SV_TARGET0
{
    // Convert to polar coordinates
    float2 Polar = (Input.Tex0 * 2.0) - 1.0;
    float Phi = atan2(Polar.y, Polar.x);
    float Radius = length(Polar);

    // Angular repeating.
    Phi += _Offset;
    Phi = Phi - _Divisor * floor(Phi / _Divisor);
    Phi = (_Symmetry) ? min(Phi, _Divisor - Phi) : Phi;
    Phi += _Roll - _Offset;

    // Convert back to the texture coordinate.
    float2 PhiSinCos; sincos(Phi, PhiSinCos.x, PhiSinCos.y);
    Input.Tex0 = ((PhiSinCos.yx * Radius) * 0.5) + 0.5;

    // Reflection at the border of the screen.
    Input.Tex0 = max(min(Input.Tex0, 2.0 - Input.Tex0), -Input.Tex0);
    return tex2D(CShade_SampleColorTex, Input.Tex0);
}

technique CShade_KinoMirror
{
    pass
    {
        SRGBWriteEnable = WRITE_SRGB;

        VertexShader = VS_Quad;
        PixelShader = PS_Mirror;
    }
}
