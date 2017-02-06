
using System;
public class SupersonicError
{
	private string description;
	private int code;
		

	public int getErrorCode(){
		return code;
	}

	public string getDescription(){
		return description;
	}

	public int getCode(){
		return code;
	}

	public SupersonicError(int errCode, string errDescription){
		code = errCode;
		description = errDescription;
	}

	public override string ToString()
	{
		return code + " : " + description;
	}
}

