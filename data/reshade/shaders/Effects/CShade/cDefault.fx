
float4 VS_Quad() : SV_POSITION
{
    return 0.0;
}

float4 PS_Quad() : SV_TARGET0
{
    return 0.0;
}

technique CShade_Default
{
    pass
    {
        VertexCount = 0;
        VertexShader = VS_Quad;
        PixelShader = PS_Quad;
    }
}
