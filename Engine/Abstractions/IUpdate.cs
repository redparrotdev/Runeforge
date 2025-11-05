using Microsoft.Xna.Framework;
using System;

namespace Engine.Abstractions;

public interface IUpdate
{
    bool IsActive { get; set; }
    int UpdateOrder { get; set; }

    event Action<IUpdate> OnActiveChanged;
    event Action<IUpdate> OnUpdateOrderChanged;

    void Update(GameTime gameTime);
}
