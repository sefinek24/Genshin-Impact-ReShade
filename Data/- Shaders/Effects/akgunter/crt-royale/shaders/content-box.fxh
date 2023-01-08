#ifndef _CONTENT_BOX_H
#define _CONTENT_BOX_H

/////////////////////////////////  MIT LICENSE  ////////////////////////////////

//  Copyright (C) 2020 Alex Gunter
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to
//  deal in the Software without restriction, including without limitation the
//  rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
//  sell copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
//  IN THE SOFTWARE.


#include "shared-objects.fxh"


void contentCropVS(
    in uint id : SV_VertexID,

    out float4 position : SV_Position,
    out float2 texcoord : TEXCOORD0
) {
	texcoord.x = (id & 1) ? content_right : content_left;
	texcoord.y = (id & 2) ? content_lower : content_upper;

	position.x = (id & 1) ?  1 : -1;
	position.y = (id & 2) ? -1 :  1;
	position.zw = 1;
}

void contentUncropVS(
    in uint id : SV_VertexID,

    out float4 position : SV_Position,
    out float2 texcoord : TEXCOORD0
) {
	texcoord.x = id & 1;
	texcoord.y = !(id & 2);
	
	position.x = (id & 1) ? content_scale.x : -content_scale.x;
	position.y = (id & 2) ? content_scale.y : -content_scale.y;
	position.zw = 1;
}

void uncropContentPixelShader(
    in float4 pos : SV_Position,
    in float2 texcoord : TEXCOORD0,

    out float4 color : SV_Target
) {
    color = tex2D(samplerGeometry, texcoord);
}


#if CONTENT_BOX_VISIBLE
    #ifndef CONTENT_BOX_INSCRIBED
        #define CONTENT_BOX_INSCRIBED 1
    #endif

    #ifndef CONTENT_BOX_THICKNESS
        #define CONTENT_BOX_THICKNESS 5
    #endif

    #ifndef CONTENT_BOX_COLOR_R
        #define CONTENT_BOX_COLOR_R 1.0
    #endif

    #ifndef CONTENT_BOX_COLOR_G
        #define CONTENT_BOX_COLOR_G 0.0
    #endif

    #ifndef CONTENT_BOX_COLOR_B
        #define CONTENT_BOX_COLOR_B 0.0
    #endif

    static const float vert_line_thickness = float(CONTENT_BOX_THICKNESS) / BUFFER_WIDTH;
    static const float horiz_line_thickness = float(CONTENT_BOX_THICKNESS) / BUFFER_HEIGHT;

    #if CONTENT_BOX_INSCRIBED
        // Set the outer borders to the edge of the content
        static const float left_line_1 = content_left;
        static const float left_line_2 = left_line_1 + vert_line_thickness;
        static const float right_line_2 = content_right;
        static const float right_line_1 = right_line_2 - vert_line_thickness;

        static const float upper_line_1 = content_upper;
        static const float upper_line_2 = upper_line_1 + horiz_line_thickness;
        static const float lower_line_2 = content_lower;
        static const float lower_line_1 = lower_line_2 - horiz_line_thickness;
    #else
        // Set the inner borders to the edge of the content
        static const float left_line_2 = content_left;
        static const float left_line_1 = left_line_2 - vert_line_thickness;
        static const float right_line_1 = content_right;
        static const float right_line_2 = right_line_1 + vert_line_thickness;

        static const float upper_line_2 = content_upper;
        static const float upper_line_1 = upper_line_2 - horiz_line_thickness;
        static const float lower_line_1 = content_lower;
        static const float lower_line_2 = lower_line_1 + horiz_line_thickness;
    #endif


    static const float4 box_color = float4(
        CONTENT_BOX_COLOR_R,
        CONTENT_BOX_COLOR_G,
        CONTENT_BOX_COLOR_B,
        1.0
    );

    void contentBoxPixelShader(
        in float4 pos : SV_Position,
        in float2 texcoord : TEXCOORD0,

        out float4 color : SV_Target
    ) {

        const bool is_inside_outerbound = (
            texcoord.x >= left_line_1 && texcoord.x <= right_line_2 &&
            texcoord.y >= upper_line_1 && texcoord.y <= lower_line_2
        );
        const bool is_outside_innerbound = (
            texcoord.x <= left_line_2 || texcoord.x >= right_line_1 ||
            texcoord.y <= upper_line_2 || texcoord.y >= lower_line_1
        );

        if (is_inside_outerbound && is_outside_innerbound) {
            color = box_color;
        }
        else {
            color = tex2D(ReShade::BackBuffer, texcoord);
        }
    }


#endif  // CONTENT_BOX_VISIBLE
#endif  //  _CONTENT_BOX_H