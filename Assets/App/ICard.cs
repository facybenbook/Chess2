﻿using System.Collections.Generic;

// A card in a Hand or Deck or Collection
public interface ICard : IHasId
{
    string Name { get; }
    string Description { get; }
    int Attack { get; }
    int Health { get; }
    IList<IEffect> Effects { get; }
}