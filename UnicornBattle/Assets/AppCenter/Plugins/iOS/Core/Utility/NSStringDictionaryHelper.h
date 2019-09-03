// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#ifndef NS_STRING_DICTIONARY_HELPER_H
#define NS_STRING_DICTIONARY_HELPER_H

#import <Foundation/Foundation.h>

extern "C" const char* appcenter_unity_ns_string_dictionary_get_key_at_idx(NSDictionary *dictionary, int idx);

extern "C" const char* appcenter_unity_ns_string_dictionary_get_value_for_key(NSDictionary *dictionary, char* key);

extern "C" size_t appcenter_unity_ns_dictionary_get_length(NSDictionary *dictionary);

extern "C" NSDictionary* appcenter_unity_create_ns_string_dictionary(char** keys, char** values, int count);

#endif
