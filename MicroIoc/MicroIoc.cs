using System;
using System.Collections.Generic;
using System.Collections;

namespace MicroIoc {
    class ContainerTypeExistException : System.Exception  {

    }

    class ContainerBadEntryException : System.Exception {

    }

    public delegate TResult Func<TResult>();

	class IocContainer : IEnumerable<KeyValuePair<Type, Tuple<object, Delegate>>>
    {
		private static IocContainer g_Instance;
		public static IocContainer Instance {
			get {
				if (g_Instance == null) {
					g_Instance = new IocContainer();
				}
				return g_Instance;
			}
		}

		bool m_commit = false;
		object m_lock = new object();
        Dictionary<Type, Tuple<object, Delegate>> m_IocTable =
            new Dictionary<Type, Tuple<object, Delegate>>();

		public IocContainer() {
		}

		public IocContainer(params IocContainer[] others) {
			foreach (var it in others) {
				this.Import(it);
			}
		}

        public IocContainer Register<T>(T input) where T : class {
			lock (m_lock) {
				if (m_commit) {
					goto ret;
				}
				if (!m_IocTable.ContainsKey(typeof(T))) {
					m_IocTable[typeof(T)] = Tuple.Create((object)input, (Delegate)null);
				}
				else {
					throw new ContainerTypeExistException();
				}
			}
		ret:
            return this;
        }

        public IocContainer Register<T>(Func<T> input) where T : class {
			lock (m_lock) {
				if (m_commit) {
					goto ret;
				}
				if (!m_IocTable.ContainsKey(typeof(T))) {
					m_IocTable[typeof(T)] = Tuple.Create((object)null, (Delegate)input);
				}
				else {
					throw new ContainerTypeExistException();
				}
			}
		ret:
            return this;
        }

		public IocContainer Register<T>() where T : class {
			return Register<T>(Activator.CreateInstance<T>());
		}

        public T Get<T>() where T : class {
			Tuple<object, Delegate> tuple;
			lock (m_lock) {
				tuple = m_IocTable[typeof(T)];
			}
            if (tuple == null) {
                return default(T);
            }
            if (tuple.Item1 != null) {
                return (T)tuple.Item1;
            }
            if (tuple.Item2 != null) {
                return ((Func<T>)tuple.Item2)();
            }
            throw new ContainerBadEntryException();
        }

		public IocContainer Purge<T>() where T : class {
			lock (m_lock) {
				if (m_commit) {
					goto circular;
				}
				m_IocTable.Remove(typeof(T));
			}
		circular:
			return this;
		}

		public IocContainer Purge() {
			lock (m_lock) {
				if (m_commit) {
					goto circular;
				}
				m_IocTable.Clear();
			}
		circular:
			return this;
		}

		public IocContainer Lockdown() {
			m_commit = true;
			return this;
		}

		public IEnumerator<KeyValuePair<Type, Tuple<object, Delegate>>> GetEnumerator() {
			lock (m_lock) {
				return m_IocTable.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public void Import(IocContainer other) {
			lock (m_lock) {
				foreach (var entry in other) {
					m_IocTable.Add(entry.Key, entry.Value);
				}
			}
		}
	}
}
