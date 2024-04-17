using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageMaker : MMSingleton<StageMaker>
{
    [SerializeField] private UnityEngine.Object _folderTexture;
    [SerializeField] private GameObject _mapObj;
    [SerializeField] private HiddenItem _hiddenItemPrefab;

    public string StageId => _mapObj.name;

#if UNITY_EDITOR
    public void GeneratedStage()
    {
        string folderPath = AssetDatabase.GetAssetPath(_folderTexture);
        GameObject hiddenObj = new GameObject($"{_mapObj.name}");
        List<GameObject> willDestroy = new List<GameObject>();
        Dictionary<GameObject, GameObject> willAddToMap = new Dictionary<GameObject, GameObject>();

        string jsonTraceTex = AssetDatabase.LoadAssetAtPath<TextAsset>($"{folderPath}/{_folderTexture.name}.txt").text;
        Dictionary<string, string> traceTex = JsonMapper.ToObject<Dictionary<string, string>>(jsonTraceTex);

        for (int i = 0; i < _mapObj.transform.childCount; i++)
        {
            var obj = _mapObj.transform.GetChild(i);
            SpriteRenderer[] sprites = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spr in sprites)
            {
                //string key = GetSpriteName(spr.gameObject.name);
                try
                {
                    string key = traceTex[spr.sprite.name];
                    obj.name = key;
                    Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>($"{folderPath}/{key}.png");
                    if (sp == null)
                        Debug.Log($"non sprite : {key}");
                    spr.sprite = sp;
                }
                catch
                {
                    Debug.Log($"key not found : {spr.name} , {spr.GetInstanceID()}");
                }
            }

            if (obj.name.Contains(Const.PREFIX_HIDDEN_OBJECT))
            {
                var hidden = PrefabUtility.InstantiatePrefab(_hiddenItemPrefab.gameObject) as GameObject;
                hidden.transform.parent = hiddenObj.transform;
                hidden.name = obj.name;
                hidden.GetComponent<HiddenItem>().Id = hidden.GetInstanceID();

                if (obj.childCount == 0)
                {
                    var origin = obj.GetComponent<SpriteRenderer>();
                    SpriteRenderer spr;
                    CircleCollider2D col;

                    if (obj.name.Contains(Const.PREFIX_ANIM_OBJECT))
                    {
                        var go = new GameObject($"{obj.name}");
                        spr = go.AddComponent<SpriteRenderer>();
                        spr.transform.parent = hidden.transform;
                        spr.transform.localPosition = Vector3.zero;
                    }
                    else
                    {
                        spr = hidden.AddComponent<SpriteRenderer>();
                    }

                    spr.sprite = origin.sprite;
                    spr.sortingOrder = origin.sortingOrder;

                    hidden.transform.position = obj.transform.position;
                }
                else
                {
                    GUIContent icon = EditorGUIUtility.IconContent("sv_icon_dot14_pix16_gizmo");
                    EditorGUIUtility.SetIconForObject(hidden.gameObject, (Texture2D)(icon.image));

                    Vector3 offset = Vector3.zero;
                    var go = new GameObject($"{obj.name}");
                    go.transform.parent = hidden.transform;
                    go.AddComponent<CircleCollider2D>();

                    List<GameObject> allChild = new();
                    for (int j = 0; j < obj.childCount; j++)
                    {
                        var child = obj.GetChild(j);
                        var origin = child.GetComponent<SpriteRenderer>();

                        var goChild = new GameObject($"{child.name}");
                        var spr = goChild.AddComponent<SpriteRenderer>();

                        goChild.transform.position = child.transform.position;
                        spr.sprite = origin.sprite;
                        spr.sortingOrder = origin.sortingOrder;

                        offset += child.transform.position;
                        allChild.Add(goChild);
                    }

                    offset /= obj.childCount;
                    hidden.transform.localPosition = offset;
                    go.transform.localPosition = Vector3.zero;
                    foreach (var child in allChild)
                        child.transform.parent = go.transform;
                }

                willDestroy.Add(obj.gameObject);
                //obj.parent = null;
                //Destroy(obj.gameObject);
                //i--;
            }
            else if (obj.name.Contains(Const.PREFIX_ANIM_OBJECT))
            {
                var go = new GameObject($"{obj.name}");
                willAddToMap.Add(go, obj.gameObject);
                //var origin = obj.GetComponentsInChildren<SpriteRenderer>();

                if (obj.childCount == 0)
                {
                    go.transform.position = obj.transform.position;
                }
                else
                {
                    Vector3 pos = Vector3.zero;
                    for (int j = 0; j < obj.childCount; j++)
                    {
                        pos += obj.GetChild(j).transform.position;
                    }

                    pos /= obj.childCount;
                    go.transform.position = pos;

                    for (int j = 0; j < obj.childCount; j++)
                    {
                        obj.GetChild(j).transform.position -= pos;
                    }
                }
            }
        }

        foreach (var i in willAddToMap)
        {
            i.Key.transform.parent = _mapObj.transform;
            i.Value.transform.parent = i.Key.transform;
            i.Value.transform.position = i.Key.transform.position;
        }

        foreach (var i in willDestroy)
        {
            i.transform.parent = null;
            Destroy(i.gameObject);
        }

        foreach (Transform g in _mapObj.GetComponentsInChildren<Transform>())
        {
            if (g.name.Contains(Const.PREFIX_ANIM_OBJECT))
            {
                if (g.TryGetComponent<SpriteRenderer>(out SpriteRenderer sp))
                    sp.gameObject.AddComponent<SpritePivotHelper>();
            }
        }

        foreach (Transform g in hiddenObj.GetComponentsInChildren<Transform>())
        {
            if (g.name.Contains(Const.PREFIX_ANIM_OBJECT))
            {
                if (g.TryGetComponent<SpriteRenderer>(out SpriteRenderer sp))
                    sp.gameObject.AddComponent<SpritePivotHelper>();
            }
        }
    }

    private string GetShortCutAnimObject(string name)
    {
        return GetSpriteName(name);
    }

    private string GetSpriteName(string objName)
    {
        string[] split = objName.Split('_');
        var _itemName = string.Empty;

        if (split.Length == 1)
            _itemName = $"{split[0]}_";
        else
            for (int i = split.Length - 1; i >= 0; i--)
            {
                if (split[i].Equals("h") || split[i].Equals("a"))
                {
                    for (int j = 0; j <= i + 1; j++)
                    {
                        _itemName += $"{split[j]}_";
                    }
                    break;
                }
            }

        if (string.IsNullOrEmpty(_itemName))
        {
            _itemName = $"{split[0]}_{split[1]}_";
        }

        return _itemName.Substring(0, _itemName.Length - 1);
    }

