using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

public class TriggerPrison : MonoBehaviour
{
    [FoldoutGroup("Trigger Prison")][SerializeField] private GameObject Enemy;
    [FoldoutGroup("Trigger Prison")][SerializeField] private List<BasicMonster> listMonster;
    [FoldoutGroup("Trigger Prison")][SerializeField] private CinemachineVirtualCamera vcEnemy;
    [FoldoutGroup("Trigger Prison")][SerializeField] private GameObject objPrison;
    [FoldoutGroup("Trigger Prison")][SerializeField] private BasicPlayerController playerController;
    [FoldoutGroup("Trigger Prison")][SerializeField] private bool isFinish = false;
    [FoldoutGroup("Trigger Prison")][SerializeField] private bool isDone = false;
    [FoldoutGroup("Trigger Prison")][SerializeField] private float startHeightPrison;
    [FoldoutGroup("Trigger Prison")][SerializeField] private float finalHeightPrison;
    [FoldoutGroup("Trigger Prison")][SerializeField] private float timeWaiting;
    [FoldoutGroup("Trigger Prison")] private WaitForSeconds wfsTimeWaiting;
    [FoldoutGroup("Trigger Prison")][SerializeField] private float timeMovingPrison;
    [FoldoutGroup("Trigger Prison")] private float deltaTimeMovingPrison;
    [FoldoutGroup("Trigger Prison")][SerializeField] private AudioSource audioSource;
    [FoldoutGroup("Trigger Prison")][SerializeField] private AudioClip audioClipTraped;
    [FoldoutGroup("Trigger Prison")][SerializeField] private AudioClip audioClipSuccess;
    public void CheckReset()
    {
        if (isFinish) return;
        isDone = false;
        foreach(BasicMonster mosnter in listMonster)
        {
            mosnter.ChangeState(EnemyState.EnemyIddle);
        }
        Enemy.SetActive(false);
        objPrison.transform.position = new Vector3(objPrison.transform.position.x, startHeightPrison, objPrison.transform.position.z);
        objPrison.SetActive(false);
    }
    private void Start()
    {
        wfsTimeWaiting = new WaitForSeconds(timeWaiting);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCam") && !isDone)
        {
            isDone = true;
            playerController.ChangeState(PlayerState.PlayerIddle);
            playerController.GetComponent<Animator>().SetFloat("Speed", 0);
            deltaTimeMovingPrison = 0;
            StartCoroutine(IePrisonStart());
        }
    }
    IEnumerator IePrisonStart()
    {
        objPrison.SetActive(true);
        StartCoroutine(IeMovingPrison(true));
        yield return wfsTimeWaiting;
        Enemy.SetActive(true);
        foreach(BasicMonster mosnter in listMonster)
        {
            mosnter.ChangeState(EnemyState.EnemyWaiting);
        }
        vcEnemy.Priority = 3;
        yield return wfsTimeWaiting;
        audioSource.PlayOneShot(audioClipTraped);
        yield return wfsTimeWaiting;
        vcEnemy.Priority = 0;
        playerController.ChangeState(PlayerState.PlayerMoving);
    }
    IEnumerator IeMovingPrison(bool goUp)
    {
        while (deltaTimeMovingPrison < timeMovingPrison)
        {
            float height = Mathf.Lerp(goUp ? startHeightPrison : finalHeightPrison, goUp ? finalHeightPrison : startHeightPrison, deltaTimeMovingPrison / timeMovingPrison);
            objPrison.transform.position = new Vector3(objPrison.transform.position.x, height, objPrison.transform.position.z);
            deltaTimeMovingPrison += Time.deltaTime;
            yield return null;
        }
        objPrison.transform.position = new Vector3(objPrison.transform.position.x, goUp ? finalHeightPrison : startHeightPrison, objPrison.transform.position.z);
        if (!goUp) objPrison.SetActive(false);
    }

    public void OnFinishPrison()
    {
        isFinish = true;
        audioSource.PlayOneShot(audioClipSuccess);
        deltaTimeMovingPrison = 0;
        StartCoroutine(IeMovingPrison(false));
    }
}
