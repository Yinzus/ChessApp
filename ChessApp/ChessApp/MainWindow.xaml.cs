using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessApp
{
    public partial class MainWindow : Window
    {
        private Chess.Game _game = new Chess.Game();
        private Button? _selected;
        private readonly Dictionary<(int r,int c), Button> _buttons = new();

        public MainWindow()
        {
            InitializeComponent();
            BuildBoard();
            Render();
        }

        private void BuildBoard()
        {
            BoardGrid.Children.Clear();
            _buttons.Clear();
            for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                var btn = new Button { FontSize = 28, FontFamily = new FontFamily("Segoe UI Symbol") };
                btn.Background = ((r + c) % 2 == 0) ? Brushes.Beige : Brushes.SaddleBrown;
                btn.Click += OnCellClick;
                btn.Tag = (r, c);
                _buttons[(r, c)] = btn;
                BoardGrid.Children.Add(btn);
            }
        }

        private void OnCellClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var (r, c) = ((int, int))btn.Tag;

            if (_selected == null)
            {
                // select if piece belongs to side to move
                if (_game.Board[r, c] != Chess.Piece.Empty && Chess.Utils.IsWhite(_game.Board[r, c]) == _game.WhiteToMove)
                {
                    _selected = btn;
                    btn.BorderThickness = new Thickness(3);
                    btn.BorderBrush = Brushes.Yellow;
                }
            }
            else
            {
                var (sr, sc) = ((int, int))_selected.Tag;
                if (_game.TryMakeMove(sr, sc, r, c))
                {
                    MovesPanel.Children.Add(new TextBlock { Text = _game.LastMoveNotation, Margin = new Thickness(0,2,0,2) });
                }
                _selected.BorderThickness = new Thickness(1);
                _selected.BorderBrush = Brushes.Black;
                _selected = null;
                Render();
            }
        }

        private void Render()
        {
            TurnText.Text = _game.WhiteToMove ? "White" : "Black";
            for (int r = 0; r < 8; r++)
            for (int c = 0; c < 8; c++)
            {
                var btn = _buttons[(r, c)];
                btn.Content = Chess.Utils.ToGlyph(_game.Board[r, c]);
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            _game = new Chess.Game();
            MovesPanel.Children.Clear();
            _selected = null;
            Render();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            _game.Undo();
            Render();
        }
    }
}