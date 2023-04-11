#include "Reshade.fxh"

#ifndef RES_SCALE
 #define RES_SCALE 2
#endif
#define RES_WIDTH (BUFFER_WIDTH/RES_SCALE)
#define RES_HEIGHT (BUFFER_HEIGHT/RES_SCALE)

#define BUFFER_SIZE int2(RES_WIDTH,RES_HEIGHT)
#define BUFFER_SIZE3 int3(RES_WIDTH,RES_HEIGHT,RESHADE_DEPTH_LINEARIZATION_FAR_PLANE*fDepthMultiplier/RES_SCALE)
#define NOISE_SIZE 32

#define PI 3.14159265359
#define SQRT2 1.41421356237

#define getNormal(c) (tex2Dlod(normalSampler,float4(c,0,0)).xyz-0.5)*2
#define getColorSampler(s,c) tex2Dlod(s,float4(c,0,0))
#define getColor(c) tex2Dlod(ReShade::BackBuffer,float4(c,0,0))

#define diffT(v1,v2,t) !any(max(abs(v1-v2)-t,0))

namespace DH2 {

    texture blueNoiseTex < source ="LDR_RGBA_0.png" ; > { Width = NOISE_SIZE; Height = NOISE_SIZE; MipLevels = 1; Format = RGBA8; };
    sampler blueNoiseSampler { Texture = blueNoiseTex; };

    texture roughnessTex { Width = RES_WIDTH; Height = RES_HEIGHT; Format = RGBA8; };
    sampler roughnessSampler { Texture = roughnessTex; };

    texture normalTex { Width = RES_WIDTH; Height = RES_HEIGHT; Format = RGBA8; };
    sampler normalSampler { Texture = normalTex; };

    texture lightPassTex { Width = RES_WIDTH; Height = RES_HEIGHT; Format = RGBA8; };
    sampler lightPassSampler { Texture = lightPassTex; };
    
    texture lightPassHitTex { Width = RES_WIDTH; Height = RES_HEIGHT; Format = RGBA8; };
    sampler lightPassHitSampler { Texture = lightPassHitTex; };
    
    texture lightAccuTex { Width = RES_WIDTH; Height = RES_HEIGHT; Format = RGBA8; };
    sampler lightAccuSampler { Texture = lightAccuTex; };

	texture smoothPassTex { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; Format = RGBA8; };
    sampler smoothPassSampler { Texture = smoothPassTex; };

	texture resultTex { Width = BUFFER_WIDTH; Height = BUFFER_HEIGHT; Format = RGBA8; };
    sampler resultSampler { Texture = resultTex; };

    uniform int framecount < source = "framecount"; >;


    uniform bool bDebug <
        ui_category = "Setting";
        ui_label = "Display reflection only";
    > = false;

    uniform bool bRoughness <
        ui_type = "slider";
        ui_category = "Roughness";
        ui_label = "Enable";
    > = true;
    
    uniform int iRoughnessRadius <
        ui_type = "slider";
        ui_category = "Roughness";
        ui_label = "Radius";
        ui_min = 1; ui_max = 4;
        ui_step = 1;
    > = 2;
    
    uniform float fRoughnessReflexivity <
        ui_type = "slider";
        ui_category = "Roughness";
        ui_label = "Intensity";
        ui_min = 0; ui_max = 4.0;
        ui_step = 0.01;
    > = 1.0;
	
    uniform float fDepthMultiplier <
        ui_type = "slider";
        ui_category = "Setting";
        ui_label = "Depth multiplier";
        ui_min = 0.1; ui_max = 100;
        ui_step = 0.1;
    > = 1.0;

    uniform int iFrameAccu <
        ui_type = "slider";
        ui_category = "Setting";
        ui_label = "Temporal accumulation";
        ui_min = 1; ui_max = 16;
        ui_step = 1;
    > = 3;
    
    uniform float fSkyDepth <
        ui_type = "slider";
        ui_category = "Setting";
        ui_label = "Sky Depth ";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.99;
    
    uniform float fSkyFading <
        ui_type = "slider";
        ui_category = "Setting";
        ui_label = "Sky fading";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.75;


    
// RAY TRACING

