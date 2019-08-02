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

namespace nn
{
    public static partial class Nn
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        internal const string DllName = "NintendoSDKPlugin";
#else
        internal const string DllName = "__Internal";
#endif
    }
}
#endif
