using System.Collections.Generic;

namespace System.Collections.Generic
{
	interface IStructuralEquatable
	{
		bool Equals(Object that, IEqualityComparer comparer);
		int GetHashCode(IEqualityComparer comparer);
	}

	interface IStructuralComparable
	{
		int CompareTo(object that, IComparer comparer);
	}

	[Serializable]
	public class Tuple<T1, T2> : IStructuralEquatable, IStructuralComparable, IComparable, ITuple
	{
		public T1 Item1 { get; private set; }
		public T2 Item2 { get; private set; }
		int ITuple.Size { get { return 2; } }

		public Tuple(T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public override bool Equals(object that)
		{
			return ((IStructuralEquatable)this).Equals(that, EqualityComparer<object>.Default);
		}

		public override int GetHashCode()
		{
			return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<object>.Default);
		}

		public override string ToString()
		{
			return "(" + ((ITuple)this).ToString() + ")";
		}

		bool IStructuralEquatable.Equals(object that, IEqualityComparer comparer)
		{
			if (that == null)
			{
				return false;
			}
			var thatTuple = that as Tuple<T1, T2>;

			if (thatTuple == null)
			{
				return false;
			}
			return comparer.Equals(Item1, thatTuple.Item1)
			&& comparer.Equals(Item2, thatTuple.Item2);
		}

		int IComparable.CompareTo(object that)
		{
			return ((IStructuralComparable)this).CompareTo(that, Comparer<object>.Default);
		}

		int IStructuralComparable.CompareTo(object that, IComparer comparer)
		{
			if (that == null)
			{
				return 1;
			}
			var thatTuple = that as Tuple<T1, T2>;
			if (thatTuple == null)
			{
				return 1;
			}
			var c = 0;
			c = comparer.Compare(Item1, thatTuple.Item1);
			if (c != 0)
			{
				return c;
			}
			return comparer.Compare(Item2, thatTuple.Item2);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return Tuple.CombineHashCodes(
				comparer.GetHashCode(Item1),
				comparer.GetHashCode(Item2)
			);
		}

		string ITuple.ToString()
		{
			return string.Format("{0}, {1}", Item1, Item2);
		}
	}
}

namespace System
{
	public static class Tuple
	{
		internal static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}
		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
		{
			return new Tuple<T1, T2>(item1, item2);
		}
	}

	interface ITuple
	{
		int Size { get; }
		string ToString();
	}

	public delegate TResult Func<TResult>();
}