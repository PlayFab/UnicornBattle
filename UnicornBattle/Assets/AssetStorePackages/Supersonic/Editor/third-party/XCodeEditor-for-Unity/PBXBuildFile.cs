using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Supersonic.UnityEditor.XCodeEditor
{
	public class PBXBuildFile : PBXObject
	{
		private const string FILE_REF_KEY = "fileRef";
		private const string SETTINGS_KEY = "settings";
		private const string ATTRIBUTES_KEY = "ATTRIBUTES";
		private const string WEAK_VALUE = "Weak";
		private const string COMPILER_FLAGS_KEY = "COMPILER_FLAGS";

		public string name;

		public PBXBuildFile( PBXFileReference fileRef, bool weak = false ) : base()
		{
			string buildFileGuid = generateBuildFileGuid(fileRef);
			this.guid = buildFileGuid;
			this.Add( FILE_REF_KEY, fileRef.guid );
			SetWeakLink( weak );
			name = fileRef.name;
		}

		public PBXBuildFile( string guid, PBXDictionary dictionary ) : base ( guid, dictionary )
		{
			if(!this.data.ContainsKey(SETTINGS_KEY))
				return;
			object settingsObj = this.data[SETTINGS_KEY];

			if(!(settingsObj is PBXDictionary))
				return;
			PBXDictionary settingsDict = (PBXDictionary) settingsObj;
			settingsDict.internalNewlines = false;

			if( !settingsDict.ContainsKey(ATTRIBUTES_KEY) )
				return;
			object attributesObj = settingsDict[ATTRIBUTES_KEY];

			if(!(attributesObj is PBXList))
				return;

			PBXList attributesCast = (PBXList)attributesObj;
			attributesCast.internalNewlines = false;
		}

		private string generateBuildFileGuid(PBXFileReference fileRef) {
			string buildFileGuid = GenerateGuid();
            //todo generate with "from ..." section of comment
			buildFileGuid += " /* " + fileRef.name + " */";
			return buildFileGuid;
		}

		public string getFileRefGuid() {
			if (ContainsKey (FILE_REF_KEY)) {
				object obj = GetObjectForKey(FILE_REF_KEY);
				if(obj is string) {
					return (string)obj;
				}
			}


			return "";
		}

		public void SetWeakLink( bool weak)
		{
			PBXDictionary settings = null;
			PBXList attributes = null;

			if (_data.ContainsKey (SETTINGS_KEY)) {
				settings = _data[SETTINGS_KEY] as PBXDictionary;
				if (settings.ContainsKey(ATTRIBUTES_KEY)) {
					attributes = settings[ATTRIBUTES_KEY] as PBXList;
				}
			}

			if (weak) {
				if (settings == null) {
					settings = new PBXDictionary();
					settings.internalNewlines = false;
					_data.Add(SETTINGS_KEY, settings);
				}

				if (attributes == null) {
					attributes = new PBXList();
					attributes.internalNewlines = false;
					attributes.Add(WEAK_VALUE);
					settings.Add(ATTRIBUTES_KEY, attributes);
				}
			}
			else {
				if(attributes != null  && attributes.Contains(WEAK_VALUE)) {
					attributes.Remove(WEAK_VALUE);
				}
			}
		}

		public bool AddCompilerFlag( string flag )
		{
			if( !_data.ContainsKey( SETTINGS_KEY ) )
				_data[ SETTINGS_KEY ] = new PBXDictionary();

			if( !((PBXDictionary)_data[ SETTINGS_KEY ]).ContainsKey( COMPILER_FLAGS_KEY ) ) {
				((PBXDictionary)_data[ SETTINGS_KEY ]).Add( COMPILER_FLAGS_KEY, flag );
				return true;
			}

			string[] flags = ((string)((PBXDictionary)_data[ SETTINGS_KEY ])[ COMPILER_FLAGS_KEY ]).Split( ' ' );
			foreach( string item in flags ) {
				if( item.CompareTo( flag ) == 0 )
					return false;
			}

			((PBXDictionary)_data[ SETTINGS_KEY ])[ COMPILER_FLAGS_KEY ] = ( string.Join( " ", flags ) + " " + flag );
			return true;
		}

	}
}
