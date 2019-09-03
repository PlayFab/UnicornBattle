// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;

namespace Microsoft.AppCenter.Unity.Crashes
{
    public class ErrorAttachmentLog
    {
        public string Text { get; private set; }
        public byte[] Data { get; private set; }
        public string FileName { get; private set; }
        public string ContentType { get; private set; }
        public AttachmentType Type { get; private set; }

        public static ErrorAttachmentLog AttachmentWithText(string text, string fileName)
        {
            return new ErrorAttachmentLog
            {
                Text = text,
                FileName = fileName,
                Type = AttachmentType.Text
            }; 
        }

        public static ErrorAttachmentLog AttachmentWithBinary(byte[] data, string fileName, string contentType)
        {
            return new ErrorAttachmentLog
            {
                Data = data,
                FileName = fileName,
                ContentType = contentType,
                Type = AttachmentType.Binary
            }; 
        }

        public enum AttachmentType { Text, Binary }
    }
}
