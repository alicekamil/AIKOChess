using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        //can only move vertically or horizontally, til same tile as enemy / unoccupied /til board ends
        List<Vector2Int> r = new List<Vector2Int>();
        
        int direction = (team == 0) ? 1 : -1;
        
        if(currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX,currentY + direction] == null)
        {
            r.Add(new Vector2Int(currentX, currentY + direction));
        }
        
        return r;
    }
}
