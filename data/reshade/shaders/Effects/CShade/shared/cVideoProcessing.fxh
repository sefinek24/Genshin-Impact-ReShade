
#if !defined(CVIDEOPROCESSING_FXH)
    #define CVIDEOPROCESSING_FXH

    #include "cGraphics.fxh"

    // Lucas-Kanade optical flow with bilinear fetches

    /*
        Calculate Lucas-Kanade optical flow by solving (A^-1 * B)
        ---------------------------------------------------------
        [A11 A12]^-1 [-B1] -> [ A11/D -A12/D] [-B1]
        [A21 A22]^-1 [-B2] -> [-A21/D  A22/D] [-B2]
        ---------------------------------------------------------
        [ Ix^2/D -IxIy/D] [-IxIt]
        [-IxIy/D  Iy^2/D] [-IyIt]
    */

    struct Texel
    {
        float2 MainTex;
        float2 Size;
        float2 LOD;
    };

    struct UnpackedTex
    {
        float4 Tex;
        float4 WarpedTex;
    };

    // Unpacks and assembles a column of texture coordinates
    void UnpackTex(in Texel Tex, in float2 Vectors, out UnpackedTex Output[9])
    {
        float4 TexOffsets[3];
        TexOffsets[0] = float4(-1.0, 1.0, 0.0, -1.0);
        TexOffsets[1] = float4(0.0, 1.0, 0.0, -1.0);
        TexOffsets[2] = float4(1.0, 1.0, 0.0, -1.0);

        // Pack normalization and masking into 1 operation
        float4 TexMask = abs(Tex.Size.xyyy) * float4(1.0, 1.0, 0.0, 0.0);

        // Calculate tex columns in 1 MAD
        int Index = 0;
        int TexIndex = 0;

        while(Index < 3)
        {
            float4 ColumnTex = Tex.MainTex.xyyy + TexOffsets[Index];
            Output[TexIndex + 0].Tex = (ColumnTex.xyyy * TexMask) + Tex.LOD.xxxy;
            Output[TexIndex + 1].Tex = (ColumnTex.xzzz * TexMask) + Tex.LOD.xxxy;
            Output[TexIndex + 2].Tex = (ColumnTex.xwww * TexMask) + Tex.LOD.xxxy;

            float4 WarpedColumnTex = ColumnTex + Vectors.xyyy;
            Output[TexIndex + 0].WarpedTex = (WarpedColumnTex.xyyy * TexMask) + Tex.LOD.xxxy;
            Output[TexIndex + 1].WarpedTex = (WarpedColumnTex.xzzz * TexMask) + Tex.LOD.xxxy;
            Output[TexIndex + 2].WarpedTex = (WarpedColumnTex.xwww * TexMask) + Tex.LOD.xxxy;

            Index = Index + 1;
            TexIndex = TexIndex + 3;
        }
    }

    // [-1.0, 1.0] -> [Width, Height]
    float2 DecodeVectors(float2 Vectors, float2 ImageSize)
    {
        return Vectors / abs(ImageSize);
    }

    // [Width, Height] -> [-1.0, 1.0]
    float2 EncodeVectors(float2 Vectors, float2 ImageSize)
    {
        return clamp(Vectors * abs(ImageSize), -1.0, 1.0);
    }

    float2 GetPixelPyLK
    (
        float2 MainTex,
        float2 Vectors,
        sampler2D SampleI0_G,
        sampler2D SampleI0,
        sampler2D SampleI1,
        int Level,
        bool Coarse
    )
    {
        // Setup constants
        const int WindowSize = 9;

        // Initialize variables
        float3 A = 0.0;
        float2 B = 0.0;
        float R = 0.0;
        float IT[WindowSize];
        float Determinant = 0.0;
        float2 NewVectors = 0.0;
        const float T = 0.5;

        // Calculate main texel information (TexelSize, TexelLOD)
        Texel TexInfo;
        TexInfo.Size.x = ddx(MainTex.x);
        TexInfo.Size.y = ddy(MainTex.y);
        TexInfo.MainTex = MainTex * (1.0 / abs(TexInfo.Size));
        TexInfo.LOD = float2(0.0, float(Level));

        // Decode written vectors from coarser level
        Vectors = DecodeVectors(Vectors, TexInfo.Size);

        // The spatial(S) and temporal(T) derivative neighbors to sample
        UnpackedTex Pixel[WindowSize];
        UnpackTex(TexInfo, Vectors, Pixel);

        [unroll]
        for(int i = 0; i < WindowSize; i++)
        {
            float I0 = tex2Dlod(SampleI0, Pixel[i].Tex).r;
            float I1 = tex2Dlod(SampleI1, Pixel[i].WarpedTex).r;
            IT[i] = I0 - I1;
            R += (IT[i] * IT[i]);
        }

        bool NoRefine = (Coarse == false) && (sqrt(R / 9.0) <= T);

        [branch]
        if(!NoRefine)
        {
            [unroll]
            for(int i = 0; i < WindowSize; i++)
            {
                // A.x = A11; A.y = A22; A.z = A12/A22
                float2 G = tex2Dlod(SampleI0_G, Pixel[i].Tex).xy;
                A.xyz += (G.xyx * G.xyy);

                // B.x = B1; B.y = B2
                B += (G * IT[i]);
            }
        }

        // Create -IxIy (A12) for A^-1 and its determinant
        A.z = -A.z;

        // Calculate A^-1 determinant
        Determinant = (A.x * A.y) - (A.z * A.z);

        // Solve A^-1
        A = A / Determinant;
 
        // Calculate Lucas-Kanade matrix
        // [ Ix^2/D -IxIy/D] [-IxIt]
        // [-IxIy/D  Iy^2/D] [-IyIt]
        NewVectors = (Determinant != 0.0) ? mul(-B.xy, float2x2(A.yzzx)) : 0.0;

        // Propagate and encode vectors
        return EncodeVectors(Vectors + NewVectors, TexInfo.Size);
    }
#endif
