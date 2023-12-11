using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;

        //One in front
        if(currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX,currentY + direction] == null)
        {
            r.Add(new Vector2Int(currentX, currentY + direction));
        }
        //Two in front
        if (team == 0 && currentY == 1)
        {
            if (board[currentX, currentY + direction] == null && board[currentX, currentY + 2 * direction] == null)
            {
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

