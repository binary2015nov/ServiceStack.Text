using System;

namespace ServiceStack.Common.Support
{
	internal class AssemblyTypeDefinition
	{
		private const char TypeDefinitionSeperator = ',';
		private const int TypeNameIndex = 0;
		private const int AssemblyNameIndex = 1;

		public AssemblyTypeDefinition(string typeDefinition)
		{
			if (typeDefinition.IsNullOrEmpty())		
				throw new ArgumentException("The value cannot be null or empty.", nameof(typeDefinition));
			
			var parts = typeDefinition.Split(TypeDefinitionSeperator);
			TypeName = parts[TypeNameIndex].Trim();
			AssemblyName = (parts.Length > AssemblyNameIndex) ? parts[AssemblyNameIndex].Trim() : null;
		}

		public string TypeName { get; set; }

		public string AssemblyName { get; set; }
	}
}