using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class AbstractSceneManager: MonoBehaviour
{
    public Transform content;
    internal AbstractPopup current;

    private Dictionary<string, AbstractPopup> cache = new();

    public abstract void Display(string alias, string custom, string trackingName);

    internal protected AbstractPopup OpenPrefab(string alias, string custom)
    {
        if (!cache.ContainsKey(alias))
        {
            AbstractPopup popup = Resources.Load<AbstractPopup>(alias);
            Debug.Log("popup as: " + alias);
            var obj = Instantiate(popup, content);
            obj.gameObject.name = alias;
            cache.Add(alias, obj);
        }
        current = cache[alias];
        current.gameObject.SetActive(true);
        current.transform.SetAsLastSibling();
        current.manager = this;
        current.OpenPopup(custom);
        return cache[alias];
    }

    public abstract void OnEndClose();
}

