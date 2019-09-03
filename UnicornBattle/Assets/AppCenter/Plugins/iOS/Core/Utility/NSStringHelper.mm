// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#import "NSStringHelper.h"
#import <Foundation/Foundation.h>

const char* appcenter_unity_ns_string_to_cstr(NSString* nsstring)
{
  if (!nsstring)
  {
    return NULL;
  }

  // It seems that with (at least) IL2CPP, when returning a char* that is to be
  // converted to a System.String in C#, the char array is freed - which causes
  // a double-deallocation if ARC also tries to free it. To prevent this, we
  // must return a manually allocated copy of the string returned by "UTF8String"
  size_t cstringLength = [nsstring length] + 1; // +1 for '\0'
  const char *cstring = [nsstring UTF8String];
  char *cstring_copy = (char*)malloc(cstringLength);
  strncpy(cstring_copy, cstring, cstringLength);
  return cstring_copy;
}

NSString* appcenter_unity_cstr_to_ns_string(const char* str)
{
  return str ? [NSString stringWithUTF8String:str] : nil;
}
