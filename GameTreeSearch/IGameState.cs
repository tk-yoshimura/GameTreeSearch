using System.Collections.Generic;

namespace GameTreeSearch {

    /// <summary>ゲーム状態インターフェイス</summary>
    /// <typeparam name="DecisionType">棋譜クラス</typeparam>
    public interface IState<DecisionType> where DecisionType : new() {
        /// <summary>評価値</summary>
        double Evaluation { get; }

        /// <summary>ゲームセットか否か</summary>
        bool IsEndGame { get; }

        /// <summary>次の取り得る棋譜をコレクションとして返す</summary>
        IEnumerable<DecisionType> NextDecisions();

        /// <summary>次のゲーム状態を生成する</summary>
        /// <param name="decision">棋譜</param>
        IState<DecisionType> NextState(DecisionType decision);
    }
}
