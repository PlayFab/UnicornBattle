/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE 
using System;
using System.Runtime.InteropServices;

namespace nn.hid
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VibrationValue
    {
        public const int FrequencyLowDefault = 160;
        public const int FrequencyHighDefault = 320;

        public float amplitudeLow;
        public float frequencyLow;
        public float amplitudeHigh;
        public float frequencyHigh;

        public static VibrationValue Make()
        {
            return new VibrationValue(0f, FrequencyLowDefault, 0f, FrequencyHighDefault);
        }

        public static VibrationValue Make(
            float amplitudeLow, float frequencyLow, float amplitudeHigh, float frequencyHigh)
        {
            return new VibrationValue(amplitudeLow, frequencyLow, amplitudeHigh, frequencyHigh);
        }

        public VibrationValue(float amplitudeLow, float frequencyLow, float amplitudeHigh, float frequencyHigh)
        {
            this.amplitudeLow = amplitudeLow;
            this.frequencyLow = frequencyLow;
            this.amplitudeHigh = amplitudeHigh;
            this.frequencyHigh = frequencyHigh;
        }

        public void Set(float amplitudeLow, float frequencyLow, float amplitudeHigh, float frequencyHigh)
        {
            this.amplitudeLow = amplitudeLow;
            this.frequencyLow = frequencyLow;
            this.amplitudeHigh = amplitudeHigh;
            this.frequencyHigh = frequencyHigh;
        }

        public void Clear()
        {
            this.amplitudeLow = 0f;
            this.frequencyLow = FrequencyLowDefault;
            this.amplitudeHigh = 0f;
            this.frequencyHigh = FrequencyHighDefault;
        }

        public override string ToString()
        {
            return string.Format("Low({0} {1}Hz) High({2} {3}Hz)",
                amplitudeLow, frequencyLow, amplitudeHigh, frequencyHigh);
        }
    }
}
#endif
