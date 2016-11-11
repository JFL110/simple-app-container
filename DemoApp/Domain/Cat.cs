using System;

namespace DemoApp
{
	public class Cat
	{
		public Cat (){}

		public int Id {get;set;}
		public string Name {get;set;}

		public override string ToString ()
		{
			return string.Format ("[Cat: Id={0}, Name={1}]", Id, Name);
		}
	}
}