    uniform bool bRayUntilHit <
        ui_category = "Ray tracing";
        ui_label = "Search until hit";
    > = false;
    
    uniform bool bRayExtrapolate <
        ui_category = "Ray tracing";
        ui_label = "Search Extrapolate";
    > = true;
    
    uniform int iRayDistance <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Search Distance";
        ui_min = 1; ui_max = BUFFER_WIDTH;
        ui_step = 1;
    > = 200;
    
    uniform int iRayPreciseHit <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Precise hit passes";
        ui_min = 0; ui_max = 8;
        ui_step = 1;
    > = 4;
    
    uniform int iRayPreciseHitSteps <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Precise hit steps";
        ui_min = 2; ui_max = 8;
        ui_step = 1;
    > = 4;


    uniform float fRayStepPrecision <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Step Precision";
        ui_min = 0.1; ui_max = 20.0;
        ui_step = 0.1;
    > = 20.0;
    
    uniform float fRayStepMultiply <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Step multiply";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.30;

    uniform float fRayHitDepthThreshold <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Ray Hit Depth Threshold";
        ui_min = 0.01; ui_max = 10;
        ui_step = 0.01;
    > = 0.25;
  
    
    uniform float fJitter <
        ui_type = "slider";
        ui_category = "Ray tracing";
        ui_label = "Jitter";
        ui_min = 0.0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.5;
    
// LIGHT COLOR
    
    uniform bool bOrientation <
        ui_category = "Ray color";
        ui_label = "Orientation reflexivity";
    > = true;
    
	uniform float fOrientationBias <
        ui_type = "slider";
        ui_category = "Ray color";
        ui_label = "Orientation bias";
        ui_min = 0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.50;
    
    uniform float fRayBounce <
        ui_type = "slider";
        ui_category = "Ray color";
        ui_label = "Bounce strength";
        ui_min = 0; ui_max = 1.0;
        ui_step = 0.01;
    > = 0.1;
    
    uniform float fOutRatio <
        ui_type = "slider";
        ui_category = "Ray color";
        ui_label = "Out screen brightness";
        ui_min = 0.0; ui_max = 1;
        ui_step = 0.01;
    > = 0.75;
        
    uniform float fFadePower <
        ui_type = "slider";
        ui_category = "Ray color";
        ui_label = "Distance Fading";
        ui_min = 0.1; ui_max = 10;
        ui_step = 0.01;
    > = 0.4;
    
// SMOTTHING
    
    uniform int iSmoothRadius <
        ui_type = "slider";
        ui_category = "Smoothing";
        ui_label = "Radius";
        ui_min = 0; ui_max = 16;
        ui_step = 1;
    > = 2;

    uniform float fSmoothDepthThreshold <
        ui_type = "slider";
        ui_category = "Smoothing";
        ui_label = "Depth Threshold";
        ui_min = 0.01; ui_max = 0.2;
        ui_step = 0.01;
    > = 0.01;

    uniform float fSmoothNormalThreshold <
        ui_type = "slider";
        ui_category = "Smoothing";
        ui_label = "Normal Threshold";
        ui_min = 0; ui_max = 2;
        ui_step = 0.01;
    > = 0.20;
 
// MERGING
    
    uniform float fSourceColor <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "Source color";
        ui_min = 0.1; ui_max = 2;
        ui_step = 0.01;
    > = 1.0;
    
    uniform float fLightMult <
        ui_type = "slider";
        ui_category = "Merging";
        ui_label = "Reflection";
        ui_min = 0.1; ui_max = 10;
        ui_step = 0.01;
    > = 1.20;

	
    float2 PixelSize() {
        return ReShade::PixelSize*RES_SCALE;
    }

    float getBrightness(float3 color) {
        return max(max(color.r,color.g),color.b);
    }

    int2 toPixels(float2 coords) {
        return BUFFER_SIZE*coords;
    }

    bool inScreen(float2 coords) {
        return coords.x>=0 && coords.x<=1
            && coords.y>=0 && coords.y<=1;
    }
    
