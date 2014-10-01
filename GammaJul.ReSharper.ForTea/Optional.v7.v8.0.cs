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


using System;
using JetBrains.Annotations;

namespace GammaJul.ReSharper.ForTea {

	/// <summary>
	/// Mimics JetBrains.Application.Components.Optional which is only available in ReSharper >= 8.1.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Optional<T> {

		[CanBeNull]
		private readonly T _value;

		[CanBeNull]
		public T CanBeNull {
			get { return _value; }
		}

		public bool IsNotNull {
			get { return !Equals(_value, default(T)); }
		}

		public bool IsNull {
			get { return Equals(_value, default(T)); }
		}

		[NotNull]
		public T NotNull {
			get {
				if (Equals(_value, default(T)))
					throw new NullReferenceException("The value of this optional object is actually Null.");
				// ReSharper disable once AssignNullToNotNullAttribute
				return _value;
			}
		}

		public Optional()
			: this(default(T)) {
		}

		public Optional([CanBeNull] T value) {
			_value = value;
		}

	}

}