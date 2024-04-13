using System;
using UnityEngine;

public enum ItemState
{
    NotSet = 0, Show = 1, Found = 2
}

public class HiddenItem : MonoBehaviour,IEquatable<HiddenItem>
{
    [SerializeField] private int _id;
    [SerializeField] private string _itemName;
    [SerializeField] private GroupType _type;
    [SerializeField] private float _radiusClick = 0.5f;

    private ItemState _state;

    public Action<HiddenItem> onClick;
    public int Id => _id;
    public GroupType Type => _type;
    public ItemState State => _state;
    public string ItemName => _itemName;

    public void Init()
    {
        _state = ItemState.Show;
        InputControl.Instance.onFingerDown += OnClick;
    }

    private void OnClick(Vector3 pos)
    {
        if(Vector3.Distance(transform.position, pos) < _radiusClick)
        {
            _state = ItemState.Found;
            onClick?.Invoke(this);
            InputControl.Instance.onFingerDown -= OnClick;
        }
    }

    private void Reset()
    {
        _id = GetHashCode();
    }
    public bool Equals(HiddenItem other)
    {
        return _id == other._id;
    }

    private void OnDrawGizmos()
    {
        if(_state != ItemState.Found)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radiusClick);
        }
    }
}
