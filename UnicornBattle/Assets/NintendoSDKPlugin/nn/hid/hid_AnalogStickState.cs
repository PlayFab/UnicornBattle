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
    public struct AnalogStickState
    {
        public const int Max = 0x7FFF;
        public int x;
        public int y;

        public float fx { get { return (float)this.x / AnalogStickState.Max; } }
        public float fy { get { return (float)this.y / AnalogStickState.Max; } }

        public void Clear()
        {
            this.x = 0;
            this.y = 0;
        }

        public override string ToString()
        {
            return string.Format("({0,6} {1,6})", this.x, this.y);
        }
    }
}
#endif
