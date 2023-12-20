using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();

        // Define the eight possible directions a bishop can move (diagonal)
        Vector2Int[] directions =
        {
            new Vector2Int(1, 1),   // Diagonal top-right
            new Vector2Int(1, -1),  // Diagonal bottom-right
            new Vector2Int(-1, 1),  // Diagonal top-left
            new Vector2Int(-1, -1), // Diagonal bottom-left
            new Vector2Int(0, 1),  // Up
            new Vector2Int(1, 0),  // Right
            new Vector2Int(0, -1), // Down
            new Vector2Int(-1,0),  // Left
        };

        foreach (var direction in directions)
        {
            // Check each cell along the diagonal up to the edge of the board
            for (int i = 1; i <= tileCountX; i++)
            {
                
                Vector2Int targetPosition = new Vector2Int(currentX + i * direction.x, currentY + i * direction.y);

                if (!IsWithinBoardBounds(targetPosition, tileCountX, tileCountY))
                {
                    // Stop searching in this direction if out of board bounds
                    break;
                }

                if (board[targetPosition.x, targetPosition.y] == null)
                {
                    // Empty cell, add as a valid move
                    validMoves.Add(targetPosition);
                }
                else
                {
                    // Cell is occupied, check if its an opponents piece (attack)
                    if (board[targetPosition.x, targetPosition.y].team != team)
                    {
                        validMoves.Add(targetPosition);
                    }

                    // Stop searching in this direction, as the bishop cant jump over pieces
                    break;
                }
            }
        }

        return validMoves;
    }

    private bool IsWithinBoardBounds(Vector2Int position, int tileCountX, int tileCountY)
    {
        return position.x >= 0 && position.x < tileCountX && position.y >= 0 && position.y < tileCountY;
    }
}
