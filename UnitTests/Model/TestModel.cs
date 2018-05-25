﻿using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace App.Model.Test
{
    [TestFixture]
    class TestBoard : TestBase
    {
        [Test]
        public void TestBoardCreation()
        {
            Assert.IsNotNull(_board);
            Assert.IsTrue(_board.Width == 8);
            Assert.IsTrue(_board.Height == 8);
            Assert.IsTrue(!_board.Pieces.Any());

            Assert.IsNotNull(_arbiter);
            Assert.AreSame(_reg.New<IBoardModel>(), _board);
            Assert.AreSame(_reg.New<IArbiterModel>(), _board.Arbiter);
            Assert.AreSame(_reg.New<IArbiterModel>(), _arbiter);

            Assert.IsNotNull(_white);
            Assert.IsNotNull(_black);

            _arbiter.NewGame(_white, _black);
            Assert.AreSame(_white.Arbiter, _black.Arbiter);
            Assert.AreSame(_white.Board, _black.Board);
            Assert.AreSame(_white.Board.Arbiter.CurrentPlayer, _black.Board.Arbiter.Board.Arbiter.CurrentPlayer);
        }

        [Test]
        public void TestBasicTurns()
        {
            _arbiter.NewGame(_white, _black);
            for (var n = 0; n < 4; ++n)
            {
                Assert.IsTrue(_arbiter.Arbitrate(_white.NextAction()).Success);
                Assert.IsTrue(_arbiter.Arbitrate(_black.NextAction()).Success);
                Trace.WriteLine(_board.Print());
            }
        }
    }
}
