using System;
using System.Collections.Generic;

namespace ChessApp.Chess
{
    public class Game
    {
        public Piece[,] Board { get; } = new Piece[8,8];
        public bool WhiteToMove { get; private set; } = true;
        private readonly Stack<Move> _history = new();
        public string LastMoveNotation { get; private set; } = "";

        public Game()
        {
            Setup();
        }

        private void Setup()
        {
            // Pawns
            for (int c = 0; c < 8; c++)
            {
                Board[6, c] = Piece.WhitePawn;
                Board[1, c] = Piece.BlackPawn;
            }
            // Rooks
            Board[7,0] = Piece.WhiteRook; Board[7,7] = Piece.WhiteRook;
            Board[0,0] = Piece.BlackRook; Board[0,7] = Piece.BlackRook;
            // Knights
            Board[7,1] = Piece.WhiteKnight; Board[7,6] = Piece.WhiteKnight;
            Board[0,1] = Piece.BlackKnight; Board[0,6] = Piece.BlackKnight;
            // Bishops
            Board[7,2] = Piece.WhiteBishop; Board[7,5] = Piece.WhiteBishop;
            Board[0,2] = Piece.BlackBishop; Board[0,5] = Piece.BlackBishop;
            // Queens
            Board[7,3] = Piece.WhiteQueen; Board[0,3] = Piece.BlackQueen;
            // Kings
            Board[7,4] = Piece.WhiteKing; Board[0,4] = Piece.BlackKing;
        }

        public void Undo()
        {
            if (_history.Count == 0) return;
            var m = _history.Pop();
            Board[m.Sr, m.Sc] = m.Moved;
            Board[m.Tr, m.Tc] = m.Captured;
            WhiteToMove = !WhiteToMove;
        }

        public bool TryMakeMove(int sr, int sc, int tr, int tc)
        {
            var piece = Board[sr, sc];
            if (piece == Piece.Empty) return false;
            if (Utils.IsWhite(piece) != WhiteToMove) return false;
            if (!IsPseudoLegal(piece, sr, sc, tr, tc)) return false;
            // capture own piece check
            if (Board[tr, tc] != Piece.Empty && Utils.IsWhite(Board[tr, tc]) == Utils.IsWhite(piece)) return false;

            var captured = Board[tr, tc];
            // make move
            Board[sr, sc] = Piece.Empty;
            Board[tr, tc] = piece;

            // Not doing full check detection; basic legality only
            _history.Push(new Move(sr, sc, tr, tc, piece, captured));
            WhiteToMove = !WhiteToMove;
            LastMoveNotation = $"{Square(sr,sc)}-{Square(tr,tc)}";
            return true;
        }

        private static string Square(int r, int c) => $"{(char)('a'+c)}{8-r}";

        private static bool ClearPath(Piece[,] b, int sr,int sc,int tr,int tc)
        {
            int dr = Math.Sign(tr - sr);
            int dc = Math.Sign(tc - sc);
            int r = sr + dr, c = sc + dc;
            while (r != tr || c != tc)
            {
                if (b[r, c] != Piece.Empty) return false;
                r += dr; c += dc;
            }
            return true;
        }

        private bool IsPseudoLegal(Piece p, int sr,int sc,int tr,int tc)
        {
            if (sr==tr && sc==tc) return false;
            int dr = tr - sr, dc = tc - sc;
            switch (p)
            {
                case Piece.WhitePawn:
                    if (dc == 0 && dr == -1 && Board[tr, tc] == Piece.Empty) return true;
                    if (dc == 0 && sr == 6 && dr == -2 && Board[sr-1, sc] == Piece.Empty && Board[tr, tc] == Piece.Empty) return true;
                    if (Math.Abs(dc) == 1 && dr == -1 && Board[tr, tc] != Piece.Empty && !Utils.IsWhite(Board[tr, tc])) return true;
                    return false;
                case Piece.BlackPawn:
                    if (dc == 0 && dr == 1 && Board[tr, tc] == Piece.Empty) return true;
                    if (dc == 0 && sr == 1 && dr == 2 && Board[sr+1, sc] == Piece.Empty && Board[tr, tc] == Piece.Empty) return true;
                    if (Math.Abs(dc) == 1 && dr == 1 && Board[tr, tc] != Piece.Empty && Utils.IsWhite(Board[tr, tc])) return true;
                    return false;
                case Piece.WhiteKnight:
                case Piece.BlackKnight:
                    return (Math.Abs(dr), Math.Abs(dc)) is (2,1) or (1,2);
                case Piece.WhiteBishop:
                case Piece.BlackBishop:
                    return Math.Abs(dr) == Math.Abs(dc) && ClearPath(Board, sr, sc, tr, tc);
                case Piece.WhiteRook:
                case Piece.BlackRook:
                    return (dr == 0 || dc == 0) && ClearPath(Board, sr, sc, tr, tc);
                case Piece.WhiteQueen:
                case Piece.BlackQueen:
                    return ((dr == 0 || dc == 0) || Math.Abs(dr) == Math.Abs(dc)) && ClearPath(Board, sr, sc, tr, tc);
                case Piece.WhiteKing:
                case Piece.BlackKing:
                    return Math.Max(Math.Abs(dr), Math.Abs(dc)) == 1;
                default: return false;
            }
        }

        private record Move(int Sr,int Sc,int Tr,int Tc, Piece Moved, Piece Captured);
    }
}