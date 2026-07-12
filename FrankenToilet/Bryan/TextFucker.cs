namespace FrankenToilet.Bryan;

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> fucks your text </summary>
public class TextFucker : MonoBehaviour
{
    /// <summary> Text component on this gameObject. </summary>
    public TextMeshProUGUI Text;

    /// <summary> Legacy text component on this gameObject. </summary>
    public Text Legacy;

    /// <summary> The color b4 it was changed. </summary>
    public Color preColor;

    /// <summary> Start lateStart. </summary>
    public void Start() =>
        StartCoroutine(LateStart());

    /// <summary> Grab text and preColor. </summary>
    public IEnumerator LateStart()
    {
        for (int i = 0; i < 10; i++) yield return null;
        Text = GetComponent<TextMeshProUGUI>();
        Legacy = GetComponent<Text>();
        preColor = (Text?.color ?? Legacy?.color) ?? Color.white;
    }

    /// <summary> fuck le text >:3 </summary>
    public void LateUpdate()
    {
        Color col = Femboy.fuckText ? Color.HSVToRGB(Mathf.LerpUnclamped(0f, 0.2f, Time.realtimeSinceStartup % 5), 1f, 1f) : preColor;

        Text?.color = col;
        Legacy?.color = col;
    }
}
