using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UiHealthBar : MonoBehaviour
{
    [FoldoutGroup("Ui Health Bar")][SerializeField] private Image imgBar;
    [FoldoutGroup("Ui Health Bar")][SerializeField] private float timeChange;
    [FoldoutGroup("Ui Health Bar")] private float deltaTime;
    public void InitHealthBar(float value, float maxvalue)
    {
        imgBar.fillAmount = value / maxvalue;
    }
    public void OnHealthChange(float value, float maxvalue)
    {
        StopAllCoroutines();
        deltaTime = 0;
        StartCoroutine(IeHealthChange(value, maxvalue));
    }
    IEnumerator IeHealthChange(float value, float maxvalue)
    {
        while (deltaTime < timeChange)
        {
            imgBar.fillAmount = Mathf.Lerp(imgBar.fillAmount, value / maxvalue, deltaTime / timeChange);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        imgBar.fillAmount = value / maxvalue;
    }
}
