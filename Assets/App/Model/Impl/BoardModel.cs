﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace App.Model
{
    using Common;

    /// <summary>
    /// The main playing board Can be of arbitrary dimention.
    /// Contents are stored as row-major. the bottom left corner for white is at contents[0][0]
    /// the topright corner for white is at contents[Height - 1][Width - 1]
    /// Both Black and White use the same coordinate system.
    /// </summary>
    public class BoardModel
        : ModelBase
        , IBoardModel
    {
        #region Public Properties
        public int Width { get; }
        public int Height { get; }
        [Inject] public IArbiterModel Arbiter { get; set; }
        public IPlayerModel WhitePlayer => Arbiter.WhitePlayer;
        public IPlayerModel BlackPlayer => Arbiter.BlackPlayer;
        public IEnumerable<IPieceModel> Pieces => GetContents();
        #endregion

        #region Public Methods
        public BoardModel() { throw new NotImplementedException(); }

        public BoardModel(int width, int height)
        {
            Width = width;
            Height = height;

            ConstructBoard();
        }

        public void NewGame(IArbiterModel arbiter)
        {
            Arbiter = arbiter;
            ClearBoard();
            ConstructBoard();
        }

        public IPieceModel RemovePiece(Coord coord)
        {
            if (!IsValid(coord))
            {
                Warn($"Attempt to remove from invalid {coord}");
                return null;
            }
            var current = _contents[coord.y][coord.x];
            _contents[coord.y][coord.x] = null;
            return current;
        }

        private void ConstructBoard()
        {
            _contents = new List<List<IPieceModel>>();
            for (var n = 0; n < Height; ++n)
            {
                var row = new List<IPieceModel>();
                for (var m = 0; m < Width; ++m)
                    row.Add(null);
                _contents.Add(row);
            }
        }

        public bool IsValid(Coord coord)
        {
            return coord.x >= 0 && coord.y >= 0 && coord.x < Width && coord.y < Height;
        }

        public bool CanPlaceCard(ICardModel card, Coord coord)
        {
            Assert.IsNotNull(card);
            Assert.IsTrue(IsValid(coord));

            // can play on empty square...
            var existing = At(coord);
            if (existing == null)
            {
                // ...unless within 4 squares of enemy king
                var adj = GetAdjacent(coord, Parameters.EnemyKingClosestPlacement).ToArray();
                return false;
                //return
                //    adj.Length == 0 ||
                //    adj.Any(c => c.Card.Type == ECardType.Piece && ((IPieceModel)c.Card).Type  == EPieceType.King && !c.Card.SameOwner(card.Owner));
            }

            // this is actually a battle
            if (existing.Owner != card.Owner)
                return true;

            // true if we can mount an existing card there
            var mountable = card as ICardModelMountable;
            if (mountable != null)
                return mountable.CanMount(card);

            return false;
        }

        public IEnumerable<IPieceModel> GetAdjacent(Coord coord, int dist = 1)
        {
            var items = new List<IPieceModel>();
            for (var y = -dist; y <= dist; ++y)
            {
                for (var x = -dist; x <= dist; ++x)
                {
                    if (x == 0 && y == 0)
                        continue;

                    var piece = At(coord);
                    if (piece == null)
                        continue;

                    items.Add(piece);
                }
            }
            return items;
        }

        public IEnumerable<IPieceModel> AttackedCards(Coord coord)
        {
            var piece = At(coord);
            if (piece == null)
                yield break;
            foreach (var c in GetPossibleMovements(piece))
            {
                var attacked = At(c);
                if (attacked != null)
                    yield return attacked;
            }
        }

        private void ClearBoard()
        {
            foreach (var card in GetContents())
            {
                RemovePiece(card.Coord);
                card.Destroy();
            }
        }

        private IEnumerable<Coord> Orthogonals(Coord orig)
        {
            for (var y = Max(orig.y - Height, 0); y < Height; ++y)
            {
                var coord = new Coord(orig.x, y);
                if (!IsValid(coord))
                    continue;
                if (!Equals(coord, orig))
                    yield return coord;
            }

            for (var x = Max(orig.x - Width, 0); x < Width; ++x)
            {
                var coord = new Coord(x, orig.y);
                if (!IsValid(coord))
                    continue;
                if (!Equals(coord, orig))
                    yield return coord;
            }
        }

        private IEnumerable<Coord> TestCoords(Coord orig, int dx, int dy)
        {
            for (int n = 1; n < Max(Width, Height); ++n)
            {
                var x = n * dx;
                var y = n * dy;
                var d = new Coord(x, y);
                var c = orig + d;
                if (!IsValid(c))
                    continue;
                yield return c;
            }
        }

        private IEnumerable<Coord> Diagonals(Coord orig)
        {
            for (int dx = -1; dx < 2; dx++)
            {
                if (dx == 0) continue;
                for (int dy = -1; dy < 2; dy++)
                {
                    if (dy == 0) continue;
                    foreach (var c in TestCoords(orig, dx, dy))
                        yield return c;
                }
            }
            yield break;
        }

        private IEnumerable<Coord> GetPossibleMovements(IPieceModel piece)
        {
            return null;
        }

        public string Print()
        {
            return Print(coord =>
            {
                var piece = At(coord);
                var rep = CardToRep(piece.Card);
                var black = piece.Owner == piece.Board.Arbiter.BlackPlayer;
                if (black)
                    rep = rep.ToLower();
                return piece == null ? "  " : rep;
            });
        }

        public string Print(Func<Coord, string> fun)
        {
            var sb = new StringBuilder();
            for (int y = Height - 1; y >= 0; --y)
            {
                // vertical axis
                sb.Append($" {y}:");

                // horizontal cards
                for (int x = 0; x < Width; ++x)
                {
                    var a = $"{fun(new Coord(x, y))}";
                    //Assert.AreEqual(2, a.Length);
                    sb.Append(a);
                }
                sb.AppendLine();
                if (y == 0)
                {
                    // write the bottom axis
                    sb.Append("   ");
                    for (int x = 0; x < Width; ++x)
                        sb.Append($"{x} ");
                }
            }
            return sb.ToString();
        }

        public string CardToRep(ICardModel model)
        {
            if (model == null) return "  ";
            var ch = $"{model.PieceType.ToString()[0]} ";
            return ch;
        }

        public IEnumerable<IPieceModel> DefendededCards(IPieceModel defender, Coord cood)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPieceModel> Defenders(Coord cood)
        {
            throw new NotImplementedException();
        }

        public IPieceModel GetContents(Coord coord)
        {
            return !IsValidCoord(coord) ? null : At(coord);
        }

        public IPieceModel At(Coord coord)
        {
            var valid = IsValidCoord(coord);
            Assert.IsTrue(valid);
            return !valid ? null : _contents[coord.y][coord.x];
        }

        public IPieceModel At(int x, int y)
        {
            if (x < 0 || y < 0)
                return null;
            if (x >= Width || y >= Height)
                return null;
            return _contents[y][x];
        }

        public bool IsValidCoord(Coord coord)
        {
            return coord.x >= 0 && coord.y >= 0 && coord.x < Width && coord.y < Height;
        }

        public IEnumerable<IPieceModel> GetContents()
        {
            return _contents.SelectMany(row => row).Where(c => c != null);
        }

        public IPieceModel PlaceCard(ICardModel card, Coord coord)
        {
            Info("BoardAgent: Placed {card.Owner.Color} {card} at {coord}");
            //_contents[coord.y][coord.x] = MakePiece(card, coord);
            return null;
        }

        public IEnumerable<Coord> GetMovements(Coord coord)
        {
            var card = _contents[coord.y][coord.x];
            return card == null ? null : Diagonals(coord);
        }
        #endregion

        #region Private Fields
        private List<List<IPieceModel>> _contents;
        static int Max(int a, int b) { return a > b ? a : b; }

        #endregion
    }
}
