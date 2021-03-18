using System;

namespace GameTreeSearch {

    /// <summary>探索打ち切り可能な反復深化探索法</summary>
    /// <typeparam name="StateType">盤面状態クラス</typeparam>
    /// <typeparam name="DecisionType">棋譜クラス</typeparam>
    public static class DiscontinuableIDDFSMethod<StateType, DecisionType> where StateType : IState<DecisionType> where DecisionType : new() {

        /// <summary>探索</summary>
        /// <param name="root_state">ルートとなる状態</param>
        /// <param name="minimum_depth">最低限探索するゲーム木の深さ</param>
        /// <param name="maximum_depth">最大限探索するゲーム木の深さ</param>
        /// <param name="discontinue_node_count">ゲーム木のノード打ち切り評価数</param>
        /// <param name="pass_decision">パス決定オブジェクト(有効な棋譜が存在しない場合に採用される)</param>
        /// <returns>ゲーム木探索によって決定された棋譜</returns>
        public static DecisionType Search(StateType root_state, int minimum_depth, int maximum_depth, int discontinue_node_count, DecisionType pass_decision) {
            if(maximum_depth <= minimum_depth || minimum_depth <= 1 || discontinue_node_count <= 0 || root_state.IsEndGame) {
                return pass_decision;
            }

            double iddfs_method(GameNode<StateType, DecisionType> node, int depth, int limit_depth, bool is_player, double alpha, double beta) {
                if (depth >= limit_depth || node.IsEndNode) {
                    discontinue_node_count--;
                    return node.Evaluation;
                }
                else if (node.ChildsCount <= 0) {
                    node.ExpandChildNodes();
                }
                else {
                    foreach (var child_node in node.ChildNodeList) {
                        child_node.IsEvaluated = false;
                    }
                }

                if (is_player) {
                    foreach (var child_node in node.ChildNodeList) {
                        double ev = iddfs_method(child_node, depth + 1, limit_depth, !is_player, alpha, beta);

                        if (alpha < ev) {
                            alpha = ev;
                        }

                        if (limit_depth > minimum_depth && discontinue_node_count <= 0) {
                            break;
                        }

                        if (alpha >= beta) {
                            node.DescendingSortChildNodes(is_evaluate: false);
                            node.Evaluation = node.EvaluationFirstChildOrSelf;
                            return beta;
                        }
                    }

                    node.DescendingSortChildNodes(is_evaluate: false);
                    node.Evaluation = node.EvaluationFirstChildOrSelf;
                    return alpha;
                }
                else {
                    foreach (var child_node in node.ChildNodeList) {
                        double ev = iddfs_method(child_node, depth + 1, limit_depth, !is_player, alpha, beta);

                        if (beta > ev) {
                            beta = ev;
                        }

                        if (limit_depth > minimum_depth && discontinue_node_count <= 0) {
                            break;
                        }

                        if (alpha >= beta) {
                            node.AscendingSortChildNodes(is_evaluate: false);
                            node.Evaluation = node.EvaluationFirstChildOrSelf;
                            return alpha;
                        }
                    }

                    node.AscendingSortChildNodes(is_evaluate: false);
                    node.Evaluation = node.EvaluationFirstChildOrSelf;
                    return beta;
                }
            }

            var root_node = new GameNode<StateType, DecisionType>(root_state, new DecisionType(), null);

            iddfs_method(root_node, 0, 1, true, double.NegativeInfinity, double.PositiveInfinity);

            for(int limit_depth = 2; limit_depth <= maximum_depth; limit_depth++) {
                iddfs_method(root_node, 0, limit_depth, true, double.NegativeInfinity, double.PositiveInfinity);

                if(limit_depth >= minimum_depth && discontinue_node_count <= 0) {
                    break;
                }
            }

            if(root_node.ChildNodeList.Count > 0) {
                return root_node.ChildNodeList[0].Decision;
            }
            else {
                return pass_decision;
            }
        }
    }
}
