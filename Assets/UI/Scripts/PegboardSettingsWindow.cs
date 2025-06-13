using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PegboardSettingsWindow : MonoBehaviour
{
    public static PegboardSettingsWindow instance;
    [SerializeField] PegboardCreator pegboardCreator;
    [SerializeField] Material pegboardMaterial;
    [SerializeField] Material referenceMaterial;

    [SerializeField] List<TMP_InputField> pegboardSizeFields;
    [SerializeField] List<TMP_InputField> pegboardBaseColorFields;
    [SerializeField] List<TMP_InputField> pegboardGridColorFields;

    [SerializeField] Color invalidHighlightColor;
    Color defaultColor;

    void Awake()
    {
        pegboardSizeFields[0].text = pegboardCreator.size.x.ToString();
        pegboardSizeFields[1].text = pegboardCreator.size.y.ToString();

        UpdateColorInputDisplay("_Base_Color", pegboardBaseColorFields);
        UpdateColorInputDisplay("_Grid_Color", pegboardGridColorFields);
        transform.localScale = new Vector3(0, 0, 0);
        instance = this;
    }

    public void Show()
    { 
        gameObject.transform.DOScale(1f, 0.25f/2);
    }

    public void Hide()
    { 
        gameObject.transform.DOScale(0f, 0.25f/2);
    }

    public void ChangeBaseColor() => ChangeColorValue("_Base_Color", pegboardBaseColorFields);
    public void ResetBaseColor() => ResetColor("_Base_Color", pegboardBaseColorFields);
    public void ChangeGridColor() => ChangeColorValue("_Grid_Color", pegboardGridColorFields);
    public void ResetGridColor() => ResetColor("_Grid_Color", pegboardGridColorFields);

    public void ChangeSize()
    {
        bool valid = true;
        List<int> values = new();

        foreach (var field in pegboardSizeFields)
        {
            if (!int.TryParse(field.text, out int dimention) || dimention <= 0)
            {
                var image = field.GetComponent<Image>();
                if (defaultColor == default) defaultColor = image.color;
                AnimateInvalidation(image);
                valid = false;
            }
            else
            {
                values.Add(dimention);
            }
        }

        if (!valid) return;

        pegboardCreator.Create(new(values[0], values[1]));
    }

    private void ChangeColorValue(string targetValue, List<TMP_InputField> inputFields)
    {
        bool valid = true;
        List<int> values = new();

        foreach (var field in inputFields)
        {
            if (!int.TryParse(field.text, out int colorValue)
                || colorValue < 0
                || colorValue > 255)
            {
                var image = field.GetComponent<Image>();
                if (defaultColor == default) defaultColor = image.color;
                AnimateInvalidation(image);
                valid = false;
            }
            else
            {
                values.Add(colorValue);
            }
        }

        if (!valid) return;
        Color result = new((float)values[0] / 255, (float)values[1] / 255, (float)values[2] / 255);
        pegboardMaterial.SetColor(targetValue, result);
    }

    private void ResetColor(string targetColor, List<TMP_InputField> inputFields)
    {
        Color color = referenceMaterial.GetColor(targetColor);
        pegboardMaterial.SetColor(targetColor, color);
        UpdateColorInputDisplay(targetColor, inputFields);
    }

    private void AnimateInvalidation(Image image)
    {
        Sequence invalidSequence = DOTween.Sequence();
        invalidSequence.Append(image.DOColor(invalidHighlightColor, 0.1f))
            .Append(image.DOColor(defaultColor, 0.1f))
            .Append(image.DOColor(invalidHighlightColor, 0.1f))
            .Append(image.DOColor(defaultColor, 0.1f));

        invalidSequence.Play();
    }

    private void UpdateColorInputDisplay(string targetColor, List<TMP_InputField> inputFields)
    {
        Color color = pegboardMaterial.GetColor(targetColor);
        inputFields[0].text = Mathf.RoundToInt(color.r * 255).ToString();
        inputFields[1].text = Mathf.RoundToInt(color.g * 255).ToString();
        inputFields[2].text = Mathf.RoundToInt(color.b * 255).ToString();
    }
}
