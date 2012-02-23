using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerProbability
{
    enum Hand
    {
        /// <summary>
        /// ノーペア
        /// </summary>
        NoPair,
        /// <summary>
        /// ワンペア（同ランク2枚1組）
        /// </summary>
        OnePair,
        /// <summary>
        /// ツーペア（同ランク2枚2組）
        /// </summary>
        TwoPair,
        /// <summary>
        /// スリー・オブ・ア・カインド（同ランク3枚）
        /// </summary>
        ThreeOfAKind,
        /// <summary>
        /// フォー・オブ・ア・カインド（同ランク4枚）
        /// </summary>
        FourOfAKind,
        /// <summary>
        /// フルハウス（同ランク2枚1組と、同ランク3枚1組）
        /// </summary>
        Fullhouse,
        /// <summary>
        /// ストレート（ランク連続）（A-2-3、A-K-Qと続くが、K-A-2は含まない）
        /// </summary>
        Straight,
        /// <summary>
        /// フラッシュ（スート統一）
        /// </summary>
        Flash,
        /// <summary>
        /// ストレートフラッシュ（スート統一かつランク連続）
        /// </summary>
        StraightFlash,
        /// <summary>
        /// ロイヤルストレートフラッシュ（スート統一かつ10-J-Q-K-A）
        /// </summary>
        RoyalStraightFlash
    }
}
