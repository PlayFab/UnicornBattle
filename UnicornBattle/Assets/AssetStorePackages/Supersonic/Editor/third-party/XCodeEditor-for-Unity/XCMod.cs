using UnityEngine;
using SupersonicJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Supersonic.UnityEditor.XCodeEditor 
{
	public class XCMod 
	{
		private Hashtable _datastore;
		private List<object> _libs;
		
		public string name { get; private set; }
		public string path { get; private set; }
		
		public string group {
			get {
				return (string)_datastore["group"];
			}
		}
		
		public List<object> patches
		{
			get {
				return (List<object>)_datastore["patches"];
			}
		}
		
		public List<object> libs {
			get {
				if( _libs == null ) {
					List<object> libsCast = (List<object>)_datastore["libs"];
					int count = libsCast.Count;
					
					_libs = new List<object>( count );
					foreach( string fileRef in libsCast ) {
						_libs.Add( new XCModFile( fileRef ) );
					}
				}
				return _libs;
			}
		}
		
		public List<object> librarysearchpaths {
			get {
				return (List<object>)_datastore["librarysearchpaths"];
			}
		}
		
		public List<object> frameworks {
			get {
				return (List<object>)_datastore["frameworks"];
			}
		}
		
		public List<object> frameworksearchpath {
			get {
				return (List<object>)_datastore["frameworksearchpaths"];
			}
		}
		
		public List<object> headerpaths {
			get {
				return (List<object>)_datastore["headerpaths"];
			}
		}
		
		public List<object> files {
			get {
				return (List<object>)_datastore["files"];
			}
		}
		
		public List<object> folders {
			get {
				return (List<object>)_datastore["folders"];
			}
		}
		
		public List<string> excludes {
			get {
				return ((List<object>)_datastore["excludes"]).ConvertAll((obj)=> (string)obj);
			}
		}

		public List<string> plist {
			get {
				return ((List<object>)_datastore["plist"]).ConvertAll((obj)=> (string)obj);
			}
		}

		public List<string> linkers {
			get {
				return ((List<object>)_datastore["linkers"]).ConvertAll((obj)=> (string)obj);
			}
		}

		public List<string> config {
			get {
				return ((List<object>)_datastore["config"]).ConvertAll((obj)=> (string)obj);
			}
		}

		public XCMod( string projectPath, string filename )
		{	
			FileInfo projectFileInfo = new FileInfo( filename );
			if( !projectFileInfo.Exists ) {
				Debug.LogWarning( "File does not exist." );
			}
			
			name = System.IO.Path.GetFileNameWithoutExtension( filename );
			path = projectPath;//System.IO.Path.GetDirectoryName( filename );
			
			string contents = projectFileInfo.OpenText().ReadToEnd();
			Dictionary<string, object> dictJson = Json.Deserialize(contents) as Dictionary<string,object>;
			_datastore = new Hashtable(dictJson);

			//append file patterns that should always be ignored
			List<object> excludes = (List<object>)_datastore ["excludes"];
			if (excludes != null) {
				excludes.AddRange (new List<object> (){"^.*\\.meta$", "^.*\\.mdown^", "^.*\\.pdf$", ".DS_Store", "^.*\\.suGlobalConfig", "^.*\\.suAdapterConfig"});
				_datastore ["excludes"] = excludes;
			}
		}
	}
	
	public class XCModFile
	{
		public string filePath { get; private set; }
		public bool isWeak { get; private set; }
		public string sourceTree {get; private set;}
		
		public XCModFile( string inputString )
		{
			isWeak = false;
			sourceTree = "SDKROOT";
			if( inputString.Contains( ":" ) ) {
				string[] parts = inputString.Split( ':' );
				filePath = parts[0];
				isWeak = System.Array.IndexOf(parts, "weak", 1) > 0;
				
				if(System.Array.IndexOf(parts, "<group>", 1) > 0)
					sourceTree = "GROUP";
				else
					sourceTree = "SDKROOT";
				
			}
			else {
				filePath = inputString;
			}
		}
	}
}