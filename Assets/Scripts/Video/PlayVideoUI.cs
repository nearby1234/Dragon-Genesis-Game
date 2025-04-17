using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideoUI : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoFileName = "Video/DragonGenesisBG.mp4";

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
    }
    void Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = path;
        videoPlayer.Play();
    }
}
