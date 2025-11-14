using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


using System.Collections;

public class BunnyInterface : MonoBehaviour
{

    public List<Transform> pathPoints1;
    public List<Transform> pathPoints2;
    public List<Transform> pathPoints3;

    public Transform enterPoint;

    private float jumpDuration = 0.64f;
    private float idleDuration = 0.71f;

    private float walkDuration = 0.61f;

    public float exitDuration;

    public Animator animator;

    public GameObject parentObject;
    public bool loopAnimation = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        animator.Play("Jump", 0, 0f);
        gameObject.transform.position = pathPoints1[0].position;
        transform.DOPath(pathPoints1.ConvertAll(p => p.position).ToArray(), jumpDuration, PathType.CatmullRom, PathMode.TopDown2D).onComplete += () =>
        {
            StartCoroutine(waitForNextAnimation());
        };
    }

    public IEnumerator waitForNextAnimation()
    {
        yield return new WaitForSeconds(idleDuration);
        gameObject.transform.position = enterPoint.position;
        NextAnimation();
        
    }
    public void NextAnimation()
    {
        transform.DOPath(pathPoints2.ConvertAll(p => p.position).ToArray(), walkDuration, PathType.CatmullRom, PathMode.TopDown2D).onComplete += () =>
        {
            ExitAnimation();
        };
    }

    public void ExitAnimation()
    {
        transform.DOPath(pathPoints3.ConvertAll(p => p.position).ToArray(), exitDuration, PathType.CatmullRom, PathMode.TopDown2D).onComplete += () =>
        {
            if (loopAnimation)
            {
                Init();
            }
            else
            {
                parentObject.SetActive(false);
            }
        };
    }
    public void StopAnimation()
    {
        transform.DOKill();
    }

    public void StopNextCycle()
    {
        loopAnimation = false;
    }


}
