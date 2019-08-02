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
namespace nn.hid
{
    public static partial class ControllerSupport
    {
        public static ErrorRange ResultCanceled { get { return new ErrorRange(202, 3101, 3102); } }
        public static ErrorRange ResultNotSupportedNpadStyle { get { return new ErrorRange(202, 3102, 3103); } }
    }

    public static partial class ControllerFirmwareUpdate
    {
        public static ErrorRange ResultControllerFirmwareUpdateError { get { return new ErrorRange(202, 3200, 3210); } }
        public static ErrorRange ResultControllerFirmwareUpdateFailed { get { return new ErrorRange(202, 3201, 3202); } }
    }
}
#endif
