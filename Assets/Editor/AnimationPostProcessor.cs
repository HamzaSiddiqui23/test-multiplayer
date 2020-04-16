using UnityEditor;
using UnityEngine;

public class AnimationPostProcessor : AssetPostprocessor
{
    private ModelImporter importer;

    private void OnPreprocessModel()
    {
        importer = assetImporter as ModelImporter;
    }

    private void OnPostprocessModel(GameObject gameObject)
    {
        ModelImporterClipAnimation[] animations = importer.defaultClipAnimations;
        importer.clipAnimations = animations;
    }
}
