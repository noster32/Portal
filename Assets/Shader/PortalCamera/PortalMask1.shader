Shader "Custom/PortalMask1"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _MaskID("Mask ID", Float) = 1
        _RenderTime("Render Time", Float) = 1999
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        Pass
        {
            Stencil
            {
                Ref 1           
                Comp Equal      
                Pass Replace
            }   

        }
    }
}
