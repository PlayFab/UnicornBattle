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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace nn.irsensor
{
    public static partial class ImageTransferProcessor
    {
        public const int QvgaImageSize = 320 * 240;
        public const int QqvgaImageSize = 160 * 120;
        public const int QqqvgaImageSize = 80 * 60;

        public const int ImageSize320x240 = 320 * 240;
        public const int ImageSize160x120 = 160 * 120;
        public const int ImageSize80x60 = 80 * 60;
        public const int ImageSize40x30 = 40 * 30;
        public const int ImageSize20x15 = 20 * 15;

        public const int QvgaWorkBufferSize = 152 * 1024;
        public const int QqvgaWorkBufferSize = 40 * 1024;
        public const int QqqvgaWorkBufferSize = 12 * 1024;

        public const int WorkBufferSize320x240 = 152 * 1024;
        public const int WorkBufferSize160x120 = 40 * 1024;
        public const int WorkBufferSize80x60 = 12 * 1024;
        public const int WorkBufferSize40x30 = 4 * 1024;
        public const int WorkBufferSize20x15 = 4 * 1024;

        public const long ExposureTimeMinNanoSeconds = 7 * 1000;
        public const long ExposureTimeMaxNanoSeconds = 600 * 1000;

        public static int GetWorkBufferSize(ImageTransferProcessorFormat format)
        {
            switch (format)
            {
                case ImageTransferProcessorFormat.Format320x240:
                    return WorkBufferSize320x240;
                case ImageTransferProcessorFormat.Format160x120:
                    return WorkBufferSize160x120;
                case ImageTransferProcessorFormat.Format80x60:
                    return WorkBufferSize80x60;
                case ImageTransferProcessorFormat.Format40x30:
                    return WorkBufferSize40x30;
                case ImageTransferProcessorFormat.Format20x15:
                    return WorkBufferSize20x15;
            }
            return WorkBufferSize320x240;
        }

        public static int GetImageSize(ImageTransferProcessorFormat format)
        {
            switch (format)
            {
                case ImageTransferProcessorFormat.Format320x240:
                    return ImageSize320x240;
                case ImageTransferProcessorFormat.Format160x120:
                    return ImageSize160x120;
                case ImageTransferProcessorFormat.Format80x60:
                    return ImageSize80x60;
                case ImageTransferProcessorFormat.Format40x30:
                    return ImageSize40x30;
                case ImageTransferProcessorFormat.Format20x15:
                    return ImageSize20x15;
            }
            return ImageSize320x240;
        }
        public static int GetImageWidth(ImageTransferProcessorFormat format)
        {
            switch (format)
            {
                case ImageTransferProcessorFormat.Format320x240:
                    return 320;
                case ImageTransferProcessorFormat.Format160x120:
                    return 160;
                case ImageTransferProcessorFormat.Format80x60:
                    return 80;
                case ImageTransferProcessorFormat.Format40x30:
                    return 40;
                case ImageTransferProcessorFormat.Format20x15:
                    return 20;
            }
            return 320;
        }
        public static int GetImageHeight(ImageTransferProcessorFormat format)
        {
            switch (format)
            {
                case ImageTransferProcessorFormat.Format320x240:
                    return 240;
                case ImageTransferProcessorFormat.Format160x120:
                    return 120;
                case ImageTransferProcessorFormat.Format80x60:
                    return 60;
                case ImageTransferProcessorFormat.Format40x30:
                    return 30;
                case ImageTransferProcessorFormat.Format20x15:
                    return 15;
            }
            return 240;
        }
    }

    public enum ImageTransferProcessorFormat
    {
        Format320x240,
        Format160x120,
        Format80x60,
        Format40x30,
        Format20x15,
        Qvga = Format320x240,
        Qqvga = Format160x120,
        Qqqvga = Format80x60,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageTransferProcessorConfig
    {
        public IrCameraConfig irCameraConfig;
        public ImageTransferProcessorFormat format;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageTransferProcessorExConfig
    {
        public IrCameraConfig irCameraConfig;
        public ImageTransferProcessorFormat origFormat;
        public ImageTransferProcessorFormat trimmingFormat;
        public short trimmingStartX;
        public short trimmingStartY;
        [MarshalAs(UnmanagedType.U1)]
        public bool isExternalLightFilterEnabled;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ImageTransferProcessorState
    {
        public long samplingNumber;
        public IrCameraAmbientNoiseLevel ambientNoiseLevel;
        private byte _reserved0;
        private byte _reserved1;
        private byte _reserved2;
        private byte _reserved3;
    }
}
#endif
