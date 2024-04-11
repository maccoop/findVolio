using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryController : AbstractPopup
{
    public TMPro.TMP_Text description;
    public Image thumb;
    public TMPro.TMP_Text story;
    private int indexNote;
    bool loadSuccess;
    private Story data;

    public int IndexNote
    {
        get
        {
            return indexNote;
        }
        set
        {
            indexNote = value;
            loadSuccess = false;
            StartCoroutine(CoDisplayNote());
        }
    }

    public override void OpenPopup(string custom)
    {
        base.OpenPopup(custom);
        data = StoryConfig.Instance.GetStory(custom);
        description.text = data.Description;
        thumb.sprite = data.Thumbnail;
        IndexNote = 0;
        loadSuccess = false;
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            if (loadSuccess)
            {
                IndexNote += 1;
            }
            else
            {
                loadSuccess = true;
            }
        }
    }

    private IEnumerator CoDisplayNote()
    {
        if (indexNote >= data.notes.Length)
            yield break;
        var note = data.notes[IndexNote];
        int indexChar = 0;
        int length = note.Length;
        string currentString = "";
        var waitDeltaTime = new WaitForSeconds(0.1f);
        while (indexChar < length && !loadSuccess)
        {
            currentString += note[indexChar];
            story.text = currentString;
            yield return waitDeltaTime;
            indexChar++;
        }
        story.text = currentString;
        loadSuccess = true;
    }
}