    bool inScreen(float3 coords) {
        return coords.x>=0 && coords.x<=1
            && coords.y>=0 && coords.y<=1
            && coords.z>=0 && coords.z<=1;
    }

    bool isPixelTreated(float2 coords) {
        float2 noiseCoords = float2(toPixels(coords)%NOISE_SIZE)/NOISE_SIZE;
        float4 noise = getColorSampler(blueNoiseSampler,noiseCoords);
        float v = noise.x;
        int accuIndex = framecount % iFrameAccu;
        float accuWidth = 1.0/(iFrameAccu);
        float accuStart = accuIndex*accuWidth;

        return accuStart<=v && v<accuStart+accuWidth;
    }
    
    float getRoughness(float2 coords) {
    	return getColorSampler(roughnessSampler,coords).r;
    }
    
    float getDepth(float2 coords) {
    	float depth = ReShade::GetLinearizedDepth(coords);
    	if(bRoughness) {
    		float r = log(1.0+getRoughness(coords));
    		depth += fRoughnessReflexivity*r*(1.0-depth)/100.0;
    		
    	}
    	return depth;
    }

    float3 getWorldPosition(float2 coords) {
        float depth = getDepth(coords);
        float3 result = float3((coords-0.5)*depth,depth);
        result *= BUFFER_SIZE3;
        return result;
    }

    float3 getScreenPosition(float3 wp) {
        float3 result = wp/BUFFER_SIZE3;
        result.xy /= result.z;
        return float3(result.xy+0.5,result.z);
    }
    
