using ChessChallenge.API;
using System;
using System.Collections.Generic;

// first time ever i am programming in c#, and first time - bots

public class MyBot : IChessBot
{
	private bool i_am_white;
	private int army_weight = 1;
	private int freedom_weight = 1;
	private int material_weight = 1;
	
	private Dictionary<Move, int> moveValueDictionary = new Dictionary<Move, int>();
	
	//private int Spread = 5;
	//private int Depth = 5;
	//private int Recent_Evaluation = 0;
	
	
	private int Evaluate_army(Board board)
	{
		int army_size_dom = 0;
		army_size_dom += BitboardHelper.GetNumberOfSetBits(board.WhitePiecesBitboard);
		army_size_dom -= BitboardHelper.GetNumberOfSetBits(board.BlackPiecesBitboard);
		return army_size_dom;
	}
	
	
	private int Evaluate_material(Board board)
	{
		int army_size_dom = 0;
		//BitboardHelper.GetNumberOfSetBits(board.WhitePiecesBitboard) - 
		//BitboardHelper.GetNumberOfSetBits(board.BlackPiecesBitboard);
		return army_size_dom;
	}
	
	
	private int Evaluate_freedom(Board board)
	{
		int white_move_mult = (board.IsWhiteToMove ? 1 : -1);
		int freedom_current = board.GetLegalMoves().Length;

		board.ForceSkipTurn();
		int freedom_opponent = board.GetLegalMoves().Length;
		board.UndoSkipTurn();
		
		return white_move_mult * (freedom_current - freedom_opponent);
	}
	
	
	private int Evaluate_position(Board board) //higher better for white
	{
		int result = 0;
		result += freedom_weight * Evaluate_freedom(board);
		result += army_weight * Evaluate_army(board);
		//result += army_weight * Evaluate_material(board);
		
		return result; 
	}
	
	
    public Move Think(Board board, Timer timer)
	{
		moveValueDictionary.Clear();
		bool i_am_white = board.IsWhiteToMove;
		
        Move[] allMoves = board.GetLegalMoves();
        foreach (Move move in allMoves)
		{
			if( board.SquareIsAttackedByOpponent(move.TargetSquare) )
			{
				continue;
			}
			board.MakeMove(move);		
				if (board.IsInCheckmate()) return move;
				moveValueDictionary.Add(move, Evaluate_position(board));			
			board.UndoMove(move);
        }
		
		Random rng = new();
		Move final_move = allMoves[rng.Next(allMoves.Length)];
		
		int best_score = i_am_white ? -1000 : 1000;
		foreach (var pair in moveValueDictionary)
		{
			if(i_am_white)
			{	
				if(pair.Value > best_score)
				{
					final_move = pair.Key;
					best_score = pair.Value;
				}
			}
			else
			{
				if(pair.Value < best_score)
				{
					final_move = pair.Key;
					best_score = pair.Value;
				}
			}
		}
		
        return final_move;
    }
}