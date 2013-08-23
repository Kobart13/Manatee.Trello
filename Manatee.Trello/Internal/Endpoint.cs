﻿/***************************************************************************************

	Copyright 2013 Little Crab Solutions

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		Endpoint.cs
	Namespace:		Manatee.Trello.Internal
	Class Name:		Endpoint
	Purpose:		Represents a series of URL segments which together represent
					a REST method in Trello's API.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Trello.Internal
{
	internal class Endpoint
	{
		private readonly List<string> _segments;

		public Endpoint(IEnumerable<string> segments)
		{
			_segments = segments.ToList();
		}
		public Endpoint(params string[] segments)
		{
			_segments = segments.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
		}

		public void Append(string segment)
		{
			_segments.Add(segment);
		}

		public override string ToString()
		{
			return string.Join("/", _segments);
		}
	}
}