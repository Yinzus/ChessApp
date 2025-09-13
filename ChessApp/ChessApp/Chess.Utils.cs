namespace ChessApp.Chess
{
    public static class Utils
    {
        public static bool IsWhite(Piece p) => p switch
        {
            Piece.WhitePawn or Piece.WhiteKnight or Piece.WhiteBishop or Piece.WhiteRook or Piece.WhiteQueen or Piece.WhiteKing => true,
            _ => false
        };

        public static string ToGlyph(Piece p) => p switch
        {
            Piece.WhiteKing => "♔",
            Piece.WhiteQueen => "♕",
            Piece.WhiteRook => "♖",
            Piece.WhiteBishop => "♗",
            Piece.WhiteKnight => "♘",
            Piece.WhitePawn => "♙",
            Piece.BlackKing => "♚",
            Piece.BlackQueen => "♛",
            Piece.BlackRook => "♜",
            Piece.BlackBishop => "♝",
            Piece.BlackKnight => "♞",
            Piece.BlackPawn => "♟",
            _ => ""
        };
    }

    public enum Piece
    {
        Empty,
        WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing,
        BlackPawn, BlackKnight, BlackBishop, BlackRook, BlackQueen, BlackKing
    }
}