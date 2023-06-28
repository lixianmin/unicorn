
/********************************************************************
created:    2022-11-02
author:     lixianmin

*********************************************************************/

using UnityEditor;

namespace Unicorn.UI
{
    public class UISerializerMenus
    {
        // [MenuItem("Assets/*Add UISerializer", true)]
        // private static bool _SerializePrefab_Validate()
        // {
        //     var prefab = Selection.activeGameObject;
        //     if (prefab is null)
        //     {
        //         return false;
        //     }
        //     
        //     var script = prefab.GetComponent<UISerializer>();
        //     if (script is null)
        //     {
        //         prefab.AddComponent<UISerializer>();
        //     }
        //     
        //     return true;
        // }
        
        [MenuItem("Assets/*Add UISerializer", false, 2000)]
        private static void _SerializePrefab()
        {
            var prefab = Selection.activeGameObject;
            var script = prefab.SetDefaultComponent<UISerializer>();
            UISerializerEditor.SerializePrefab(script);
        }
    }
}