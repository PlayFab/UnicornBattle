#!/usr/bin/python

# Copyright 2016 Google Inc. All Rights Reserved.

"""Stand-alone implementation of the Gradle Firebase plugin.

Converts the services json file to xml:
https://googleplex-android.googlesource.com/platform/tools/base/+/studio-master-dev/build-system/google-services/src/main/groovy/com/google/gms/googleservices
"""

__author__ = 'Wouter van Oortmerssen'

import argparse
import json
import os
import sys
from xml.etree import ElementTree

# Input filename if it isn't set.
DEFAULT_INPUT_FILENAME = 'app/google-services.json'
# Output filename if it isn't set.
DEFAULT_OUTPUT_FILENAME = 'res/values/googleservices.xml'

# Indicates a web client in the oauth_client list.
OAUTH_CLIENT_TYPE_WEB = 3

def gen_string(parent, name, text):
  """Generate one <string /> element."""
  if text:
    child = ElementTree.SubElement(parent, 'string', {
        'name': name,
        'translatable': 'false'
    })
    child.text = text


def indent(elem, level=0):
  """Recurse through XML tree and add intentation."""
  i = '\n' + level*'  '
  if elem is not None:
    if not elem.text or not elem.text.strip():
      elem.text = i + '  '
    if not elem.tail or not elem.tail.strip():
      elem.tail = i
    for elem in elem:
      indent(elem, level+1)
    if not elem.tail or not elem.tail.strip():
      elem.tail = i
  else:
    if level and (not elem.tail or not elem.tail.strip()):
      elem.tail = i


def main():
  parser = argparse.ArgumentParser(description=(
      ('Converts a Firebase %s into %s similar to the Gradle plugin.' %
       (DEFAULT_INPUT_FILENAME, DEFAULT_OUTPUT_FILENAME))))
  parser.add_argument('-i', help='Override input file name',
                      metavar='FILE', required=False)
  parser.add_argument('-o', help='Override destination file name',
                      metavar='FILE', required=False)
  parser.add_argument('-p', help=('Package ID to select within the set of '
                                  'packages in the input file.  If this is '
                                  'not specified, the first package in the '
                                  'input file is selected.'))
  parser.add_argument('-l', help=('List all package IDs referenced by the '
                                  'input file.  If this is specified, '
                                  'the output file is not created.'),
                      action='store_true', default=False, required=False)
  parser.add_argument('-f', help=('Print project fields from the input file '
                                  'in the form \'name=value\\n\' for each '
                                  'field.  If this is specified, the output '
                                  'is not created.'),
                      action='store_true', default=False, required=False)
  args = parser.parse_args()

  if args.i:
    input_filename = args.i
  else:
    input_filename = DEFAULT_INPUT_FILENAME

  with open(input_filename, 'r') as ifile:
    json_string = ifile.read()

  jsobj = json.loads(json_string)

  root = ElementTree.Element('resources')

  project_info = jsobj.get('project_info')
  if project_info:
    gen_string(root, 'firebase_database_url', project_info.get('firebase_url'))
    gen_string(root, 'gcm_defaultSenderId', project_info.get('project_number'))
    gen_string(root, 'google_storage_bucket',
               project_info.get('storage_bucket'))
    gen_string(root, 'project_id', project_info.get('project_id'))

  if args.f:
    if not project_info:
      sys.stderr.write('No project info found in %s.' % input_filename)
      return 1
    for field, value in project_info.iteritems():
      sys.stdout.write('%s=%s\n' % (field, value))
    return 0

  packages = set()
  client_list = jsobj.get('client')
  if client_list:
    # Search for the user specified package in the file.
    selected_package_name = ''
    selected_client = client_list[0]
    find_package_name = args.p
    for client in client_list:
      package_name = client.get('client_info', {}).get(
          'android_client_info', {}).get('package_name', '')
      if not package_name:
        package_name = client.get('oauth_client', {}).get(
            'android_info', {}).get('package_name', '')
      if package_name:
        if not selected_package_name:
          selected_package_name = package_name
          selected_client = client
        if package_name == find_package_name:
          selected_package_name = package_name
          selected_client = client
        packages.add(package_name)

    if args.p and selected_package_name != find_package_name:
      sys.stderr.write('No packages found in %s which match the package '
                       'name %s\n'
                       '\n'
                       'Found the following:\n'
                       '%s\n' % (input_filename, find_package_name,
                                 '\n'.join(packages)))
      return 1

    client_api_key = selected_client.get('api_key')
    if client_api_key:
      client_api_key0 = client_api_key[0]
      gen_string(root, 'google_api_key', client_api_key0.get('current_key'))
      gen_string(root, 'google_crash_reporting_api_key',
                 client_api_key0.get('current_key'))

    client_info = selected_client.get('client_info')
    if client_info:
      gen_string(root, 'google_app_id', client_info.get('mobilesdk_app_id'))

    oauth_client_list = selected_client.get('oauth_client')
    if oauth_client_list:
      for oauth_client in oauth_client_list:
        client_type = oauth_client.get('client_type')
        client_id = oauth_client.get('client_id')
        if client_type and client_type == OAUTH_CLIENT_TYPE_WEB and client_id:
          gen_string(root, 'default_web_client_id', client_id)

    services = selected_client.get('services')
    if services:
      ads_service = services.get('ads_service')
      if ads_service:
        gen_string(root, 'test_banner_ad_unit_id',
                   ads_service.get('test_banner_ad_unit_id'))
        gen_string(root, 'test_interstitial_ad_unit_id',
                   ads_service.get('test_interstitial_ad_unit_id'))
      analytics_service = services.get('analytics_service')
      if analytics_service:
        analytics_property = analytics_service.get('analytics_property')
        if analytics_property:
          gen_string(root, 'ga_trackingId',
                     analytics_property.get('tracking_id'))
      # enable this once we have an example if this service being present
      # in the json data:
      maps_service_enabled = False
      if maps_service_enabled:
        maps_service = services.get('maps_service')
        if maps_service:
          maps_api_key = maps_service.get('api_key')
          if maps_api_key:
            for k in range(0, len(maps_api_key)):
              # generates potentially multiple of these keys, which is
              # the same behavior as the java plugin.
              gen_string(root, 'google_maps_key',
                         maps_api_key[k].get('maps_api_key'))

  tree = ElementTree.ElementTree(root)

  indent(root)

  if args.l:
    for package in packages:
      if package:
        sys.stdout.write(package + '\n')
  else:
    if args.o:
      output_filename = args.o
    else:
      output_filename = DEFAULT_OUTPUT_FILENAME

    path = os.path.dirname(output_filename)

    if path and not os.path.exists(path):
      os.makedirs(path)

    tree.write(output_filename, 'utf-8', True)

  return 0

if __name__ == '__main__':
  sys.exit(main())
