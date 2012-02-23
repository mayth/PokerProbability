using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections;

namespace PokerProbability
{
    /// <summary>
    /// ShuffleListの状態を表す列挙型。
    /// </summary>
    enum ShuffleListStatus
    {
        /// <summary>
        /// 未シャッフル
        /// </summary>
        NotShuffled,
        /// <summary>
        /// シャッフル済み
        /// </summary>
        Shuffled,
        /// <summary>
        /// 最後の要素まで読んだ
        /// </summary>
        InEnd
    }

    /// <summary>
    /// シャッフル配列
    /// </summary>
    /// <typeparam name="T">配列の型</typeparam>
    class ShuffleList<T> : IList<T>
    {
        // 非公開メンバ
        List<T> list;
        int currentIndex;

        // 公開メンバ
        ShuffleListStatus status;
        public ShuffleListStatus Status
        {
            get { return status; }
            private set { status = value; }
        }

        /// <summary>
        /// 空で、既定の初期量を備えた、ShuffleList&lt;T&gt;クラスの新しいインスタンスを初期化します。
        /// </summary>
        public ShuffleList()
        {
            list = new List<T>();
            currentIndex = 0;
            Status = ShuffleListStatus.NotShuffled;
        }

        /// <summary>
        /// 空で、指定の初期量を備えた、ShuffleList&lt;T&gt;クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="capacity">新しいリストに格納できる要素の数</param>
        public ShuffleList(int capacity)
        {
            list = new List<T>(capacity);
            currentIndex = 0;
            Status = ShuffleListStatus.NotShuffled;
        }

        /// <summary>
        /// 指定されたリストがコピーされた、ShuffleList&lt;T&gt;クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="collection">新しいリストに要素がコピーされたコレクション</param>
        public ShuffleList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
            currentIndex = 0;
            Status = ShuffleListStatus.NotShuffled;
        }

        /// <summary>
        /// 配列に項目を追加する。
        /// </summary>
        /// <param name="item">追加する値</param>
        /// <remarks>
        /// 配列が未シャッフルである場合はitemを配列の最後に追加する。配列がシャッフル済みである場合はランダムな位置に追加する。
        /// </remarks>
        public void Add(T item)
        {
            if (Status == ShuffleListStatus.NotShuffled)
            {
                list.Add(item);
            }
            else
            {
                Random rnd = new Random();
                list.Insert(rnd.Next(list.Count), item);
            }
        }

        /// <summary>
        /// 配列に項目を追加する。
        /// </summary>
        /// <param name="item">追加する値</param>
        /// <param name="random">配列がシャッフル済みであるとき、trueならitemをランダムな位置に追加、falseなら配列の最後に追加する。</param>
        /// <remarks>
        /// 配列が未シャッフルである場合、randomの値にかかわらず常に配列の最後にitemを追加する。
        /// </remarks>
        public void Add(T item, bool random)
        {
            if (random)
                this.Add(item); // ランダムな位置に追加する（挙動は説明の通り）。
            else
                list.Add(item); // 配列の最後に追加する。
        }

        /// <summary>
        /// 配列をシャッフルする。
        /// </summary>
        public void Shuffle()
        {
            Random rnd = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int r = i + rnd.Next(list.Count - i);
                T temp = list[r];
                list[r] = list[i];
                list[i] = temp;
            }
            currentIndex = 0;
            Status = ShuffleListStatus.Shuffled;
        }

        /// <summary>
        /// 次の項目を取得する。
        /// </summary>
        public T NextItem
        {
            get
            {
                // 配列の最後まで到達したとき
                if (currentIndex >= list.Count - 1)
                {
                    Status = ShuffleListStatus.InEnd;
                    currentIndex = 0;
                    return list[list.Count - 1];
                }
                return list[currentIndex++];
            }
        }

        #region IList<T> メンバ

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }

        #endregion

        #region ICollection<T> メンバ

        public void Clear()
        {
            list.Clear();
            currentIndex = 0;
            Status = ShuffleListStatus.NotShuffled;
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        #endregion

        #region IEnumerable<T> メンバ

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < list.Count; i++)
                yield return list[i];
        }

        #endregion

        #region IEnumerable メンバ

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < list.Count; i++)
                yield return list[i];
        }

        #endregion
    }
}
