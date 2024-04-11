using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryConfig", menuName = "config/StoryConfig")]
public class StoryConfig : SingletonScriptableObject<StoryConfig>
{
    public List<Story> stories;

    public Story GetStory(string id) => stories.Find(x => x.Id == id);
}

[System.Serializable]
public class Story: AbstractData
{
    [TextArea]
    public string[] notes;
}
