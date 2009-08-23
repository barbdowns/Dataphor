/*
	Dataphor
	© Copyright 2000-2008 Alphora
	This file is licensed under a modified BSD-license which can be found here: http://dataphor.org/dataphor_license.txt
*/

using System;
using System.Collections;
using System.Collections.Specialized;

namespace Alphora.Dataphor.DAE.Runtime
{
	public class Stack : Stack<object>
	{
		public Stack() : base() { }
		public Stack(int AMaxStackDepth, int AMaxCallDepth) : base(AMaxStackDepth, AMaxCallDepth) { }
		
		protected object FErrorVar;
		public object ErrorVar
		{
			get { return FErrorVar; }
			set { FErrorVar = value; }
		}
	}
}
