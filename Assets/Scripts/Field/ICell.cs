using System;
using UnityEngine;

namespace Match3.Field
{
    public interface ICell
    {
        event Action< bool > OnSelectChanged;
        event Action OnDestroy;
        Vector2Int Index { get; }
        Color Color { get; }
        bool IsSelected { get; }
        bool IsDestroyed { get; }
    }
}