/*=============================================================================

    Copyright (c) Pascal Gilcher. All rights reserved.

 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential

=============================================================================*/

#pragma once

/*===========================================================================*/

namespace Whitebalance
{

float3 blackbody_xyz(float temperature) 
{
    float term = 1000.0 / temperature;

    const float4 xc_coefficients[2] = 
    {
        float4(-3.0258469, 2.1070379, 0.2226347, 0.240390), 
        float4(-0.2661293,-0.2343589, 0.8776956, 0.179910) 
    };

    const float4 yc_coefficients[3] =
    {
        float4(-1.1063814,-1.34811020, 2.18555832,-0.20219683), 
        float4(-0.9549476,-1.37418593, 2.09137015,-0.16748867), 
        float4( 3.0817580,-5.87338670, 3.75112997,-0.37001483)
    };

    float3 xyz;

    float4 xc;
    xc.w = 1.0;
    xc.xyz = term;
    xc.xy *= term;
    xc.x *= term;

    float x = dot(xc, temperature > 4000.0 ? xc_coefficients[0] : xc_coefficients[1]); //xc

    float4 yc;
    yc.w = 1.0;
    yc.xyz = x;
    yc.xy *= x;
    yc.x *= x;

    float y = dot(yc, temperature < 2222.0 ? yc_coefficients[0] : (temperature < 4000.0 ? yc_coefficients[1] : yc_coefficients[2])); //yc

    float3 XYZ;
    XYZ.y = 1.0;
    XYZ.x = XYZ.y / y * x;
    XYZ.z = XYZ.y / y * (1.0 - x - y);

    return XYZ;
}

float3x3 chromatic_adaptation(float3 xyz_src, float3 xyz_dst)
{
    //https://en.wikipedia.org/wiki/LMS_color_space
    //Hunt-Pointer-Estevez transformation, old LMS <-> XYZ, also called von Kries transform
    //using newer XYZ <-> LMS matrices won't work here
    const float3x3 xyz_to_lms = float3x3(0.4002, 0.7076, -0.0808,           
                                        -0.2263, 1.1653, 0.0457,
                                         0,0,0.9182);
    const float3x3 lms_to_xyz = float3x3(1.8601 ,  -1.1295, 0.2199,
                                        0.3612, 0.6388 , -0.0000,
                                        0, 0, 1.0891);                                

    float3 lms_src = mul(xyz_src, xyz_to_lms);
    float3 lms_dst = mul(xyz_dst, xyz_to_lms);

    float3x3 von_kries_transform = float3x3(lms_dst.x / lms_src.x, 0, 0,
                                            0, lms_dst.y / lms_src.y, 0,
                                            0, 0, lms_dst.z / lms_src.z);

    return mul(mul(xyz_to_lms, von_kries_transform), lms_to_xyz);
}

float3 set_white_balance(float3 rgb, float temperature)
{
    float3 xyz_src = blackbody_xyz(6500.0);
    float3 xyz_dst = blackbody_xyz(temperature);

    float3 adjusted = Colorspace::rgb_to_xyz(rgb);
    adjusted = mul(adjusted, chromatic_adaptation(xyz_src, xyz_dst));
    adjusted = Colorspace::xyz_to_rgb(adjusted);
    return adjusted;
}

} //Namespace