    void PS_RoughnessPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outRoughness : SV_Target0) {
    	if(!bRoughness) discard;
        
        float3 refColor = getColor(coords).rgb;
        float refB = getBrightness(refColor);
        float roughness = 0.0;
        for(int d = -iRoughnessRadius;d<=iRoughnessRadius;d++) {
        	if(d!=0) {
	        	float3 color = getColor(float2(coords.x+ReShade::PixelSize.x*d,coords.y)).rgb;
	        	float b = getBrightness(color);
	        	float diff = abs(refB-b);
	        	roughness = max(roughness,5*diff/abs(d));
        	}
        }
        for(int d = -iRoughnessRadius;d<=iRoughnessRadius;d++) {
        	if(d!=0) {
	        	float3 color = getColor(float2(coords.x+ReShade::PixelSize.x*d,coords.y)).rgb;
	        	float b = getBrightness(color);
	        	float diff = abs(refB-b);
	        	roughness = max(roughness,5*diff/abs(d));
        	}
        }
        
        outRoughness = float4(roughness,roughness,roughness,1.0);
    }

    void PS_NormalPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outNormal : SV_Target0) {
        float3 offset = float3(PixelSize().xy, 0.0);

        float3 posCenter = getWorldPosition(coords);
        float3 posNorth  = getWorldPosition(coords - offset.zy);
        float3 posEast   = getWorldPosition(coords + offset.xz);
        float3 normal = normalize(cross(posCenter - posNorth, posCenter - posEast));
        
        outNormal = float4(normal/2.0+0.5,1.0/iFrameAccu);
    }
    
    float4 getRayColor(float2 coords) {
    	return fRayBounce>0 
			? fRayBounce*getColorSampler(resultSampler,coords)+(1-fRayBounce)*getColor(coords)
			: getColor(coords);
    }

    float4 trace(float3 refWp,float3 lightVector) {

		float stepRatio = 1.0+fRayStepMultiply/10.0;

        float stepLength = 1.0/fRayStepPrecision;
        float3 incrementVector = lightVector*stepLength;
        float traceDistance = 0;
        float3 currentWp = refWp;
        
        bool crossed = false;
        float deltaZ = 0;
        float deltaZbefore = 0;
        float3 lastCross;
		
		bool outSource = false;
		
		
        do {
        	currentWp += incrementVector;
            traceDistance += stepLength;
            
            float3 screenCoords = getScreenPosition(currentWp);
            
			bool outSearch = !bRayUntilHit && traceDistance>=iRayDistance;
			
			bool outScreen = !inScreen(screenCoords);
            
            float3 screenWp = getWorldPosition(screenCoords.xy);
            
            deltaZ = screenWp.z-currentWp.z;
            
            if(outSource) {
            	if(!outScreen && !outSearch && sign(deltaZ)!=sign(deltaZbefore)) {
            		// search more precise
            		float preciseRatio = 1.0/iRayPreciseHitSteps;
            		float3 preciseIncrementVector = incrementVector;
            		float preciseLength = stepLength;
            		for(int precisePass=0;precisePass<iRayPreciseHit;precisePass++) {
            			preciseIncrementVector *= preciseRatio;
						preciseLength *= preciseRatio;
						
						int preciseStep=0;
            			bool recrossed=false;
            			while(!recrossed && preciseStep<iRayPreciseHitSteps-1) {
            				currentWp -= preciseIncrementVector;
            				traceDistance -= preciseLength;
            				deltaZ = screenWp.z-currentWp.z;
            				recrossed = sign(deltaZ)==sign(deltaZbefore);
                            preciseStep++;
            			}
            			if(recrossed) {
            				currentWp += preciseIncrementVector;
            				traceDistance += preciseLength;
            				deltaZ = screenWp.z-currentWp.z;
            			}
            		}
            		
            		lastCross = currentWp;
            		crossed = true;
            		
            		
            	}
            	if(abs(deltaZ)<=fRayHitDepthThreshold || outScreen || outSearch) {
            		
            		if(bRayExtrapolate && outSearch) {
            			// try to extrapolate hit
            			float3  nextWp = currentWp + incrementVector;
            			float nextDeltaZ = screenWp.z-nextWp.z;
            			if(abs(nextDeltaZ)<abs(deltaZ)) {
            				// getting closer
            				float dPerStep = abs(deltaZ)-abs(nextDeltaZ);
            				float neededStep = abs(deltaZ)/dPerStep;
            				nextWp = currentWp + incrementVector*neededStep;
            				screenCoords = getScreenPosition(nextWp);
            				if(inScreen(screenCoords.xy)) {
            					lastCross = nextWp;
            					crossed = true;
            				}
            			}
            		}
            		// hit !
            		return float4(crossed ? lastCross : currentWp,outScreen ? fOutRatio : 1.0);
            	}
            } else {
            	if(outScreen) {
            		currentWp -= incrementVector;
            		screenCoords = getScreenPosition(currentWp);
            				
            		return float4(crossed ? lastCross : currentWp,fOutRatio);
				}
            	outSource = abs(deltaZ)>fRayHitDepthThreshold;
            }
            
            deltaZbefore = deltaZ;
            
            stepLength *= stepRatio;
            incrementVector *= stepRatio;

        } while(bRayUntilHit || traceDistance<iRayDistance);

        return 0.0;

    }
    
    void PS_ClearAccu(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outColor : SV_Target, out float4 outPos : SV_Target1)
    {
        if(framecount%iFrameAccu!=0) discard;
        outColor = float4(0,0,0,1);
        outPos = float4(0,0,0,0);
    }

    void PS_LightPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outColor : SV_Target0, out float4 outHit : SV_Target1) {

		float depth = getDepth(coords);
		if(depth>fSkyDepth) {
			outColor = float4(1,1,1,1.0);
			outHit = float4(0.5,0.5,0.5,0.0);
		} else {
		
	        float3 targetWp = getWorldPosition(coords);
	        float3 targetNormal = getNormal(coords);
	
	        float3 lightVector = reflect(targetWp,targetNormal);
	        
	        float3 jitter = tex2Dfetch(blueNoiseSampler,(framecount+coords*BUFFER_SIZE)%NOISE_SIZE).rgb-0.5;
	        lightVector += jitter*fJitter;
	        lightVector = normalize(lightVector);
	        
	       // float4 hitColor = trace(targetWp,lightVector);
	        float4 hitPosition = trace(targetWp,lightVector);
	        if(hitPosition.a==0) {
	        	// no hit
	        } else {
	        	float3 screenCoords = getScreenPosition(hitPosition.xyz);
	        	float4 color = getRayColor(screenCoords.xy);
	        	float b = getBrightness(color.rgb);
	                        
	        	float distance = 1+0.02*distance(hitPosition.xyz,targetWp);
	        	float distanceRatio = 0.1+1.0/pow(distance,fFadePower);//(1-pow(distance,fFadePower)/pow(iRayDistance,fFadePower));
		        
		        //outColor = float4(hitPosition.a*(0.5+b)*distanceRatio*color.rgb,1.0);
				outColor = float4(hitPosition.a*distanceRatio*color.rgb,1.0);
		  	  outHit = float4(screenCoords,1.0);			
	        }
        }
    }
    
    void PS_UpdateAccu(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outPixel : SV_Target)
    {
        outPixel = float4(getColorSampler(lightPassSampler,coords).rgb,1.0);///iFrameAccu);
    }

    void PS_SmoothPass(float4 vpos : SV_Position, float2 coords : TexCoord, out float4 outColor : SV_Target0) {
        
        int2 coordsInt = toPixels(coords);
        float refDepth = getDepth(coords);

        //if(refDepth>fSkyDepth) {
		//	outColor = float4(1,1,1,1.0);
		//} else {
        
	        float3 refNormal = getNormal(coords);
	        
	        float4 result = 0;
	        float weightSum = 0;
	
			float4 resultMiss = 0;
	        float weightSumMiss = 0;
		
			float4 resultWhite = 0;
	        float weightSumWhite = 0;
	        
	        int2 offset = 0;
	        int radius = RES_SCALE*iSmoothRadius;
	        
	        float maxRadius2 = 1+radius*radius;
	        
	        [loop]
	        for(offset.x=-radius;offset.x<=radius;offset.x++) {
	        	[loop]
	            for(offset.y=-radius;offset.y<=radius;offset.y++) {
	                float2 currentCoords = coords+offset*PixelSize();
	                
	                if(inScreen(currentCoords)) {
	                
	                     
	                    
						float depth = getDepth(currentCoords);
						if(depth>fSkyDepth) continue;
	                    if(abs(depth-refDepth)<=fSmoothDepthThreshold) {
	                        float3 normal = getNormal(currentCoords);
	                        if(diffT(normal,refNormal,fSmoothNormalThreshold)) {
	                        	
								float4 hitPos = getColorSampler(lightPassHitSampler,currentCoords);
	                    		float4 color = getColorSampler(lightAccuSampler,currentCoords);
	                   
	                        	
	                        	float d2 = 1+dot(offset,offset);
	                            float weight = 2+1.0/d2;
	                            
								// hit
	                        	result += color*weight;
	                            weightSum += weight;
	                        } // end normal
	                    } // end depth                 
	                } // end inScreen
	            } // end for y
	        } // end for x
	
	        if(weightSum>0) {
				result.rgb = saturate(fLightMult*result.rgb/weightSum);
			}
			        
	        if(bOrientation) {
	        	
		        float3 normal = getNormal(coords);
		        //float cos = dot(normal,float3(0,0,1));
		        float cos = dot(normal,float3(coords-0.5,1));
		        //result.rgb *= saturate(fOrientationBias+1-cos);
		        float3 corrected = saturate((-cos+1.0)*result.rgb/2.0);
		        result.rgb = fOrientationBias*corrected+(1.0-fOrientationBias)*result.rgb;
	        }
	        
	        result.a = 1.0/iFrameAccu;
	        outColor = result;
        //}
    }
    
    void PS_UpdateResult(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outPixel : SV_Target)
    {

    	float depth = getDepth(coords);
        float3 color = getColor(coords).rgb;
        float depthRatio = 1.0;
        float rRatio = 1.0;
		float r = 0.0;
		if(bRoughness) {
			r = getRoughness(coords);
			rRatio = fRoughnessReflexivity;
		} 
        
        
        if(depth>fSkyDepth) {
        	depthRatio = 0.0;
        } else if(depth>fSkyFading) {
        	float total = fSkyDepth-fSkyFading;
        	depthRatio = 1.0-(depth-fSkyFading)/total;
        }

	        
        float3 light = getColorSampler(smoothPassSampler,coords).rgb;
        float b = getBrightness(color);
        float lb = getBrightness(light);
        
        //float4 result = saturate((fSourceOffset+color)*(fSourceColor+pow(light+fLightOffset,fLightPow)));
        //result = color*b+result*(1.0-b);
        //float4 result = fSourceColor*color+fLightMult*light*saturate(1.0-b)*(max(0,1.0-b-fSourceOffset));
        float3 result = (1-r)*fSourceColor*color+fLightMult*(b<=0.5 ? b : 1.0-b)*light;
        outPixel = float4(min(1.-b,depthRatio)*result+max(b,1.0-depthRatio)*color,1.0);
    }
    
    void PS_DisplayResult(in float4 position : SV_Position, in float2 coords : TEXCOORD, out float4 outPixel : SV_Target)
    {
        float4 color = bDebug 
			? getColorSampler(smoothPassSampler,coords)
			: getColorSampler(resultSampler,coords);
        outPixel = color;
    }
    
    
    technique DH_SSR {
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_RoughnessPass;
            RenderTarget = roughnessTex;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_NormalPass;
            RenderTarget = normalTex;
            
            ClearRenderTargets = false;
						
			BlendEnable = true;
			BlendOp = ADD;

			// The data source and optional pre-blend operation used for blending.
			// Available values:
			//   ZERO, ONE,
			//   SRCCOLOR, SRCALPHA, INVSRCCOLOR, INVSRCALPHA
			//   DESTCOLOR, DESTALPHA, INVDESTCOLOR, INVDESTALPHA
			SrcBlend = SRCALPHA;
			SrcBlendAlpha = ONE;
			DestBlend = INVSRCALPHA;
			DestBlendAlpha = ONE;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_LightPass;
            RenderTarget = lightPassTex;
            RenderTarget1 = lightPassHitTex;
            
            ClearRenderTargets = false;
						
			BlendEnable = true;
			BlendOp = ADD;

			// The data source and optional pre-blend operation used for blending.
			// Available values:
			//   ZERO, ONE,
			//   SRCCOLOR, SRCALPHA, INVSRCCOLOR, INVSRCALPHA
			//   DESTCOLOR, DESTALPHA, INVDESTCOLOR, INVDESTALPHA
			SrcBlend = SRCALPHA;
			SrcBlendAlpha = ONE;
			DestBlend = INVSRCALPHA;
			DestBlendAlpha = ONE;
        }
        pass
        {
            VertexShader = PostProcessVS;
            PixelShader = PS_UpdateAccu;
            RenderTarget = lightAccuTex;
            
            ClearRenderTargets = false;
            
            BlendEnable = true;
			BlendOp = ADD;

			// The data source and optional pre-blend operation used for blending.
			// Available values:
			//   ZERO, ONE,
			//   SRCCOLOR, SRCALPHA, INVSRCCOLOR, INVSRCALPHA
			//   DESTCOLOR, DESTALPHA, INVDESTCOLOR, INVDESTALPHA
			SrcBlend = SRCALPHA;
			SrcBlendAlpha = ONE;
			DestBlend = INVSRCALPHA;
			DestBlendAlpha = ONE;
        }
		pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_SmoothPass;
            RenderTarget = smoothPassTex;

            ClearRenderTargets = false;
            
            BlendEnable = true;
			BlendOp = ADD;

			// The data source and optional pre-blend operation used for blending.
			// Available values:
			//   ZERO, ONE,
			//   SRCCOLOR, SRCALPHA, INVSRCCOLOR, INVSRCALPHA
			//   DESTCOLOR, DESTALPHA, INVDESTCOLOR, INVDESTALPHA
			SrcBlend = SRCALPHA;
			SrcBlendAlpha = ONE;
			DestBlend = INVSRCALPHA;
			DestBlendAlpha = ONE;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_UpdateResult;
            RenderTarget = resultTex;

            ClearRenderTargets = false;
        }
        pass {
            VertexShader = PostProcessVS;
            PixelShader = PS_DisplayResult;
        }
    }


}