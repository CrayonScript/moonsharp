﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace Tutorials.Chapters
{
	[Tutorial]
	static class Chapter06
	{
		#region UserData classes

		[MoonSharpUserData]
		class MyClass
		{
			public double calcHypotenuse(double a, double b)
			{
				return Math.Sqrt(a * a + b * b);
			}

			public string ManipulateString(string input, ref string tobeconcat, out string lowercase)
			{
				tobeconcat = input + tobeconcat;
				lowercase = input.ToLower();
				return input.ToUpper();
			}
		}

		[MoonSharpUserData]
		class MyClassStatic
		{
			public static double calcHypotenuse(double a, double b)
			{
				return Math.Sqrt(a * a + b * b);
			}
		}

		class IndexerTestClass
		{
			Dictionary<int, int> mymap = new Dictionary<int, int>();

			public int this[int idx]
			{
				get { return mymap[idx]; }
				set { mymap[idx] = value; }
			}

			public int this[int idx1, int idx2, int idx3]
			{
				get { int idx = (idx1 + idx2) * idx3; return mymap[idx]; }
				set { int idx = (idx1 + idx2) * idx3; mymap[idx] = value; }
			}
		}

		class ArithmOperatorsTestClass : IComparable, System.Collections.IEnumerable
		{
			public int Value { get; set; }

			public ArithmOperatorsTestClass()
			{
			}

			public ArithmOperatorsTestClass(int value)
			{
				Value = value;
			}

			public int Length { get { return 117; } }

			[MoonSharpUserDataMetamethod("__concat")]
			public static int Concat(ArithmOperatorsTestClass o, int v)
			{
				return o.Value + v;
			}

			[MoonSharpUserDataMetamethod("__concat")]
			public static int Concat(int v, ArithmOperatorsTestClass o)
			{
				return o.Value + v;
			}

			[MoonSharpUserDataMetamethod("__concat")]
			public static int Concat(ArithmOperatorsTestClass o1, ArithmOperatorsTestClass o2)
			{
				return o1.Value + o2.Value;
			}

			public static int operator +(ArithmOperatorsTestClass o, int v)
			{
				return o.Value + v;
			}

			public static int operator +(int v, ArithmOperatorsTestClass o)
			{
				return o.Value + v;
			}

			public static int operator +(ArithmOperatorsTestClass o1, ArithmOperatorsTestClass o2)
			{
				return o1.Value + o2.Value;
			}

			public override bool Equals(object obj)
			{
				if (obj is double)
					return ((double)obj) == Value;

				ArithmOperatorsTestClass other = obj as ArithmOperatorsTestClass;
				if (other == null) return false;
				return Value == other.Value;
			}

			public override int GetHashCode()
			{
				return Value.GetHashCode();
			}

			public int CompareTo(object obj)
			{
				if (obj is double)
					return Value.CompareTo((int)(double)obj);

				ArithmOperatorsTestClass other = obj as ArithmOperatorsTestClass;
				if (other == null) return 1;
				return Value.CompareTo(other.Value);
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return (new List<int>() { 1, 2, 3 }).GetEnumerator();
			}

			[MoonSharpUserDataMetamethod("__call")]
			public int DefaultMethod()
			{
				return -Value;
			}

			[MoonSharpUserDataMetamethod("__pairs")]
			[MoonSharpUserDataMetamethod("__ipairs")]
			public System.Collections.IEnumerator Pairs()
			{
				return (new List<DynValue>() { 
					DynValue.NewTuple(DynValue.NewString("a"), DynValue.NewString("A")),
					DynValue.NewTuple(DynValue.NewString("b"), DynValue.NewString("B")),
					DynValue.NewTuple(DynValue.NewString("c"), DynValue.NewString("C")) }).GetEnumerator();
			}


		}


		#endregion


		[Tutorial]
		public static double CallMyClass1()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Automatically register all MoonSharpUserData types
			UserData.RegisterAssembly();

			Script script = new Script();

			// Pass an instance of MyClass to the script in a global
			script.Globals["obj"] = new MyClass();

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}

		[Tutorial]
		static double CallMyClass2()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Register just MyClass, explicitely.
			UserData.RegisterType<MyClass>();

			Script script = new Script();

			// create a userdata, again, explicitely.
			DynValue obj = UserData.Create(new MyClass());

			script.Globals.Set("obj", obj);

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}


		[Tutorial]
		static double MyClassStaticThroughInstance()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Automatically register all MoonSharpUserData types
			UserData.RegisterAssembly();

			Script script = new Script();

			script.Globals["obj"] = new MyClassStatic();

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}

		[Tutorial]
		static double MyClassStaticThroughPlaceholder()
		{
			string scriptCode = @"    
				return obj.calcHypotenuse(3, 4);
			";

			// Automatically register all MoonSharpUserData types
			UserData.RegisterAssembly();

			Script script = new Script();

			script.Globals["obj"] = typeof(MyClassStatic);

			DynValue res = script.DoString(scriptCode);

			return res.Number;
		}


		[Tutorial]
		static string ByRefParams()
		{
			string scriptCode = @"    
				x, y, z = myobj:manipulateString('CiAo', 'hello');
				return x, y, z
			";

			// Automatically register all MoonSharpUserData types
			UserData.RegisterAssembly();

			Script script = new Script();

			script.Globals["myobj"] = new MyClass();

			DynValue res = script.DoString(scriptCode);

			return string.Join(", ", res.Tuple.Select(v => v.ToPrintString()));
		}

		[Tutorial]
		static void IndexerTests()
		{
			string scriptCode = @"    
				-- sets the value of an indexer
				o[5] = 19; 		
				print(o[5]);

				-- use the value of an indexer
				x = 5 + o[5]; 	
				print(x);

				-- sets the value of an indexer using multiple indices (not standard Lua!)
				o[1,2,3] = 19; 		
				print(o[1,2,3]);

				-- use the value of an indexer using multiple indices (not standard Lua!)
				x = 5 + o[1,2,3]; 	
				print(x);
			";

			UserData.RegisterType<IndexerTestClass>();

			Script script = new Script();

			script.Globals["o"] = new IndexerTestClass();

			script.DoString(scriptCode);
		}

		[Tutorial]
		static void OperatorsAndMetaMethods_WithAttributes()
		{
			string scriptCode = @"    
				print( o .. 1 );
				print( 1 .. o );
				print( o .. o );
			";

			UserData.RegisterType<ArithmOperatorsTestClass>();
			Script script = new Script();
			script.Globals["o"] = new ArithmOperatorsTestClass(5);

			script.DoString(scriptCode);
		}

		[Tutorial]
		static void OperatorsAndMetaMethods_WithOperatorsOverloads()
		{
			string scriptCode = @"    
				print( o + 1 );
				print( 1 + o );
				print( o + o );
			";

			UserData.RegisterType<ArithmOperatorsTestClass>();
			Script script = new Script();
			script.Globals["o"] = new ArithmOperatorsTestClass(5);

			script.DoString(scriptCode);
		}

		[Tutorial]
		static void OperatorsAndMetaMethods_Comparisons()
		{
			string scriptCode = @"    
				print( 'o == 1 ?', o == 1 );
				print( '1 == o ?', 1 == o );
				print( 'o == 5 ?', o == 5 );
				print( 'o != 1 ?', o != 1 );
				print( 'o <  1 ?', o <  1 );
				print( 'o <= 1 ?', o <= 1 );
				print( 'o <  6 ?', o <  6 );
				print( 'o <= 6 ?', o <= 6 );
				print( 'o >  1 ?', o >  1 );
				print( 'o >= 1 ?', o >= 1 );
				print( 'o >  6 ?', o >  6 );
				print( 'o >= 6 ?', o >= 6 );
			";

			UserData.RegisterType<ArithmOperatorsTestClass>();
			Script script = new Script();
			script.Globals["o"] = new ArithmOperatorsTestClass(5);

			script.DoString(scriptCode);
		}

		[Tutorial]
		static void OperatorsAndMetaMethods_Length()
		{
			string scriptCode = @"    
				print( '#o + o ?', #o + o);
			";

			UserData.RegisterType<ArithmOperatorsTestClass>();
			Script script = new Script();
			script.Globals["o"] = new ArithmOperatorsTestClass(5);

			script.DoString(scriptCode);
		}

		[Tutorial]
		static void OperatorsAndMetaMethods_ForEach()
		{
			string scriptCode = @"    
				local sum = 0
				for i in o do
					sum = sum + i
				end

				print (sum);
			";

			UserData.RegisterType<ArithmOperatorsTestClass>();
			Script script = new Script();
			script.Globals["o"] = new ArithmOperatorsTestClass(5);

			script.DoString(scriptCode);
		}


	}
}
