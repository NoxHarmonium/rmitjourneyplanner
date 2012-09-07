using System;

namespace JRPCServer
{
	public static class Strings
	{
		public const string ERR_PROP_NOT_FOUND = "The property you are trying to modify can not be found.";
		public const string ERR_INVALID_CAST = "The property value you supplied cannot be cast to the property type.";
		public const string ERR_UNSUPP_REFTYPE = "The property is a reference type that the parser cannot handle.";
		public const string VALIDATION_SUCCESS = "Success";
	    public const string ERR_JM_NULL =
	        "The JourneyManager parameter can not be null. It is required for this class to operate.";

	    public const string ERR_JM_NOTFOUND = "There is no JourneyManager object registered with the ObjectCache.";

	    public const string ERR_J_NULL = "The journey UUID parameter cannot be null.";

	    public const string ERR_ANY_NULL = "There can be no null parameters passed to this function.";

	    public const string ERR_INVALID_RUNID = "The specified run uuid is invalid for this journey.";
	}
}

