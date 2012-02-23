using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerProbability
{
    /// <summary>
    /// スートを表します。
    /// </summary>
    enum Suit
    {
        /// <summary>
        /// スペード
        /// </summary>
        Spade,
        /// <summary>
        /// クローバー
        /// </summary>
        Clover,
        /// <summary>
        /// ハート
        /// </summary>
        Heart,
        /// <summary>
        /// ダイヤ
        /// </summary>
        Dia
    }

    /// <summary>
    /// カードを表します。
    /// </summary>
    struct Card
    {
        Suit _suit;
        /// <summary>
        /// このカードのスートを取得・設定します。
        /// </summary>
        public Suit Suit
        {
            get { return _suit; }
            set { _suit = value; }
        }

        int _number;
        /// <summary>
        /// このカードの番号を取得・設定します。
        /// </summary>
        public int Number
        {
            get { return _number; }
            set
            {
                if (1 <= value && value <= 13)
                    _number = value;
            }
        }

        /// <summary>
        /// スートと番号を指定してカードを生成します。
        /// </summary>
        /// <param name="suit">スート</param>
        /// <param name="number">番号</param>
        public Card(Suit suit, int number)
        {
            if (number < 1 || 13 < number)
                throw new ArgumentOutOfRangeException("number", number, "カードの番号は1から13の範囲で指定する必要があります。");

            _suit = suit;
            _number = number;
        }

        /// <summary>
        /// このカードの文字列形式を返します。
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}{1}", Suit.ToString()[0], Number);
        }
    }
}
