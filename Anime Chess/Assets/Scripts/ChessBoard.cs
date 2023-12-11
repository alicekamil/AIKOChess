using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [Header("Art")] 
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3725f;
    [SerializeField] private float dragOffset = 0.9f;

    [Header("Prefabs & Materials")] 
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;
    
    // LOGIC
    private ChessPiece[,] chessPieces;
    private ChessPiece currentDragging;
    private List<Vector2Int> availableMoves = new();
    private List<ChessPiece> deadWhites = new();
    private List<ChessPiece> deadBlacks = new();
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;

    private Camera currentCamera;
    private GameObject[,] tiles;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private void Awake()
    {
        GenerateGrid(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        //TODO: Move to a playercontroller and invoke 
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // Get the indexes of the tile we hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // Check if there was no tile previously hovered
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            // Check if the new hovered tile is different from the previously hovered tile
            if (currentHover != hitPosition)
            {
                // Reset layer
                tiles[currentHover.x, currentHover.y].layer = (IsContainingValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                // Update current
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null)
                {
                    // Is our turn?
                    if (true)
                    {
                        currentDragging = chessPieces[hitPosition.x, hitPosition.y];
                        // List of walkable tiles
                        availableMoves = currentDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        SetHighLightedTiles();
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (currentDragging != null && Input.GetMouseButtonUp(0))
                {
                    var previousPosition = new Vector2Int(currentDragging.currentX, currentDragging.currentY); // Stupid?

                    
                    bool validMove = CanMoveTo(currentDragging, hitPosition.x, hitPosition.y);
                    if (!validMove)
                    {
                        //Back to its old position
                        currentDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y)); // Stupid?
                        currentDragging = null;
                    }
                    currentDragging = null;
                    RemoveHighLightedTiles();
                }
            }
        }
        else
        {
            // If the mouse cursor is not over any tile anymore
            
            // Check if there was a tile previously hovered
            if (currentHover != -Vector2Int.one)
            {
                // Reset the hovering
                tiles[currentHover.x, currentHover.y].layer = 
                    (IsContainingValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight")
                    : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentDragging && Input.GetMouseButtonUp(0))
            {
                currentDragging.SetPosition(GetTileCenter(currentDragging.currentX, currentDragging.currentY)); // Stupid?
                currentDragging = null;
                RemoveHighLightedTiles();
            }
        }
        // If dragging a piece
        if (currentDragging)
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
            {
                currentDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
            }
        }
    }
    

    // Generate Chessboard
    private void GenerateGrid(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;
        tiles = new GameObject[tileCountX, tileCountY];

        for (int x = 0; x < tileCountX; x++)
        for (int y = 0; y < tileCountY; y++)
            tiles[x, y] = GenerateTile(tileSize, x, y);
    }
    private GameObject GenerateTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject($"X:{x}, Y:{y}");
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        // Vertices
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize) - bounds;

        int[] tris = new[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");

        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }
    
    // Spawning
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);
        }
        
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);

        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }
    }
    /*private List<Piece> CreateTeam(Team team) //Boardcontroller
    {
        List<Piece> pieces = new List<Piece>();
        int firstRowY = team == Team.White ? 0 : 7;
        int secondRowY = team == Team.White ? 1 : 6;

        pieces.Add(new Piece(PieceType.King, team, new Vector2Integer(4, firstRowY)));
        pieces.Add(new Piece(PieceType.Queen, team, new Vector2Integer(3, firstRowY)));
        pieces.Add(new Piece(PieceType.Rook, team, new Vector2Integer(0, firstRowY)));
        pieces.Add(new Piece(PieceType.Rook, team, new Vector2Integer(7, firstRowY)));
        pieces.Add(new Piece(PieceType.Knight, team, new Vector2Integer(1, firstRowY)));
        pieces.Add(new Piece(PieceType.Knight, team, new Vector2Integer(6, firstRowY)));
        pieces.Add(new Piece(PieceType.Bishop, team, new Vector2Integer(2, firstRowY)));
        pieces.Add(new Piece(PieceType.Bishop, team, new Vector2Integer(5, firstRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(0, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(1, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(2, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(3, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(4, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(5, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(6, secondRowY)));
        pieces.Add(new Piece(PieceType.Pawn, team, new Vector2Integer(7, secondRowY)));

        return pieces;
    }*/
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        return cp;
    }
    
    // Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(chessPieces[x,y] != null)
                    PositionSinglePiece(x,y, true);
    }

    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x, y].currentY = y;
        chessPieces[x, y].SetPosition(GetTileCenter(x, y), force);

    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }
    
    // Highlight tiles
    private void SetHighLightedTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighLightedTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }
    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // Invalid
    }
    private bool CanMoveTo(ChessPiece chessPiece, int x, int y)
    {
        if (!IsContainingValidMove(ref availableMoves, new Vector2(x, y)))
        {
            return false;
        }
        
        Vector2Int previousPosition = new Vector2Int(chessPiece.currentX, chessPiece.currentY);

        // Is there another piece on the target position?
        if (chessPieces[x, y] != null)
        {
            ChessPiece otherChessPiece = chessPieces[x, y];

            if (chessPiece.team == otherChessPiece.team)
            {
                return false;
            }

            if (otherChessPiece.team == 0) // Stupid
            {
                deadWhites.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathSize); // Stupid
                otherChessPiece.SetPosition(
                    new Vector3(8 * tileSize, yOffset, -1 * tileSize)
                    - bounds
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.forward * deathSpacing) * deadWhites.Count); // So stupid
                    
            }
            else
            {
                deadBlacks.Add(otherChessPiece);
                otherChessPiece.SetScale(Vector3.one * deathSize); // Stupid
                otherChessPiece.SetPosition(
                    new Vector3(-1 * tileSize, yOffset, 8 * tileSize)
                    - bounds
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.back * deathSpacing) * deadBlacks.Count); // So stupid
            }
        }
        chessPieces[x, y] = chessPiece;
        chessPieces[previousPosition.x, previousPosition.y] = null;
        
        PositionSinglePiece(x,y);

        return true;
    }

    private bool IsContainingValidMove(ref List<Vector2Int> moves, Vector2 pos) // Stupid
    {
        for (int i = 0; i < moves.Count; i++)
        {
            if (moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }
        return false;
    }
}
