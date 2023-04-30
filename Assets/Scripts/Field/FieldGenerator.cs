using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Field
{
    public sealed class FieldGenerator : IFieldGenerator
    {
        private List< Color > _availableColors = new()
        {
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow
        };

        public ICell[,] Generate( Vector2Int fieldSize )
        {
            ICell[,] result = new Cell[fieldSize.x, fieldSize.y];
            
            Random.InitState( 0 );
            
            for ( int i = 0; i < fieldSize.x; i++ )
            {
                for ( int j = 0; j < fieldSize.y; j++ )
                {
                    Color[] restrictedColors = GetRestrictedColor( result, i, j );
                    result[ i, j ] = GenerateAtIndex( i, j, restrictedColors );
                }
            }

            return result;
        }

        public ICell GenerateAtIndex( int x, int y, params Color[] restrictedColors ) 
            => GenerateAtIndex( new Vector2Int( x, y ), restrictedColors );
        
        public ICell GenerateAtIndex( Vector2Int index, params Color[] restrictedColors )
        {
            if ( _availableColors.Count == 0 ) throw new Exception( $@"[{GetType().Name}] Empty color list." );
            
            List< Color > availableColors = new List< Color >( _availableColors );
            foreach ( var restrictedColor in restrictedColors )
            {
                availableColors.Remove( restrictedColor );
            }

            int colorIndex = Random.Range( 0, availableColors.Count );

            return new Cell( index, availableColors[ colorIndex ] );
        }

        public Color[] GetRestrictedColor( ICell[,] cells, int x, int y )
        {
            List< Color > restricted = new List< Color >( 8 );
            
            ICell cell1 = x > 0 ? cells[ x - 1, y ] : null;
            ICell cell2 = x > 1 ? cells[ x - 2, y ] : null;

            if ( CompareCellColor( cell1, cell2 ) )
            {
                restricted.Add( cell1.Color );
            }
            
            cell1 = x < cells.GetLength( 0 ) - 1 ? cells[ x + 1, y ] : null;
            cell2 = x < cells.GetLength( 0 ) - 2 ? cells[ x + 2, y ] : null;
            
            if ( CompareCellColor( cell1, cell2 ) )
            {
                restricted.Add( cell1.Color );
            }
            
            cell1 = y > 0 ? cells[ x, y - 1 ] : null;
            cell2 = y > 1 ? cells[ x, y - 2 ] : null;
            
            if ( CompareCellColor( cell1, cell2 ) )
            {
                restricted.Add( cell1.Color );
            }
            
            cell1 = y < cells.GetLength( 1 ) - 1 ? cells[ x, y + 1 ] : null;
            cell2 = y < cells.GetLength( 1 ) - 2 ? cells[ x, y + 2 ] : null;
            
            if ( CompareCellColor( cell1, cell2 ) )
            {
                restricted.Add( cell1.Color );
            }

            return restricted.ToArray();
        }

        private bool CompareCellColor( ICell cell1, ICell cell2 )
        {
            if ( cell1 == null || cell2 == null ) return false;

            return cell1.Color == cell2.Color;
        }
    }
}