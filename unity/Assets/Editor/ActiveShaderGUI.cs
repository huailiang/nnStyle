using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityEditor
{

    public class ActiveShaderGUI : ShaderGUI
    {

        const string KEY_ACTIVE_RELU = "ACTIVE_SHADER_RELU";
        const string KEY_ACTIVE_LRELU = "ACTIVE_SHADER_LRELU";
        const string KEY_ACTIVE_TANH = "ACTIVE_SHADER_TANH";
        const string KEY_ACTIVE_SIGMOD = "ACTIVE_SHADER_SIGMOD";

        internal enum ActiveMode
        {
            RELU,
            LRELU,
            TANH,
            SIGMOD
        }

        Material material;

        ActiveMode mode;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            material = materialEditor.target as Material;

            EditorGUI.BeginChangeCheck();
            mode = (ActiveMode)EditorGUILayout.EnumPopup("active function", mode);
            if (EditorGUI.EndChangeCheck())
            {
                EnableMatKeyword(KEY_ACTIVE_RELU, mode == ActiveMode.RELU);
                EnableMatKeyword(KEY_ACTIVE_LRELU, mode == ActiveMode.LRELU);
                EnableMatKeyword(KEY_ACTIVE_TANH, mode == ActiveMode.TANH);
                EnableMatKeyword(KEY_ACTIVE_SIGMOD, mode == ActiveMode.SIGMOD);
            }
            base.OnGUI(materialEditor, props);
        }


        private void EnableMatKeyword(string key, bool enable)
        {
            if (enable)
            {
                material.EnableKeyword(key);
            }
            else
            {
                material.DisableKeyword(key);
            }
        }

    }
}