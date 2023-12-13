using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        // If our current pos in Y is Greater than or equal zero, less than the total tilecount, and the tile in front is empty? 
        if(currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX,currentY + direction] == null)
        { // Add the tile in front of us to the list of walkable
            r.Add(new Vector2Int(currentX, currentY + direction));
        }
        // If white team and the pawn is on its start position
        if (team == 0 && currentY == 1)
        { // If two tiles in front of us are empty
            if (board[currentX, currentY + direction] == null && board[currentX, currentY + 2 * direction] == null)
            { // Add two tiles in front of us to the list of walkable
                r.Add(new Vector2Int(currentX, currentY + 2 * direction));
            }
        }
        if (team == 1 && currentY == tileCountY-2)
        {
            if (board[currentX, currentY + direction] == null && board[currentX, currentY + 2 * direction] == null)
            {
                r.Add(new Vector2Int(currentX, currentY + 2 * direction));
            }
        }
        // Attack
        if (currentX +1 != tileCountX && currentY + direction >= 0 && currentY + direction < tileCountY)
            if(board[currentX + 1, currentY + direction] != null&& board[currentX+1, currentY+direction].team != team)
                r.Add(new Vector2Int(currentX + 1, currentY + direction));
        if(currentX != 0 && currentY + direction >= 0 && currentY + direction < tileCountY)
            if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX - 1, currentY + direction));
        
        return r;
    }
}

