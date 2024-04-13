using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputControl : MMSingleton<InputControl>
{
    public enum InputType
    {
        None = 0, Mouse = 1, Touch = 2
    }

    private InputType _inputType;
    private bool _enableInput;
    private Camera _camera;

    public Action<Vector3> onFingerDown;

    public bool EnableInput { get => _enableInput; set => _enableInput = value; }
    public Camera Camera => _camera;

    private void Start()
    {
        if(_camera == null)
            _camera = Camera.main;

        _inputType = Input.touchSupported? InputType.Touch : InputType.Mouse;
    }

    private void Update()       
    {
        if (!_enableInput) return;

        if(_inputType == InputType.Touch && Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && IsPointerOverUIObject())
            {
                Vector3 pos = _camera.ScreenToWorldPoint(touch.position);
                onFingerDown?.Invoke(new Vector3(pos.x, pos.y,0));
            }      
        }
        else if(_inputType == InputType.Mouse)
        {
            if (Input.GetMouseButtonDown(0) && IsPointerOverUIObject())
            {
                Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
                onFingerDown?.Invoke(new Vector3(pos.x, pos.y, 0));
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if (_inputType == InputType.Touch)
        {
            if (Input.touchCount == 0)
                return false;
            eventData.position = Input.touches[0].position;
        }
        else if(_inputType == InputType.Mouse)
            eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

}
