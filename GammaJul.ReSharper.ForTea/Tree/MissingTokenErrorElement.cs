#region License
//    Copyright 2012 Julien Lebosquain
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
#endregion
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace GammaJul.ReSharper.ForTea.Tree {

	/// <summary>
	/// Implementation of <see cref="ErrorElement"/> for a missing block end tag.
	/// </summary>
	public class MissingTokenErrorElement : ErrorElement {
		private readonly MissingTokenType _missingTokenType;

		public MissingTokenType MissingTokenType {
			get { return _missingTokenType; }
		}

		private static string GetErrorMessage(MissingTokenType missingTokenType) {
			switch (missingTokenType) {
				case MissingTokenType.BlockEnd: return "Missing block end";
				case MissingTokenType.DirectiveName: return "Missing directive name";
				case MissingTokenType.AttributeName: return "Missing attribute name";
				case MissingTokenType.AttributeNameAndEqualSign: return "Missing attribute name and equal sign";
				case MissingTokenType.EqualSign: return "Missing equal sign";
				case MissingTokenType.AttributeValue: return "Missing attribute value";
				case MissingTokenType.EqualSignAndAttributeValue: return "Missing equal sign and attribute value";
				case MissingTokenType.Quote: return "Missing quote";
				default: return missingTokenType.ToString();
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MissingTokenErrorElement"/> class.
		/// </summary>
		public MissingTokenErrorElement(MissingTokenType missingTokenType)
			: base(GetErrorMessage(missingTokenType)) {
			_missingTokenType = missingTokenType;
		}
	}

}