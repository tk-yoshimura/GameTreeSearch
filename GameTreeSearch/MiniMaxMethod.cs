namespace GameTreeSearch {

    /// <summary>ミニマックス法</summary>
    /// <typeparam name="StateType">盤面状態クラス</typeparam>
    /// <typeparam name="DecisionType">棋譜クラス</typeparam>
    public static class MiniMaxMethod<StateType, DecisionType> where StateType : IState<DecisionType> where DecisionType : new() {

        /// <summary>探索</summary>
        /// <param name="root_state">ルートとなる状態</param>
        /// <param name="max_depth">ゲーム木の深さ</param>
        /// <param name="pass_decision">パス決定オブジェクト(有効な棋譜が存在しない場合に採用される)</param>
        /// <returns>ゲーム木探索によって決定された棋譜</returns>
        public static DecisionType Search(StateType root_state, int max_depth, DecisionType pass_decision) {
            if(max_depth <= 0 || root_state.IsEndGame) {
                return pass_decision;
            }

            double minimax_method(GameNode<StateType, DecisionType> node, int depth, bool is_player) {
                if (depth >= max_depth || node.IsEndNode) {
                    return node.Evaluation;
                }

                if (is_player) {
                    GameNode<StateType, DecisionType> max_node = null;
                    double max_ev = double.NegativeInfinity;

                    foreach (GameNode<StateType, DecisionType> child_node in node.ChildNodes()) {
                        double ev = minimax_method(child_node, depth + 1, !is_player);

                        if (max_ev < ev) {
                            max_node = child_node;
                            max_ev = ev;
                        }
                    }

                    node.ChildNodeList.Clear();
                    if (max_node != null) {
                        node.ChildNodeList.Add(max_node);
                    }

                    return max_ev;
                }
                else {
                    GameNode<StateType, DecisionType> min_node = null;
                    double min_ev = double.PositiveInfinity;

                    foreach (GameNode<StateType, DecisionType> child_node in node.ChildNodes()) {
                        double ev = minimax_method(child_node, depth + 1, !is_player);

                        if (min_ev > ev) {
                            min_node = child_node;
                            min_ev = ev;
                        }
                    }

                    node.ChildNodeList.Clear();
                    if (min_node != null) {
                        node.ChildNodeList.Add(min_node);
                    }

                    return min_ev;
                }
            }

            GameNode<StateType, DecisionType> root_node = new(root_state, new DecisionType(), null);

            minimax_method(root_node, 0, true);

            if(root_node.ChildNodeList.Count > 0) {
                return root_node.ChildNodeList[0].Decision;
            }
            else {
                return pass_decision;
            }
        }
    }
}
