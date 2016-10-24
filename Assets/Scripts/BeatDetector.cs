using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{

    class BeatDetector
    {
        const float VOLUME_MIN = 0.001f;

        const int BUCKETS = 16;
        const float BEAT_CONSTANT = 1.2f;

        int history;
        float mEnergyIndex;
        float mLastBeat = 0;
        float mTempo = 2f; // BPS
        float[,] mEnergyHistory;
        float[] mAverageEnergy = new float[BUCKETS];

        public BeatDetector(float windowSize, float samplingRate)
        {
            mEnergyIndex = 0;
            history = (int) Math.Floor(samplingRate / windowSize);
            mEnergyHistory = new float[BUCKETS, history];
        }

        public float Update(float[] spectrum, float time) {
            float[] sum = new float[BUCKETS];
            int j = 0;
            int startBucketIndex = 0;
            int bucketSize = 1;
            int i = 0;
            while (j < BUCKETS) {
                if(spectrum[i] > VOLUME_MIN)
                {
                    sum[j] += (spectrum[i] * spectrum[i]) / bucketSize;
                }

                i++;

                if (i >= startBucketIndex + bucketSize) {
                    j++;
                    bucketSize = j + 1;
                    startBucketIndex = i;
                }
            }

            float dBeat = time - mLastBeat;
            float tempo = 1 / dBeat;
            float gaus = (float)((1 + Math.Cos(Math.Abs(tempo - mTempo))) / 2 * Math.PI);
            float weight = gaus * tempo * Math.Abs(tempo - mTempo);

            float beat = -1.0f;
            for (i = 0; i < BUCKETS; ++i) {
                float newBeat = (sum[i] * weight) - BEAT_CONSTANT * mAverageEnergy[i];
                if (beat < newBeat && newBeat > 0) {
                    beat = newBeat;
                    mLastBeat = time;
                }

                mAverageEnergy[i] -=
                    mEnergyHistory[i, (int)mEnergyIndex] / history;
                mEnergyHistory[i, (int) mEnergyIndex] = sum[i];
                mAverageEnergy[i] += mEnergyHistory[i,(int)mEnergyIndex] / history;
                ++mEnergyIndex;
                if (mEnergyIndex >= history) {
                    mEnergyIndex = 0;
                }
            }

            return beat > 0 ? 1f : 0f; 
        }
    }


}
