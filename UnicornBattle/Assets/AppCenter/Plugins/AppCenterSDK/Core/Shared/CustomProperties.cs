// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using Microsoft.AppCenter.Unity.Internal;

namespace Microsoft.AppCenter.Unity
{
#if UNITY_IOS
    using RawType = System.IntPtr;
#elif UNITY_ANDROID
    using RawType = UnityEngine.AndroidJavaObject;
#else
    using RawType = System.Object;
#endif

    public class CustomProperties
    {
        private readonly RawType _rawObject;

        internal RawType GetRawObject()
        {
            return _rawObject;
        }

        public CustomProperties()
        {
            _rawObject = CustomPropertiesInternal.Create();
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, string val)
        {
            CustomPropertiesInternal.SetString(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, DateTime val)
        {
            CustomPropertiesInternal.SetDate(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, int val)
        {
            CustomPropertiesInternal.SetNumber(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, long val)
        {
            CustomPropertiesInternal.SetNumber(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, float val)
        {
            CustomPropertiesInternal.SetNumber(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, double val)
        {
            CustomPropertiesInternal.SetNumber(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Set the specified property value with the specified key.
        /// If the properties previously contained a property for the key, the old value is replaced.
        /// </summary>
        /// <param name="key">Key with which the specified value is to be set.</param>
        /// <param name="val">Value to be set with the specified key.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Set(string key, bool val)
        {
            CustomPropertiesInternal.SetBool(_rawObject, key, val);
            return this;
        }

        /// <summary>
        /// Clear the property for the specified key.
        /// </summary>
        /// <param name="key">Key whose mapping is to be cleared.</param>
        /// <returns>This instance.</returns>
        public CustomProperties Clear(string key)
        {
            CustomPropertiesInternal.Clear(_rawObject, key);
            return this;
        }
    }
}
