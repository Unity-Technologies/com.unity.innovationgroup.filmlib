//
// This file was automatically generated. Please don't edit by hand.
//

#ifndef FUR_CS_HLSL
#define FUR_CS_HLSL
//
// UnityEngine.Experimental.Rendering.HDPipeline.Fur+MaterialFeatureFlags:  static fields
//
#define MATERIALFEATUREFLAGS_HAIR_KAJIYA_KAY (1)

//
// UnityEngine.Experimental.Rendering.HDPipeline.Fur+SurfaceData:  static fields
//
#define DEBUGVIEW_FUR_SURFACEDATA_MATERIAL_FEATURES (1500)
#define DEBUGVIEW_FUR_SURFACEDATA_AMBIENT_OCCLUSION (1501)
#define DEBUGVIEW_FUR_SURFACEDATA_DIFFUSE (1502)
#define DEBUGVIEW_FUR_SURFACEDATA_SPECULAR_OCCLUSION (1503)
#define DEBUGVIEW_FUR_SURFACEDATA_NORMAL (1504)
#define DEBUGVIEW_FUR_SURFACEDATA_NORMAL_VIEW_SPACE (1505)
#define DEBUGVIEW_FUR_SURFACEDATA_GEOMETRIC_NORMAL (1506)
#define DEBUGVIEW_FUR_SURFACEDATA_GEOMETRIC_NORMAL_VIEW_SPACE (1507)
#define DEBUGVIEW_FUR_SURFACEDATA_SMOOTHNESS (1508)
#define DEBUGVIEW_FUR_SURFACEDATA_TRANSMITTANCE (1509)
#define DEBUGVIEW_FUR_SURFACEDATA_RIM_TRANSMISSION_INTENSITY (1510)
#define DEBUGVIEW_FUR_SURFACEDATA_HAIR_STRAND_DIRECTION (1511)
#define DEBUGVIEW_FUR_SURFACEDATA_SECONDARY_SMOOTHNESS (1512)
#define DEBUGVIEW_FUR_SURFACEDATA_SPECULAR_TINT (1513)
#define DEBUGVIEW_FUR_SURFACEDATA_SECONDARY_SPECULAR_TINT (1514)
#define DEBUGVIEW_FUR_SURFACEDATA_SPECULAR_SHIFT (1515)
#define DEBUGVIEW_FUR_SURFACEDATA_SECONDARY_SPECULAR_SHIFT (1516)

//
// UnityEngine.Experimental.Rendering.HDPipeline.Fur+BSDFData:  static fields
//
#define DEBUGVIEW_FUR_BSDFDATA_MATERIAL_FEATURES (1550)
#define DEBUGVIEW_FUR_BSDFDATA_AMBIENT_OCCLUSION (1551)
#define DEBUGVIEW_FUR_BSDFDATA_SPECULAR_OCCLUSION (1552)
#define DEBUGVIEW_FUR_BSDFDATA_DIFFUSE_COLOR (1553)
#define DEBUGVIEW_FUR_BSDFDATA_FRESNEL0 (1554)
#define DEBUGVIEW_FUR_BSDFDATA_SPECULAR_TINT (1555)
#define DEBUGVIEW_FUR_BSDFDATA_NORMAL_WS (1556)
#define DEBUGVIEW_FUR_BSDFDATA_NORMAL_VIEW_SPACE (1557)
#define DEBUGVIEW_FUR_BSDFDATA_GEOMETRIC_NORMAL (1558)
#define DEBUGVIEW_FUR_BSDFDATA_GEOMETRIC_NORMAL_VIEW_SPACE (1559)
#define DEBUGVIEW_FUR_BSDFDATA_PERCEPTUAL_ROUGHNESS (1560)
#define DEBUGVIEW_FUR_BSDFDATA_TRANSMITTANCE (1561)
#define DEBUGVIEW_FUR_BSDFDATA_RIM_TRANSMISSION_INTENSITY (1562)
#define DEBUGVIEW_FUR_BSDFDATA_HAIR_STRAND_DIRECTION_WS (1563)
#define DEBUGVIEW_FUR_BSDFDATA_ROUGHNESS_T (1564)
#define DEBUGVIEW_FUR_BSDFDATA_ROUGHNESS_B (1565)
#define DEBUGVIEW_FUR_BSDFDATA_ANISOTROPY (1566)
#define DEBUGVIEW_FUR_BSDFDATA_SECONDARY_PERCEPTUAL_ROUGHNESS (1567)
#define DEBUGVIEW_FUR_BSDFDATA_SECONDARY_SPECULAR_TINT (1568)
#define DEBUGVIEW_FUR_BSDFDATA_SPECULAR_EXPONENT (1569)
#define DEBUGVIEW_FUR_BSDFDATA_SECONDARY_SPECULAR_EXPONENT (1570)
#define DEBUGVIEW_FUR_BSDFDATA_SPECULAR_SHIFT (1571)
#define DEBUGVIEW_FUR_BSDFDATA_SECONDARY_SPECULAR_SHIFT (1572)

// Generated from UnityEngine.Experimental.Rendering.HDPipeline.Fur+SurfaceData
// PackingRules = Exact
struct SurfaceData
{
    uint materialFeatures;
    float ambientOcclusion;
    float3 diffuseColor;
    float specularOcclusion;
    float3 normalWS;
    float3 geomNormalWS;
    float perceptualSmoothness;
    float3 transmittance;
    float rimTransmissionIntensity;
    float3 hairStrandDirectionWS;
    float secondaryPerceptualSmoothness;
    float3 specularTint;
    float3 secondarySpecularTint;
    float specularShift;
    float secondarySpecularShift;
};

