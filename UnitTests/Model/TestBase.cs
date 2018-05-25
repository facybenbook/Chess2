﻿using NUnit.Framework;

namespace App.Model.Test
{
    using Registry;

    [TestFixture]
    class TestBase : Flow.Impl.Logger
    {
        protected IRegistry<IModel> _reg;
        protected IBoardModel _board;
        protected IPlayerModel _white;
        protected IPlayerModel _black;
        protected IArbiterModel _arbiter;

        // this is only place concrete types are needed
        private void PrepareBindings()
        {
            _reg = new Registry<IModel>();
            _reg.Bind<Service.ICardTemplateService, Service.Impl.CardTemplateService>();
            _reg.Bind<IBoardModel, BoardModel>(new BoardModel(8, 8));
            _reg.Bind<IArbiterModel, ArbiterModel>(new ArbiterModel());
            _reg.Bind<IWhitePlayer, WhitePlayer>();
            _reg.Bind<IBlackPlayer, BlackPlayer>();
            _reg.Bind<ICardModel, CardModel>();
            _reg.Bind<IDeckModel, MockDeck>();
            _reg.Bind<IHandModel, MockHand>();
            _reg.Bind<IPieceModel, PieceModel>();
            _reg.Resolve();
        }

        [SetUp]
        public void Setup()
        {
            PrepareBindings();

            _board = _reg.New<IBoardModel>();
            _arbiter = _reg.New<IArbiterModel>();
            _white = _reg.New<IWhitePlayer>();
            _black = _reg.New<IBlackPlayer>();
        }

        [TearDown]
        public void TearDown()
        {
            _arbiter.Destroy();
            _white.Destroy();
            _black.Destroy();
            _board.Destroy();
        }
    }
}
