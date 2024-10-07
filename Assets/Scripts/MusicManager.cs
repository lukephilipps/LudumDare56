using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Singleton;

    public AudioSource intro;
    public AudioSource[] ostLoops;
    public float lerpSpeed = 0.25f;

    private bool lerpingVolumes;
    private int currentTrack = 0;
    private int previousTrack = 0;

    private void Start()
    {
        Singleton = this;

        foreach (var song in ostLoops)
            song.PlayDelayed(2.0f);
    }

    private void Update()
    {
        if (lerpingVolumes)
        {
            ostLoops[previousTrack].volume = Mathf.Lerp(ostLoops[previousTrack].volume, 0.0f, lerpSpeed * Time.deltaTime);
            ostLoops[currentTrack].volume = Mathf.Lerp(ostLoops[currentTrack].volume, 0.08f, lerpSpeed * Time.deltaTime);

            if (ostLoops[currentTrack].volume > 0.0795f)
            {
                ostLoops[previousTrack].volume = 0.0f;
                ostLoops[currentTrack].volume = 0.08f;
                lerpingVolumes = false;
            }
        }
    }

    public void ActivateTrack(int trackNum)
    {
        if (trackNum < 0) trackNum = 0;
        if (trackNum >= ostLoops.Length) trackNum = ostLoops.Length - 1;

        foreach (var song in ostLoops)
            song.volume = 0.0f;

        lerpingVolumes = true;
        ostLoops[currentTrack].volume = 0.08f;
        previousTrack = currentTrack;
        currentTrack = trackNum;
    }
}
