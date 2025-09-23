Shader "Hidden/ColorBlindnessCorrection"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _ColorBlindnessType ("Colorblindness Type", Int) = 0
        _ContrastBoost ("Contrast Boost", Range(0.5, 2.0)) = 1.0
        _SaturationAdjustment ("Saturation Adjustment", Range(0.8, 1.5)) = 1.0
        _BrightnessAdjustment ("Brightness Adjustment", Range(0.5, 1.5)) = 1.0
        _EffectStrength ("Effect Strength", Range(0.0, 1.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
        ZTest Always ZWrite Off Cull Off
        
        Pass
        {
            Name "ColorBlindnessCorrection"
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            int _ColorBlindnessType;
            float _ContrastBoost;
            float _SaturationAdjustment;
            float _BrightnessAdjustment;
            float _EffectStrength;
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }
            
            // Colorblindness correction matrices
            // These matrices help distinguish colors for different types of colorblindness
            
            float3x3 GetDeuteranopiaCorrectionMatrix()
            {
                // Enhanced green channel distinction for deuteranopia (green-blind)
                // This matrix redistributes colors to make green-red distinction clearer
                return float3x3(
                    0.80, 0.20, 0.0,   // Red enhanced
                    0.25, 0.75, 0.0,   // Green redistribution
                    0.0, 0.14, 0.86    // Blue preserved
                );
            }
            
            float3x3 GetProtanopiaCorrectionMatrix()
            {
                // Enhanced red channel distinction for protanopia (red-blind)
                // This matrix helps distinguish red from green
                return float3x3(
                    0.56, 0.43, 0.01,  // Red redistribution
                    0.55, 0.44, 0.01,  // Green adjustment
                    0.0, 0.24, 0.76    // Blue preserved
                );
            }
            
            float3x3 GetTritanopiaCorrectionMatrix()
            {
                // Enhanced blue channel distinction for tritanopia (blue-blind)
                // This matrix helps distinguish blue from yellow
                return float3x3(
                    0.97, 0.02, 0.01,  // Red preserved
                    0.11, 0.87, 0.02,  // Green adjustment
                    0.07, 0.33, 0.60   // Blue redistribution
                );
            }
            
            float3 AdjustContrast(float3 color, float contrast)
            {
                // Enhance contrast around middle gray (0.5)
                return saturate((color - 0.5) * contrast + 0.5);
            }
            
            float3 AdjustSaturation(float3 color, float saturation)
            {
                // Calculate luminance using standard weights
                float luminance = dot(color, float3(0.299, 0.587, 0.114));
                return lerp(float3(luminance, luminance, luminance), color, saturation);
            }
            
            float3 AdjustBrightness(float3 color, float brightness)
            {
                return saturate(color * brightness);
            }
            
            // Enhanced color separation for better accessibility
            float3 EnhanceColorSeparation(float3 color, int colorBlindnessType)
            {
                if (colorBlindnessType == 1) // Deuteranopia
                {
                    // Enhance red-green separation
                    float redGreenDiff = abs(color.r - color.g);
                    if (redGreenDiff < 0.1)
                    {
                        // Boost the stronger color
                        if (color.r > color.g)
                            color.r = min(1.0, color.r + 0.2);
                        else
                            color.g = min(1.0, color.g + 0.2);
                    }
                }
                else if (colorBlindnessType == 2) // Protanopia
                {
                    // Similar enhancement for protanopia
                    float redGreenDiff = abs(color.r - color.g);
                    if (redGreenDiff < 0.15)
                    {
                        color.r = min(1.0, color.r + 0.15);
                        color.g = max(0.0, color.g - 0.1);
                    }
                }
                else if (colorBlindnessType == 3) // Tritanopia
                {
                    // Enhance blue-yellow separation
                    float blueYellow = color.b - (color.r + color.g) * 0.5;
                    if (abs(blueYellow) < 0.1)
                    {
                        if (blueYellow > 0)
                            color.b = min(1.0, color.b + 0.2);
                        else
                            color.rg = min(1.0, color.rg + 0.1);
                    }
                }
                
                return color;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                half4 originalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half3 color = originalColor.rgb;
                
                // Apply colorblindness correction based on type
                if (_ColorBlindnessType == 1) // Deuteranopia
                {
                    color = mul(GetDeuteranopiaCorrectionMatrix(), color);
                }
                else if (_ColorBlindnessType == 2) // Protanopia
                {
                    color = mul(GetProtanopiaCorrectionMatrix(), color);
                }
                else if (_ColorBlindnessType == 3) // Tritanopia
                {
                    color = mul(GetTritanopiaCorrectionMatrix(), color);
                }
                
                // Apply additional enhancements
                if (_ColorBlindnessType > 0)
                {
                    color = EnhanceColorSeparation(color, _ColorBlindnessType);
                }
                
                // Apply visual adjustments
                color = AdjustBrightness(color, _BrightnessAdjustment);
                color = AdjustContrast(color, _ContrastBoost);
                color = AdjustSaturation(color, _SaturationAdjustment);
                
                // Blend with original based on effect strength
                color = lerp(originalColor.rgb, color, _EffectStrength);
                
                return half4(color, originalColor.a);
            }
            ENDHLSL
        }
    }
}