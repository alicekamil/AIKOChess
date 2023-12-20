using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // Define the possible relative positions for a knight's L-shaped movement
        Vector2Int[] knightMoves =
        {
            new Vector2Int(2, 1),
            new Vector2Int(1, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(-1, -2),
            new Vector2Int(1, -2),
            new Vector2Int(2, -1),
        };

        foreach (var move in knightMoves)
        {
            Vector2Int targetPosition = new Vector2Int(currentX + move.x, currentY + move.y);

            if (IsWithinBoardBounds(targetPosition, tileCountX, tileCountY) &&
                (board[targetPosition.x, targetPosition.y] == null || board[targetPosition.x, targetPosition.y].team != team))
            {
                // Empty cell or cell with an opponent's piece, add as a valid move
                validMoves.Add(targetPosition);
            }
        }

        return validMoves;
    }

    private bool IsWithinBoardBounds(Vector2Int position, int tileCountX, int tileCountY)
    {
        return position.x >= 0 && position.x < tileCountX && position.y >= 0 && position.y < tileCountY;
    }
}

