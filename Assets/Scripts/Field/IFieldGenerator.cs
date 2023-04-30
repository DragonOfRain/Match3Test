using UnityEngine;

namespace Match3.Field
{
    public interface IFieldGenerator
    {
        ICell[,] Generate( Vector2Int fieldSize );
        ICell GenerateAtIndex( Vector2Int index, params Color[] restrictedColors );
    }
}