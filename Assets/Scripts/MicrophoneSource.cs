using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class MicrophoneSource : MonoBehaviour
{
    private string CurrentAudioSource;
    private AudioSource mAudioSource;

    const int WINDOW_SIZE = 1 << 10;
    const int SAMPLING_RATE = 44100;
    const float VOLUME_MIN = 0.001f;

    private BeatDetector mBeatDetector;

    private bool mStarted;

    public float[][] audioData = new float[2][];
    public float mVolume;
    private float mBeat;

    void Start()
    {
        mAudioSource = GetComponent<AudioSource>();
        mBeatDetector = new BeatDetector(WINDOW_SIZE, SAMPLING_RATE);
        CurrentAudioSource = Microphone.devices[0].ToString();
        mStarted = false;
        audioData[0] = new float[WINDOW_SIZE];
        audioData[1] = new float[WINDOW_SIZE];
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 400), CurrentAudioSource);
    }

    // Update is called once per frame
    void Update()
    {
        if (!mStarted)
        {
            mAudioSource.clip = Microphone.Start(CurrentAudioSource, true, 1, SAMPLING_RATE);
            while (!(Microphone.GetPosition(CurrentAudioSource) > 0)) { }
            mAudioSource.Play();

            mStarted = true;
        }

        mAudioSource.GetOutputData(audioData[0], 0);
        mAudioSource.GetSpectrumData(audioData[1], 0, FFTWindow.BlackmanHarris);

        // Sum Calculation
        float sum = 0;
        foreach (float f in audioData[0])
        {
            sum += f;
        }

        sum /= audioData[1].Length;
        mVolume = sum;

        // Beat calculation
        mBeat = mBeatDetector.Update(SpectrumData(), Time.time);
    }

    public float[] OutputData()
    {
        return audioData[0];
    }

    public float[] SpectrumData()
    {
        return audioData[1];
    }

    public float Volume()
    {
        return mVolume;
    }

    public float Beat()
    {
        return mBeat;
    }
}
