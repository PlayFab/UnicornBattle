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
using System.Runtime.InteropServices;

namespace nn.irsensor
{
    public static partial class HandAnalysis
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_RunHandAnalysis")]
        public static extern nn.Result Run(IrCameraHandle handle, HandAnalysisConfig config);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetHandAnalysisSilhouetteState1")]
        public static extern nn.Result GetSilhouetteState(ref HandAnalysisSilhouetteState pOutValue, IrCameraHandle handle);

        public static nn.Result GetSilhouetteState(
            HandAnalysisSilhouetteState[] pOutValueArray, ref int pReturnCount,
            long infSamplingNumber, IrCameraHandle handle)
        {
            return GetSilhouetteState(pOutValueArray, ref pReturnCount, pOutValueArray.Length, infSamplingNumber, handle);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetHandAnalysisSilhouetteState")]
        private static extern nn.Result GetSilhouetteState(
            [In, Out] HandAnalysisSilhouetteState[] pOutValueArray, ref int pReturnCount,
            int maxCount, long infSamplingNumber, IrCameraHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetHandAnalysisSilhouetteStateAndPoints1")]
        public static extern nn.Result GetSilhouetteState(
            ref HandAnalysisSilhouetteState pOutState, [In, Out] nn.util.Float2[] pOutPointBuffer,
            IrCameraHandle handle);

        public static nn.Result GetSilhouetteState(
            HandAnalysisSilhouetteState[] pOutStateArray, nn.util.Float2[][] pOutPointArray, ref int pReturnCount,
            long infSamplingNumber, IrCameraHandle handle)
        {
            return GetSilhouetteState(pOutStateArray, pOutPointArray, ref pReturnCount, pOutStateArray.Length, infSamplingNumber, handle);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetHandAnalysisSilhouetteStateAndPoints")]
        private static extern nn.Result GetSilhouetteState(
            [In, Out] HandAnalysisSilhouetteState[] pOutStateArray, [In, Out] nn.util.Float2[][] pOutPointArray, ref int pReturnCount,
            int maxCount, long infSamplingNumber, IrCameraHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetHandAnalysisImageState1")]
        public static extern nn.Result GetImageState(
            ref HandAnalysisImageState pOutState, [In, Out] ushort[] pOutImageBuffer,
            IrCameraHandle handle);

        public static nn.Result GetImageState(
            HandAnalysisImageState[] pOutStateArray, ushort[] pOutImageArray, ref int pReturnCount,
            long infSamplingNumber, IrCameraHandle handle)
        {
            return GetImageState(pOutStateArray, pOutImageArray, ref pReturnCount, pOutStateArray.Length, infSamplingNumber, handle);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetHandAnalysisImageState")]
        private static extern nn.Result GetImageState(
            [In, Out] HandAnalysisImageState[] pOutStateArray, [In, Out] ushort[] pOutImageArray, ref int pReturnCount,
            int maxCount, long infSamplingNumber, IrCameraHandle handle);
    }
}
#endif
