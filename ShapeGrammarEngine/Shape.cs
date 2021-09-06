﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShapeGrammarEngine
{
	public readonly struct Shape: IEquatable<Shape>
	{
		public readonly HashSet<(int, int)> Definition;

		public Shape(HashSet<(int, int)> definition)
		{
			this.Definition = definition;
		}

		public static bool operator ==(Shape lhs, Shape rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Shape lhs, Shape rhs) => !(lhs == rhs);

		public override bool Equals(object obj) => obj is Shape other && this.Equals(other);
		bool IEquatable<Shape>.Equals(Shape other) => this.Equals(other);

		private bool Equals(Shape s)
		{
			if (this.Definition.Count != s.Definition.Count)
			{
				return false;
			}

			foreach ((int, int) connection in this.Definition)
			{
				if (! (s.Definition.Contains(connection) || s.Definition.Contains((connection.Item2, connection.Item1))))
				{
					return false;
				}
			}

			return true;
		}
	}
}
