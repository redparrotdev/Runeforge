using Microsoft.Xna.Framework;
using System;

namespace Engine.Abstractions.Logic;

public interface IUpdate
{
    bool IsActive { get; set; }

    event Action<IUpdate> OnActiveChanged;

    void Update(GameTime gameTime);
}
