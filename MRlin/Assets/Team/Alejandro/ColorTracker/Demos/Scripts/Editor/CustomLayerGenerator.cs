using UnityEditor;

/// <summary>
/// Creates some extra layer needed for demos
/// </summary>
[InitializeOnLoad]
public class CustomLayerGenerator
{
    static CustomLayerGenerator()
    {
        CreateLayer();
    }

    static void CreateLayer()
    {
        SerializedObject SerializedObjecttagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty it = SerializedObjecttagManager.GetIterator();
        bool showChildren = true;
        while (it.NextVisible(showChildren))
        {
            if (it.name == "layers")
            {
                it.GetArrayElementAtIndex(25).stringValue = "Fruits";
                it.GetArrayElementAtIndex(26).stringValue = "FruitsSlice";
            }
        }
        SerializedObjecttagManager.ApplyModifiedProperties();
    }
}

