#region License

// Copyright 2010 Morten Mertner, Buu Nguyen (http://www.buunguyen.net/blog)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Fasterflect
{
	/// <summary>
	/// Extension methods for deep cloning of objects.
	/// </summary>
	public static class CloneExtensions
	{
		#region Clone Overloads
		/// <summary>
		/// This method will produce a deep clone of the <paramref name="source"/> object.
		/// Reference integrity is maintained and every unique object in the graph is
		/// cloned only once.
		/// Note that all objects in the graph must have a default constructor.
		/// </summary>
		/// <typeparam name="T">The type of the object to clone.</typeparam>
		/// <param name="source">The object to clone.</param>
		/// <returns>A deep clone of the source object.</returns>
		public static T DeepClone<T>( this T source ) where T : class, new()
		{
			return source.DeepClone(null);
		}

		private static T DeepClone<T>( this T source, Dictionary<object, object> map ) where T : class, new()
		{
			Type type = typeof(T);
			IList<FieldInfo> fields = type.FieldsIncludingBaseTypes();
			var clone = type.CreateInstance() as T;
			map = map ?? new Dictionary<object, object> { { source, clone } };
			object[] values = fields.Select( f => GetValue( f, source, map ) ).ToArray();
			for( int i = 0; i < fields.Count; i++ )
			{
				fields[ i ].SetValue( clone, values[ i ] );
			}
			return clone;
		}

		private static object GetValue( FieldInfo field, object source, Dictionary<object, object> map )
		{
			object result = field.GetValue(source);
			object clone;
			if( map.TryGetValue(result, out clone) )
			{
				return clone;
			}
			bool follow = result != null && result.GetType().IsClass && result.GetType() != typeof (string);
			return follow ? result.DeepClone( map ) : result;
		}
		#endregion
	}
}