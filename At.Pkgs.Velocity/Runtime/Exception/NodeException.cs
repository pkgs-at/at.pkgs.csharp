// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace At.Pkgs.Velocity.Runtime.Exception
{
	using System;
	using System.Runtime.Serialization;
	using At.Pkgs.Velocity.Runtime.Parser.Node;

	[Serializable]
	public class NodeException : Exception
	{
		public NodeException(String exceptionMessage, INode node)
			: base(string.Format("{0}: {1} [line {2},column {3}]", exceptionMessage, node.Literal, node.Line, node.Column))
		{
		}

		public NodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}