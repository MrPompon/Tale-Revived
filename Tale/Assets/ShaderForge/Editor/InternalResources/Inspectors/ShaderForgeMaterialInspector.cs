

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ShaderForge;

public class ShaderForgeMaterialInspector : MaterialEditor {
	
	// this is the same as the ShaderProperty function, show here so 
	// you can see how it works
    /*
    private void shaderpropertyimpl(shader shader, int propertyindex)
    {
        int i = propertyindex;
        string label = shaderutil.getpropertydescription(shader, i);
        string propertyname = shaderutil.getpropertyname(shader, i);




        switch (shaderutil.getpropertytype(shader, i))
        {
            case shaderutil.shaderpropertytype.range: // float ranges
            {
                guilayout.beginhorizontal();
                float v2 = shaderutil.getrangelimits(shader, i, 1);
                float v3 = shaderutil.getrangelimits(shader, i, 2);



                rangeproperty(propertyname, label, v2, v3);
                guilayout.endhorizontal();
				
                break;
            }
            case shaderutil.shaderpropertytype.float: // floats
            {
                floatproperty(propertyname, label);
                break;
            }
            case shaderutil.shaderpropertytype.color: // colors
            {
                colorproperty(propertyname, label);
                break;
            }
            case shaderutil.shaderpropertytype.texenv: // textures
            {
                shaderutil.shaderpropertytexdim desiredtexdim = shaderutil.gettexdim(shader, i);
                textureproperty(propertyname, label, desiredtexdim);
				
                guilayout.space(6);
                break;
            }
            case shaderutil.shaderpropertytype.vector: // vectors
            {
                vectorproperty(propertyname, label);
                break;
            }
            default:
            {
                guilayout.label("unknown property " + label + " : " + shaderutil.getpropertytype(shader, i));
                break;
            }
        }
    }*



    public override void OnInspectorGUI()
	{
		base.serializedObject.Update();
		var theShader = serializedObject.FindProperty ("m_Shader");


		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null )
		{

			Shader shader = theShader.objectReferenceValue as Shader;


			// SHADER FORGE BUTTONS
			if( GUILayout.Button( "Open shader in Shader Forge" ) ) {
				SF_Editor.Init( shader );
			}
			if( SF_Tools.advancedInspector ) {
				GUILayout.BeginHorizontal();
				{
					GUIStyle btnStyle = "MiniButton";
					if( GUILayout.Button( "Open shader code", btnStyle ) ) {
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal( AssetDatabase.GetAssetPath( shader ), 1 );
					}
					//if( GUILayout.Button( "Open compiled shader", btnStyle ) ) {
					//	ShaderForgeInspector.OpenCompiledShader( shader );
					//}
				}
				GUILayout.EndHorizontal();
			}

			Material mat = target as Material;
			

			mat.globalIlluminationFlags = (MaterialGlobalIlluminationFlags)EditorGUILayout.EnumPopup( "Emission GI", mat.globalIlluminationFlags);
			
			GUILayout.Space(6);




			if(this.PropertiesGUI())
				this.PropertiesChanged();
		}
	}


	/*
	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		var theShader = serializedObject.FindProperty ("m_Shader");	
		if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
		{
			float controlSize = 80;
			
			EditorGUIUtility.LookLikeControls(Screen.width - controlSize - 20);
			
			EditorGUI.BeginChangeCheck();
			Shader shader = theShader.objectReferenceValue as Shader;


			// SHADER FORGE BUTTONS
			if( GUILayout.Button( "Open shader in Shader Forge" ) ) {
				SF_Editor.Init( shader );
			}
			if( SF_Tools.advancedInspector ) {
				GUILayout.BeginHorizontal();
				{
					GUIStyle btnStyle = "MiniButton";
					if( GUILayout.Button( "Open source", btnStyle ) ) {
						UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal( AssetDatabase.GetAssetPath( shader ), 1 );
					}
					if( GUILayout.Button( "Open compiled", btnStyle ) ) {
						ShaderForgeInspector.OpenCompiledShader( shader );
					}
				}
				GUILayout.EndHorizontal();
			}

			GUILayout.Space(6);


			//GUILayout.Box("Material Properties:",EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
			//GUI.color = Color.white;
			// END SF BUTTONS


			//GUI.color = SF_Node.colorExposed;
			for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
			{
				ShaderPropertyImpl(shader, i);
			}
			//GUI.color = Color.white;
			
			if (EditorGUI.EndChangeCheck())
				PropertiesChanged ();
		}
	}
	*/
}
