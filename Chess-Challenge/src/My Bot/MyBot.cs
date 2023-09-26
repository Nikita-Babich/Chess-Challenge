using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
	//test Tuple with public (Move, int) function
	Random random = new Random();
	
	//private bool i_am_white; //zero black, one white 
	private int army_weight = 100;
	private int freedom_weight = 1;
	
	private int Evaluate_army_size(Board board)
	{
		int army_size_dom = 0;
		army_size_dom += BitboardHelper.GetNumberOfSetBits(board.WhitePiecesBitboard);
		army_size_dom -= BitboardHelper.GetNumberOfSetBits(board.BlackPiecesBitboard);
		return army_size_dom;
	}
	
	private int Evaluate_freedom(Board board) //absolute score
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
		result += army_weight * Evaluate_army_size(board);
		result += random.Next(0, 3); // Fluctuation for vriability in gameplay
		return result; 
	}
	
	private (int, Move) Minimax(Board board, int depth, bool maximizingPlayer)
	{
		Move BestMove = board.GetLegalMoves()[0];
		int BestValue;
		
		if (depth == 0 | board.IsInCheckmate())
		{
			int score = Evaluate_position(board);
			return (score, BestMove); //new MoveResult {BestMove = null, BestValue = score};
		}
		
		if (maximizingPlayer)
		{
			BestValue = int.MinValue;
			foreach (Move child in board.GetLegalMoves())
			{
				board.MakeMove(child);
				var (LocalValue, LocalMove) = Minimax(board, depth - 1, false);
				if (LocalValue > BestValue)
				{
					BestValue = LocalValue;
					BestMove = child;
				}
				board.UndoMove(child);
			}
		}
		else
		{
			BestValue = int.MaxValue;
			foreach (Move child in board.GetLegalMoves())
			{
				board.MakeMove(child);
				var (LocalValue, LocalMove) = Minimax(board, depth - 1, false);
				if (LocalValue < BestValue)
				{
					BestValue = LocalValue;
					BestMove = child;
				}
				board.UndoMove(child);
			}
		}
		return (BestValue, BestMove);
	}
	
    public Move Think(Board board, Timer timer)
	{
		
		bool i_am_white = board.IsWhiteToMove;
		var (_, final_move) = Minimax(board, 3, i_am_white); //.Item1
        
		
        return final_move;
    }
}