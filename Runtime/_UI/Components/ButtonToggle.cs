using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 按钮式Toggle
/// </summary>
[RequireComponent( typeof(Button))]
public class ButtonToggle : MonoBehaviour
{
    /// <summary>
    /// 选中状态下Sprite
    /// </summary>
    public Sprite chooseSprite;
    [SerializeField]
    private bool _choose;
    /// <summary>
    /// 状态
    /// </summary>
    public bool Choose {
        get {
            return _choose;
        }
        set {
            _choose = value;
            chooseStateChange?.Invoke(_choose);
            ChangeBtnState();
        }
    }
    /// <summary>
    /// 状态改变回调Action
    /// </summary>
    public Action<bool> chooseStateChange;
    private Sprite _defaultSprite;
    private Button _button;
    private Image _image;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _defaultSprite = _image.sprite;       
        _button.onClick.AddListener(OnBtnClick);
        ChangeBtnState();
    }

    private void OnBtnClick()
    {
        Choose = !Choose;
        
    }
    void ChangeBtnState() {
        if (_choose)
        {
            _image.sprite = chooseSprite;
        }
        else {
            _image.sprite = _defaultSprite;
        }
    }
}
