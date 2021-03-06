﻿using App.Common;

namespace App.View
{
    using Agent;

    public interface IArbiterView
        : IView<IArbiterAgent>
    {
        IBoardView BoardView { get; }
        IPlayerView CurrentPlayerView { get; }
        EColor CurrentPlayerColor { get; }

        bool CurrentPlayerOwns(IOwned owned);
    }
}
