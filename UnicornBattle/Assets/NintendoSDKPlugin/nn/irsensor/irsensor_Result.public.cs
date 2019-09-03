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
namespace nn.irsensor
{
    public static class IrSensor
    {
        public static ErrorRange ResultIrsensorUnavailable { get { return new ErrorRange(205, 110, 120); } }
        public static ErrorRange ResultIrsensorUnconnected { get { return new ErrorRange(205, 110, 111); } }
        public static ErrorRange ResultIrsensorUnsupported { get { return new ErrorRange(205, 111, 112); } }
        public static ErrorRange ResultIrsensorDeviceError { get { return new ErrorRange(205, 122, 140); } }
        public static ErrorRange ResultIrsensorFirmwareCheckIncompleted { get { return new ErrorRange(205, 150, 151); } }
        public static ErrorRange ResultIrsensorNotReady { get { return new ErrorRange(205, 160, 170); } }
        public static ErrorRange ResultIrsensorDeviceNotReady { get { return new ErrorRange(205, 160, 161); } }
        public static ErrorRange ResultIrsensorDeviceResourceNotAvailable { get { return new ErrorRange(205, 161, 162); } }
        public static ErrorRange ResultHandAnalysisError { get { return new ErrorRange(205, 1100, 1200); } }
        public static ErrorRange ResultHandAnalysisModeIncorrect { get { return new ErrorRange(205, 1101, 1102); } }
    }
}
#endif
