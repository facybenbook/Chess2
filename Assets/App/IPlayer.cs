﻿using Flow;

namespace App
{
    public interface IPlayer
    {
        EColor Color { get; }
        Flow.IFuture<int> RollDice();
        void AddMaxMana(int mana);
        IFuture<Arbiter.PlayCard> TryPlayCard();
        IFuture<Arbiter.MovePiece> TryMovePiece();
        IFuture<bool> Pass();
    }
}