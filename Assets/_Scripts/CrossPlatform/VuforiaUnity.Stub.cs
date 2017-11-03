﻿#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
using System;

namespace VuforiaUnity
{
    public enum InitError {
        INIT_SUCCESS,
        INIT_EXTERNAL_DEVICE_NOT_DETECTED,
        INIT_LICENSE_ERROR_MISSING_KEY,
        INIT_LICENSE_ERROR_INVALID_KEY,
        INIT_LICENSE_ERROR_NO_NETWORK_TRANSIENT,
        INIT_LICENSE_ERROR_NO_NETWORK_PERMANENT,
        INIT_LICENSE_ERROR_CANCELED_KEY,
        INIT_LICENSE_ERROR_PRODUCT_TYPE_MISMATCH,
        INIT_DEVICE_NOT_SUPPORTED,
        INIT_ERROR
    }
}
#endif