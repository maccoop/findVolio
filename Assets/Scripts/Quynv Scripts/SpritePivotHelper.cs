using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SpritePivotHelper : MonoBehaviour
{
#if UNITY_EDITOR
    //[Range(0,1)] [SerializeField] private float x;
    //[Range(0,1)] [SerializeField] private float y;
    private SpriteRenderer sprite;
    private TextureImporter tex;
    private CircleCollider2D col;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        try
        {
            string path = AssetDatabase.GetAssetPath(sprite.sprite.texture);
            tex = (TextureImporter)AssetImporter.GetAtPath(path);
            col = GetComponent<CircleCollider2D>();
        }
        catch (Exception e)
        {
            //Debug.Log(e);
        }
    }

    [Button("Update position")]
    public void ChangePivot()
    {
        //Vector2 pivot = new Vector2(x, y);
        //TextureImporterSettings texSettings = new TextureImporterSettings();
        //tex.ReadTextureSettings(texSettings);
        //texSettings.spriteAlignment = (int)SpriteAlignment.Custom;
        //tex.SetTextureSettings(texSettings);
        //tex.SaveAndReimport();
        Vector2 delta = tex.spritePivot - new Vector2(0.5f, 0.5f);
        Vector2 deltaWorld = delta * sprite.bounds.size;
        transform.position += new Vector3(deltaWorld.x, deltaWorld.y, 0);
        col.offset -= deltaWorld;
    }
#endif
}