#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(StageMaker))]
public class StageMakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generated Stage", GUILayout.Height(25)))
            StageMaker.Instance.GeneratedStage();
    }
}

public class RemoveDuplicateImage : EditorWindow
{
    private UnityEngine.Object folder;

    [MenuItem("Tools/Gameplay/Remove duplicate image")]
    public static void Init()
    {
        RemoveDuplicateImage window = EditorWindow.GetWindowWithRect<RemoveDuplicateImage>(new Rect(Vector2.zero, new Vector2(300, 200)));
        window.titleContent = new GUIContent("Remove duplicate image");
        window.Show();
    }

    struct TextureComparison
    {
        public int width;
        public int height;
        public static bool operator ==(TextureComparison a, TextureComparison b)
        {
            return a.width == b.width && a.height == b.height;
        }

        public static bool operator !=(TextureComparison a, TextureComparison b)
        {
            return a.width != b.width || a.height != b.height;
        }

        public override bool Equals(object obj)
        {
            return this == (TextureComparison)obj;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(30);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Folder", GUILayout.Width(100));
        folder = EditorGUILayout.ObjectField(folder, typeof(UnityEngine.Object), false, GUILayout.MaxWidth(200));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Confirm", GUILayout.Width(100)))
        {
            var GUIs = AssetDatabase.FindAssets("t:texture2D", new string[] { AssetDatabase.GetAssetPath(folder) });
            List<string> keep = new();
            List<string> remove = new();
            string log = string.Empty;
            foreach (var gui in GUIs)
            {
                string path = AssetDatabase.GUIDToAssetPath(gui);
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                string[] str = obj.name.Split('_');
                string key = GetSpriteName(obj.name);

                if (!keep.Contains(key))
                {
                    keep.Add(key);
                    AssetDatabase.RenameAsset(path, key);
                }
                else
                {
                    remove.Add(path);
                    log += $"{obj.name}, ";
                }
            }

            AssetDatabase.DeleteAssets(remove.ToArray(), new List<string>());
            log = $"Delete {remove.Count} file : " + log;
            Debug.Log(log);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Check file", GUILayout.Width(100)))
        {
            var GUIs = AssetDatabase.FindAssets("t:texture2D", new string[] { AssetDatabase.GetAssetPath(folder) });

            Dictionary<string, TextureComparison> item = new Dictionary<string, TextureComparison>();
            Dictionary<string, List<string>> logDict = new Dictionary<string, List<string>>();
            List<string> logHasSpace = new List<string>();
            Dictionary<Hash128, List<string>> logDictSameTexture = new Dictionary<Hash128, List<string>>();

            string logNamingWrong = "naming texture wrong :\n";
            foreach (var gui in GUIs)
            {
                string path = AssetDatabase.GUIDToAssetPath(gui);
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>(path);
                string key = GetSpriteName(obj.name);

                TextureComparison comparison = new TextureComparison() { height = obj.height, width = obj.width };

                if (item.ContainsKey(key))
                {
                    if (obj.width != item[key].width || obj.height != item[key].height)
                    {
                        var index = item.ToList().FindIndex(x => x.Key.Equals(key));
                        logDict.ElementAt(index).Value.Add(obj.name);
                    }
                }
                else
                {
                    item.Add(key, comparison);
                    logDict.Add(obj.name, new List<string>());
                }

                if (obj.name.Contains(' '))
                    logHasSpace.Add(obj.name);

                Hash128 has = obj.imageContentsHash;
                if (logDictSameTexture.ContainsKey(has))
                {
                    logDictSameTexture[has].Add(obj.name);
                }
                else
                    logDictSameTexture.Add(has, new List<string>() { obj.name });
            }

            foreach (var i in logDict)
            {
                if (i.Value.Count == 0)
                    continue;

                string l = string.Empty;
                foreach (var j in i.Value)
                    l += $"{j}, ";

                logNamingWrong += $"<color=red>{i.Key}</color> : {l}\n";
            }
            Debug.Log(logNamingWrong);

            string hasSpace = "\nTexture has space : ";
            foreach (var i in logHasSpace)
                hasSpace += $"{i}, ";
            Debug.Log(hasSpace);

            string logSameTex = "\nTexture same content :";
            int count = 0;
            foreach (var i in logDictSameTexture)
            {
                if (i.Value.Count > 1)
                {
                    logSameTex += $"\n<color=#ADD8E6>{i.Value[0]}</color> : ";
                    for (int j = 1; j < i.Value.Count; j++)
                    {
                        logSameTex += $"{i.Value[j]}, ";
                        count++;
                    }
                }
            }
            Debug.Log(logSameTex + $"\nTotal {count} texture dulicate that can delete");
        }
        if (GUILayout.Button("Log and delete"))
        {
            string folderPath = AssetDatabase.GetAssetPath(folder);
            var GUIs = AssetDatabase.FindAssets("t:texture2D", new string[] { folderPath });
            Dictionary<Hash128, List<string>> checkDulicate = new Dictionary<Hash128, List<string>>();
            Dictionary<string, string> traceTex = new Dictionary<string, string>();
            string txtpath = $"{folderPath}/{folder.name}.txt";

            int count = 0;
            string log = string.Empty;

            using (StreamWriter writer = new StreamWriter(txtpath, false))
            {
                foreach (var gui in GUIs)
                {
                    string path = AssetDatabase.GUIDToAssetPath(gui);
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>(path);
                    Hash128 hash = obj.imageContentsHash;
                    if (checkDulicate.ContainsKey(hash))
                    {
                        checkDulicate[hash].Add(obj.name);
                        traceTex.Add(obj.name, checkDulicate[hash][0]);
                        log += $"{obj.name}, ";
                        AssetDatabase.DeleteAsset(path);
                        count++;
                    }
                    else
                    {
                        checkDulicate.Add(obj.imageContentsHash, new List<string>() { obj.name });
                        traceTex.Add(obj.name, checkDulicate[hash][0]);
                    }
                }

                Debug.Log($"Delete {count} file : {log}");
                JsonWriter jsonWriter = new JsonWriter();
                jsonWriter.PrettyPrint = true;
                JsonMapper.ToJson(traceTex, jsonWriter);
                writer.Write(jsonWriter.ToString());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    private string GetSpriteName(string objName)
    {
        string[] split = objName.Split('_');
        var _itemName = string.Empty;

        if (split.Length == 1)
            _itemName = $"{split[0]}_";
        else
            for (int i = split.Length - 1; i >= 0; i--)
            {
                if (split[i].Equals("h") || split[i].Equals("a"))
                {
                    for (int j = 0; j <= i + 1; j++)
                    {
                        _itemName += $"{split[j]}_";
                    }
                    break;
                }
            }

        if (string.IsNullOrEmpty(_itemName))
        {
            _itemName = $"{split[0]}_{split[1]}_";
        }

        return _itemName.Substring(0, _itemName.Length - 1);
    }
}
#endif