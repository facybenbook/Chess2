﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using App.Model;

namespace App
{
    using Action;

    [TestFixture]
    public class TestBoard
    {
        [TestCase(4,4)]
        [TestCase(8,8)]
        public void TestCreate(int width, int height)
        {
            var board = new Model.Board();
            board.Create(width, height);
            Assert.AreEqual(board.Width, width);
            Assert.AreEqual(board.Height, height);
            Assert.IsTrue(board.IsValidCoord(new Coord(0, 0)));
            Assert.IsTrue(board.IsValidCoord(new Coord(width - 1, height - 1)));
            Assert.IsFalse(board.IsValidCoord(new Coord(width, height)));

            var squares = 0;
            foreach (var square in board.GetContents())
            {
                Assert.IsNull(square);
                ++squares;
            }
            Assert.AreEqual(squares, width * height);
        }
    }
}
