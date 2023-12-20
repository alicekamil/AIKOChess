using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // Define the direction of movement for the pawn based on the team
        // 1 for white team, -1 for black team
        int direction = (team == 0) ? 1 : -1;

        // If the tile in front is within bounds and empty 
        if(currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX,currentY + direction] == null)
        { // Add the tile in front of us to the list of walkable
            validMoves.Add(new Vector2Int(currentX, currentY + direction));
        }
        
        // If white team and the pawn is on its start position
        if (team == 0 && currentY == 1)
        { // If two tiles in front are empty
            if (board[currentX, currentY + direction] == null && board[currentX, currentY + 2 * direction] == null)
            { 
                validMoves.Add(new Vector2Int(currentX, currentY + 2 * direction));
            }
        }
        
        // If black team and the pawn is on its start position
        if (team == 1 && currentY == tileCountY-2)
        { // If two tiles in front are empty 
            if (board[currentX, currentY + direction] == null && board[currentX, currentY + 2 * direction] == null)
            {
                validMoves.Add(new Vector2Int(currentX, currentY + 2 * direction));
            }
        }
        
        // Check if theres enemies on diagonal tiles
        if (currentX + 1 != tileCountX && currentY + direction >= 0 && currentY + direction < tileCountY)
        {
            // If the right diagonal tile has an opponent's piece
            if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
            {
                validMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
            }
        }
        
        // Check if the pawn is not on the leftmost column,
        // and if the tile in front is within the vertical bounds of the board
        if (currentX != 0 && currentY + direction >= 0 && currentY + direction < tileCountY)
        {
            // If the left diagonal tile has an opponent's piece
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
            {
                validMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
            }
        }

        // Return valid moves
        return validMoves;
    }
}

