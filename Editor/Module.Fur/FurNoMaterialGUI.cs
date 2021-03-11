using System;
using UnityEditor;

public class FurNoMaterialGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        // We control the material entirely on the property block.
    }
}