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
namespace nn.fs
{
    public static partial class FileSystem
    {
        public static ErrorRange ResultHandledByAllProcess { get { return new ErrorRange(2, 0, 1000); } }
        public static ErrorRange ResultPathNotFound { get { return new ErrorRange(2, 1, 2); } }
        public static ErrorRange ResultPathAlreadyExists { get { return new ErrorRange(2, 2, 3); } }
        public static ErrorRange ResultTargetLocked { get { return new ErrorRange(2, 7, 8); } }
        public static ErrorRange ResultDirectoryNotEmpty { get { return new ErrorRange(2, 8, 9); } }
        public static ErrorRange ResultDirectoryStatusChanged { get { return new ErrorRange(2, 13, 14); } }
        public static ErrorRange ResultUsableSpaceNotEnough { get { return new ErrorRange(2, 30, 46); } }
        public static ErrorRange ResultUnsupportedSdkVersion { get { return new ErrorRange(2, 50, 51); } }
        public static ErrorRange ResultMountNameAlreadyExists { get { return new ErrorRange(2, 60, 61); } }
        public static ErrorRange ResultTargetNotFound { get { return new ErrorRange(2, 1002, 1003); } }
    }

    public static partial class SaveData
    {
        public static ErrorRange ResultUsableSpaceNotEnoughForSaveData { get { return new ErrorRange(2, 31, 32); } }
    }

    public static partial class Host
    {
        public static ErrorRange ResultSaveDataHostFileSystemCorrupted { get { return new ErrorRange(2, 4441, 4460); } }
        public static ErrorRange ResultSaveDataHostEntryCorrupted { get { return new ErrorRange(2, 4442, 4443); } }
        public static ErrorRange ResultSaveDataHostFileDataCorrupted { get { return new ErrorRange(2, 4443, 4444); } }
        public static ErrorRange ResultSaveDataHostFileCorrupted { get { return new ErrorRange(2, 4444, 4445); } }
        public static ErrorRange ResultInvalidSaveDataHostHandle { get { return new ErrorRange(2, 4445, 4446); } }
        public static ErrorRange ResultHostFileSystemCorrupted { get { return new ErrorRange(2, 4701, 4720); } }
        public static ErrorRange ResultHostEntryCorrupted { get { return new ErrorRange(2, 4702, 4703); } }
        public static ErrorRange ResultHostFileDataCorrupted { get { return new ErrorRange(2, 4703, 4704); } }
        public static ErrorRange ResultHostFileCorrupted { get { return new ErrorRange(2, 4704, 4705); } }
        public static ErrorRange ResultInvalidHostHandle { get { return new ErrorRange(2, 4705, 4706); } }
    }

    public static partial class Rom
    {
        public static ErrorRange ResultRomHostFileSystemCorrupted { get { return new ErrorRange(2, 4241, 4260); } }
        public static ErrorRange ResultRomHostEntryCorrupted { get { return new ErrorRange(2, 4242, 4243); } }
        public static ErrorRange ResultRomHostFileDataCorrupted { get { return new ErrorRange(2, 4243, 4244); } }
        public static ErrorRange ResultRomHostFileCorrupted { get { return new ErrorRange(2, 4244, 4245); } }
        public static ErrorRange ResultInvalidRomHostHandle { get { return new ErrorRange(2, 4245, 4246); } }
    }
}
#endif
