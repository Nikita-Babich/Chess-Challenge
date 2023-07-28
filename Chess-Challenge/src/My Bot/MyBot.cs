using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
	//I tried to implement the same way of thinking that i use when playing chess against people.
    //Note: checkmate and stalemate - both can be simply defined as a situation with 0 legal moves.
    //The algorithm tries to achieve any of those by reducing opponent's freedom.
	//private float material_dominance = 1;
	private Dictionary<Move, int> moveValueDictionary = new Dictionary<Move, int>();
	
    public Move Think(Board board, Timer timer)
	{
		moveValueDictionary.Clear();
        Move[] allMoves = board.GetLegalMoves();
        foreach (Move move in allMoves)
		{
			if (MoveIsCheckmate(board, move))
			{
                return move;
            }
			MakeMove(move);
			moveValueDictionary.Add(move, board.GetLegalMoves().Length);
			UndoMove(move);
        }
		
		Move final_move = allMoves[0];
		int least_freedom = 300;
		foreach (var pair in moveValueDictionary)
		{
			if(pair.Value < least_freedom)
			{
				final_move = pair.Key;
				least_freedom = pair.Value;
			}
		}
		
        return final_move;
    }
}