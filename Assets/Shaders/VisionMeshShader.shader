Shader "Custom/VisionMeshShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Stencil
            {
                Ref 1
                Comp always
                Pass replace
            }

            ColorMask 0
        }
    }
}
