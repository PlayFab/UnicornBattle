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

namespace nn.irsensor
{
    public class ImageTransferProcessorManager
    {
        private ImageTransferProcessorState state;
        public ImageTransferProcessorState State { get { return state; } }
        public byte[] ImageBuffer { get; private set; }

        private IntPtr pWorkBuffer = IntPtr.Zero;
        private long workBufferSize;
        private ImageTransferProcessorExConfig config;
        private IrCameraHandle handle;

        ~ImageTransferProcessorManager()
        {
            _Destroy();
        }

        public void Initialize(IrCameraHandle handle, ImageTransferProcessorFormat format)
        {
            ImageTransferProcessorExConfig config = new ImageTransferProcessorExConfig();
            ImageTransferProcessor.GetDefaultConfig(ref config);
            Initialize(handle, config);
        }

        public void Initialize(IrCameraHandle handle, ImageTransferProcessorConfig config)
        {
            Initialize(handle, new ImageTransferProcessorExConfig()
            {
                origFormat = config.format,
                trimmingFormat = config.format,
                irCameraConfig = config.irCameraConfig
            });
        }

        public void Initialize(IrCameraHandle handle, ImageTransferProcessorExConfig config)
        {
            this.handle = handle;
            if (pWorkBuffer != IntPtr.Zero)
            {
                _Destroy();
            }
            this.config = config;
            ImageTransferProcessor.InitializeWorkBuffer(ref pWorkBuffer, ref workBufferSize, config);
            ImageBuffer = new byte[ImageTransferProcessor.GetImageSize(config.trimmingFormat)];
        }

        public void Destroy()
        {
            _Destroy();
            GC.SuppressFinalize(this);
        }

        public bool IsRunning()
        {
            return nn.irsensor.ImageProcessor.GetStatus(handle) == ImageProcessorStatus.Running;
        }

        public void Run()
        {
            ImageTransferProcessor.Run(handle, config, pWorkBuffer, workBufferSize);
        }

        public nn.Result Update()
        {
            long size = ImageTransferProcessor.GetImageSize(config.trimmingFormat);
            GCHandle imageBufHandle = GCHandle.Alloc(ImageBuffer, GCHandleType.Pinned);
            nn.Result result = ImageTransferProcessor.GetState(ref state, imageBufHandle.AddrOfPinnedObject(), size, handle);
            imageBufHandle.Free();
            return result;
        }

        public void Stop()
        {
            if (ImageProcessor.GetStatus(handle) == ImageProcessorStatus.Running)
            {
                ImageProcessor.Stop(handle);
            }
        }

        private void _Destroy()
        {
            Stop();
            ImageTransferProcessor.DestroyWorkBuffer(pWorkBuffer);
            pWorkBuffer = IntPtr.Zero;
            workBufferSize = 0L;
        }
    }
}
#endif
