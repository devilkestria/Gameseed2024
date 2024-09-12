using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlackscreenManager : MonoBehaviour
{
    [FoldoutGroup("Blackscreen Manager")][SerializeField] private GameObject panelBlackscreen;
    [FoldoutGroup("Blackscreen Manager")][SerializeField] private Image imgBlackscreen;
    [FoldoutGroup("Blackscreen Manager")][SerializeField] private float timeFade;
    [FoldoutGroup("Blackscreen Manager")] float deltaTimeFade;
    [FoldoutGroup("Blackscreen Manager")] public UnityAction eventOnFadeIn;
    [FoldoutGroup("Blackscreen Manager")] public UnityAction eventOnFadeOut;
    [FoldoutGroup("Blackscreen Manager")] Color color = Color.black;

    public void FadeIn()
    {
        deltaTimeFade = 0;
        color.a = 0;
        imgBlackscreen.color = color;
        StartCoroutine(IeFade(true));
    }
    public void FadeOut()
    {
        deltaTimeFade = 0;
        color.a = 1;
        imgBlackscreen.color = color;
        StartCoroutine(IeFade(false));
    }

    IEnumerator IeFade(bool FadeIn)
    {
        while (deltaTimeFade < timeFade)
        {
            float alpha = Mathf.Lerp(FadeIn ? 0 : 1, FadeIn ? 1 : 0, deltaTimeFade / timeFade);
            color.a = alpha;
            imgBlackscreen.color = color;
            deltaTimeFade += Time.deltaTime;
            yield return null;
        }
        color.a = FadeIn ? 1 : 0;
        imgBlackscreen.color = color;
        if (FadeIn)
            eventOnFadeIn?.Invoke();
        else
            eventOnFadeOut?.Invoke();
    }
}
