using UnityEngine;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class WindowLayouts
    {
        public static readonly GUILayoutOption[] TextInputLayout =
        {
            //GUILayout.MaxWidth(400)
            GUILayout.ExpandWidth(true)
        };

        public static readonly GUILayoutOption[] ClassSetupButtonLayout =
        {
            GUILayout.MinWidth(60),
            GUILayout.ExpandWidth(false)
        };
    }
}