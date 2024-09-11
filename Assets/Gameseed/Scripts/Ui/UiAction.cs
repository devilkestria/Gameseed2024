using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UiAction : MonoBehaviour
{
    [FoldoutGroup("Ui Action")] [SerializeField] private Image imgCooldown;
    [FoldoutGroup("Ui Action")] private float timeAction;
    [FoldoutGroup("Ui Action")] private float deltaTimeAction;
    public void InitUiAction(float time)
    {
        timeAction = time;
    }
    public void StartCooldown()
    {
        StopAllCoroutines();
        deltaTimeAction = 0;
        imgCooldown.gameObject.SetActive(true);
        imgCooldown.fillAmount = 1;
        StartCoroutine(IeCooldown());
    }
    IEnumerator IeCooldown()
    {
        while(deltaTimeAction < timeAction)
        {
            imgCooldown.fillAmount = Mathf.Lerp(imgCooldown.fillAmount, 0, deltaTimeAction/timeAction);
            deltaTimeAction += Time.deltaTime;
            yield return null;
        }
        imgCooldown.gameObject.SetActive(false);
    }
}