// Generated from UnityEngine.Experimental.Rendering.HDPipeline.Fur+BSDFData
// PackingRules = Exact
struct BSDFData
{
    uint materialFeatures;
    float ambientOcclusion;
    float specularOcclusion;
    float3 diffuseColor;
    float3 fresnel0;
    float3 specularTint;
    float3 normalWS;
    float3 geomNormalWS;
    float perceptualRoughness;
    float3 transmittance;
    float rimTransmissionIntensity;
    float3 hairStrandDirectionWS;
    float roughnessT;
    float roughnessB;
    float anisotropy;
    float secondaryPerceptualRoughness;
    float3 secondarySpecularTint;
    float specularExponent;
    float secondarySpecularExponent;
    float specularShift;
    float secondarySpecularShift;
};

//
// Debug functions
//
void GetGeneratedSurfaceDataDebug(uint paramId, SurfaceData surfacedata, inout float3 result, inout bool needLinearToSRGB)
{
    switch (paramId)
    {
        case DEBUGVIEW_FUR_SURFACEDATA_MATERIAL_FEATURES:
            result = GetIndexColor(surfacedata.materialFeatures);
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_AMBIENT_OCCLUSION:
            result = surfacedata.ambientOcclusion.xxx;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_DIFFUSE:
            result = surfacedata.diffuseColor;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SPECULAR_OCCLUSION:
            result = surfacedata.specularOcclusion.xxx;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_NORMAL:
            result = surfacedata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_NORMAL_VIEW_SPACE:
            result = surfacedata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_GEOMETRIC_NORMAL:
            result = surfacedata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_GEOMETRIC_NORMAL_VIEW_SPACE:
            result = surfacedata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SMOOTHNESS:
            result = surfacedata.perceptualSmoothness.xxx;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_TRANSMITTANCE:
            result = surfacedata.transmittance;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_RIM_TRANSMISSION_INTENSITY:
            result = surfacedata.rimTransmissionIntensity.xxx;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_HAIR_STRAND_DIRECTION:
            result = surfacedata.hairStrandDirectionWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SECONDARY_SMOOTHNESS:
            result = surfacedata.secondaryPerceptualSmoothness.xxx;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SPECULAR_TINT:
            result = surfacedata.specularTint;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SECONDARY_SPECULAR_TINT:
            result = surfacedata.secondarySpecularTint;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SPECULAR_SHIFT:
            result = surfacedata.specularShift.xxx;
            break;
        case DEBUGVIEW_FUR_SURFACEDATA_SECONDARY_SPECULAR_SHIFT:
            result = surfacedata.secondarySpecularShift.xxx;
            break;
    }
}

//
// Debug functions
//
void GetGeneratedBSDFDataDebug(uint paramId, BSDFData bsdfdata, inout float3 result, inout bool needLinearToSRGB)
{
    switch (paramId)
    {
        case DEBUGVIEW_FUR_BSDFDATA_MATERIAL_FEATURES:
            result = GetIndexColor(bsdfdata.materialFeatures);
            break;
        case DEBUGVIEW_FUR_BSDFDATA_AMBIENT_OCCLUSION:
            result = bsdfdata.ambientOcclusion.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SPECULAR_OCCLUSION:
            result = bsdfdata.specularOcclusion.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_DIFFUSE_COLOR:
            result = bsdfdata.diffuseColor;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_FRESNEL0:
            result = bsdfdata.fresnel0;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SPECULAR_TINT:
            result = bsdfdata.specularTint;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_NORMAL_WS:
            result = bsdfdata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_NORMAL_VIEW_SPACE:
            result = bsdfdata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_GEOMETRIC_NORMAL:
            result = bsdfdata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_GEOMETRIC_NORMAL_VIEW_SPACE:
            result = bsdfdata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_PERCEPTUAL_ROUGHNESS:
            result = bsdfdata.perceptualRoughness.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_TRANSMITTANCE:
            result = bsdfdata.transmittance;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_RIM_TRANSMISSION_INTENSITY:
            result = bsdfdata.rimTransmissionIntensity.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_HAIR_STRAND_DIRECTION_WS:
            result = bsdfdata.hairStrandDirectionWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_ROUGHNESS_T:
            result = bsdfdata.roughnessT.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_ROUGHNESS_B:
            result = bsdfdata.roughnessB.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_ANISOTROPY:
            result = bsdfdata.anisotropy.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SECONDARY_PERCEPTUAL_ROUGHNESS:
            result = bsdfdata.secondaryPerceptualRoughness.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SECONDARY_SPECULAR_TINT:
            result = bsdfdata.secondarySpecularTint;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SPECULAR_EXPONENT:
            result = bsdfdata.specularExponent.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SECONDARY_SPECULAR_EXPONENT:
            result = bsdfdata.secondarySpecularExponent.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SPECULAR_SHIFT:
            result = bsdfdata.specularShift.xxx;
            break;
        case DEBUGVIEW_FUR_BSDFDATA_SECONDARY_SPECULAR_SHIFT:
            result = bsdfdata.secondarySpecularShift.xxx;
            break;
    }
}


#endif
