using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


public class VideoInterface : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    public List<VideoClip> clips;
    public CanvasGroup canvasGroup;

    public GameObject loader;

    void Start()
    {
        if (clips.Count > 0)
        {
            videoPlayer.clip = clips[0];
            canvasGroup.alpha = 0.4f;
            StartCoroutine(waitForVideoToPrepare());

        }
    }

    IEnumerator waitForVideoToPrepare()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoPlayer.Play();
        loader.SetActive(false);

    }

    public void CustomAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    public void SetClip(int index)
    {
        canvasGroup.alpha = 1f;
        if (index >= 0 && index < clips.Count)
        {
            videoPlayer.clip = clips[index];
            if(index == 0)
            {
                canvasGroup.alpha = 0.4f;
            }
            if (index == 4)
            {
                canvasGroup.alpha = 0.2f;
            }
            if(index == 1 || index == 2 || index == 3)
            {
                canvasGroup.alpha = 0.2f;
            }
            StartCoroutine(waitForVideoToPrepare());
        }
    }
}
