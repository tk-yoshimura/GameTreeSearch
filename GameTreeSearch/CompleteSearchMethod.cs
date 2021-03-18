using System;

namespace GameTreeSearch {

    /// <summary>完全探索法</summary>
    /// <typeparam name="StateType">盤面状態クラス</typeparam>
    /// <typeparam name="DecisionType">棋譜クラス</typeparam>
    public static class CompleteSearchMethod<StateType, DecisionType> where StateType : IState<DecisionType> where DecisionType : new() {

        /// <summary>探索</summary>
        /// <param name="root_state">ルートとなる状態</param>
        /// <param name="pass_decision">パス決定オブジェクト(有効な棋譜が存在しない場合に採用される)</param>
        /// <returns>ゲーム木探索によって決定された棋譜</returns>
        public static DecisionType Search(StateType root_state, DecisionType pass_decision) {
            if(root_state.IsEndGame) {
                return pass_decision;
            }

            double completesearch_method(GameNode<StateType, DecisionType> node, bool is_player, double alpha, double beta) {
                if (node.IsEndNode) {
                    return node.Evaluation;
                }

                if (is_player) {
                    GameNode<StateType, DecisionType> max_node = null;
                    double max_ev = double.NegativeInfinity;

                    foreach (var child_node in node.ChildNodes()) {
                        double ev = completesearch_method(child_node, !is_player, alpha, beta);

                        if (max_ev < ev) {
                            max_node = child_node;
                            max_ev = ev;
                        }

                        if (alpha < ev) {
                            alpha = ev;
                        }

                        if (alpha >= beta) {
                            node.ChildNodeList.Clear();
                            if (max_node != null) {
                                node.ChildNodeList.Add(max_node);
                            }

                            return beta;
                        }
                    }

                    node.ChildNodeList.Clear();
                    if (max_node != null) {
                        node.ChildNodeList.Add(max_node);
                    }

                    return alpha;
                }
                else {
                    GameNode<StateType, DecisionType> min_node = null;
                    double min_ev = double.PositiveInfinity;

                    foreach (var child_node in node.ChildNodes()) {
                        double ev = completesearch_method(child_node, !is_player, alpha, beta);

                        if (min_ev > ev) {
                            min_node = child_node;
                            min_ev = ev;
                        }

                        if (beta > ev) {
                            beta = ev;
                            min_node = child_node;
                        }

                        if (alpha >= beta) {
                            node.ChildNodeList.Clear();
                            if (min_node != null) {
                                node.ChildNodeList.Add(min_node);
                            }

                            return alpha;
                        }
                    }

                    node.ChildNodeList.Clear();
                    if (min_node != null) {
                        node.ChildNodeList.Add(min_node);
                    }

                    return beta;
                }
            }

            var root_node = new GameNode<StateType, DecisionType>(root_state, new DecisionType(), null);

            completesearch_method(root_node, true, double.NegativeInfinity, double.PositiveInfinity);

            if(root_node.ChildNodeList.Count > 0) {
                return root_node.ChildNodeList[0].Decision;
            }
            else {
                return pass_decision;
            }
        }
    }
}
