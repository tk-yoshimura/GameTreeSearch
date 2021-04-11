using System.Collections.Generic;

namespace GameTreeSearch {

    /// <summary>ゲーム木ノード</summary>
    /// <typeparam name="StateType">盤面状態クラス</typeparam>
    /// <typeparam name="DecisionType">棋譜クラス</typeparam>
    public class GameNode<StateType, DecisionType> where StateType : IState<DecisionType> where DecisionType : new() {
        private double evaluation;

        /// <summary>コンストラクタ</summary>
        /// <param name="state">盤面状態</param>
        /// <param name="decision">この盤面状態にした棋譜</param>
        /// <param name="parent_node">親ノード</param>
        public GameNode(StateType state, DecisionType decision, GameNode<StateType, DecisionType> parent_node) {
            this.evaluation = double.NaN;
            this.State = state;
            this.Decision = decision;
            this.ParentNode = parent_node;
            this.ChildNodeList = new List<GameNode<StateType, DecisionType>>();
        }

        /// <summary>子ノードをコレクションとして返す</summary>
        public IEnumerable<GameNode<StateType, DecisionType>> ChildNodes() {
            foreach(DecisionType decision in State.NextDecisions()) {
                GameNode<StateType, DecisionType> child_node = new((StateType)State.NextState(decision), decision, this);

                ChildNodeList.Add(child_node);

                yield return child_node;
            }
        }

        /// <summary>子ノードを展開</summary>
        public void ExpandChildNodes() {
            foreach(DecisionType decision in State.NextDecisions()) {
                GameNode<StateType, DecisionType> child_node = new((StateType)State.NextState(decision), decision, this);
                ChildNodeList.Add(child_node);
            }
        }

        /// <summary>子ノード昇順ソート</summary>
        /// <param name="is_evaluate">未評価の子ノードの評価を行うか</param>
        public void AscendingSortChildNodes(bool is_evaluate) {
            if(ChildsCount > 1) {
                if(is_evaluate) {
                    ChildNodeList.Sort((a, b) => (a.Evaluation < b.Evaluation) ? -1 : ((a.Evaluation <= b.Evaluation) ? 0 : +1));
                }
                else {
                    ChildNodeList.Sort((a, b) => {
                        if(a.IsEvaluated && b.IsEvaluated) {
                            return (a.evaluation < b.evaluation) ? -1 : ((a.evaluation > b.evaluation) ? +1 : 0);
                        }

                        return (!a.IsEvaluated && !b.IsEvaluated) ? 0 : (a.IsEvaluated ? -1 : +1);
                    });
                }
            }
        }

        /// <summary>子ノード降順ソート</summary>
        /// <param name="is_evaluate">未評価の子ノードの評価を行うか</param>
        public void DescendingSortChildNodes(bool is_evaluate) {
            if(ChildsCount > 1) {
                if(is_evaluate) {
                    ChildNodeList.Sort((a, b) => (a.Evaluation > b.Evaluation) ? -1 : ((a.Evaluation >= b.Evaluation) ? 0 : +1));
                }
                else {
                    ChildNodeList.Sort((a, b) => {
                        if(a.IsEvaluated && b.IsEvaluated) {
                            return (a.evaluation > b.evaluation) ? -1 : ((a.evaluation < b.evaluation) ? +1 : 0);
                        }

                        return (!a.IsEvaluated && !b.IsEvaluated) ? 0 : (a.IsEvaluated ? -1 : +1);
                    });
                }
            }
        }

        /// <summary>盤面状態</summary>
        public StateType State { get; set; }

        /// <summary>この状態にした棋譜</summary>
        public DecisionType Decision { get; set; }

        /// <summary>親ノード</summary>
        public GameNode<StateType, DecisionType> ParentNode { get; }

        /// <summary>子ノードリスト</summary>
        public List<GameNode<StateType, DecisionType>> ChildNodeList { get; }

        /// <summary>子ノードの数</summary>
        public int ChildsCount {
            get {
                return ChildNodeList.Count;
            }
        }

        /// <summary>評価値</summary>
        public double Evaluation {
            get {
                return IsEvaluated ? evaluation : (evaluation = State.Evaluation);
            }
            set {
                evaluation = value;
            }
        }

        /// <summary>子ノードの先頭の評価値か、子ノードが存在しない時は自身の評価値</summary>
        public double EvaluationFirstChildOrSelf {
            get {
                return ChildsCount > 0 ? ChildNodeList[0].evaluation : evaluation;
            }
        }

        /// <summary>評価済みか否か</summary>
        public bool IsEvaluated {
            get {
                return !double.IsNaN(evaluation);
            }
            set {
                evaluation = value ? State.Evaluation : double.NaN;
            }
        }

        /// <summary>末端ノードか否か</summary>
        public bool IsEndNode {
            get {
                return ChildNodeList.Count <= 0 && State.IsEndGame;
            }
        }

        /// <summary>文字列出力</summary>
        public override string ToString() {
            return IsEvaluated ? $"EV = {evaluation}" : "UnEvaluated";
        }
    }
}
