using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Field
{
    public sealed class CellGrid
    {
        public event Action OnFieldChanged;
        
        public Cell[,] Cells { get; private set; }
        
        private IFieldGenerator _generator;
        private int _maxHeight;
        private List< Cell > _destroyedCells = new List< Cell >();

        private Vector2Int? _lastSelected;

        public CellGrid( Vector2Int fieldSize, IFieldGenerator generator )
        {
            _generator = generator ?? throw new ArgumentNullException( nameof(generator) );
            _maxHeight = fieldSize.y;

            Cells = (Cell[,])_generator.Generate( fieldSize );
        }

        public void SelectCell( Vector2Int index )
        {
            if ( _lastSelected.HasValue )
            {
                Cells[ _lastSelected.Value.x, _lastSelected.Value.y ].SetSelected( false );
                
                if ( _lastSelected.Value.Equals( index ) )
                {
                    _lastSelected = null;
                }
                else if ( CheckPossibilityToSwap( _lastSelected.Value, index ) )
                {
                    SwapCells( _lastSelected.Value, index );
                    CheckMatches( _lastSelected.Value, index );
                    _lastSelected = null;
                    OnFieldChanged?.Invoke();
                }
            }
            else
            {
                _lastSelected = index;
                Cells[ index.x, index.y ].SetSelected( true );
            }
        }

        private void CheckMatches( Vector2Int cell1, Vector2Int cell2 )
        {
            if ( TryGetThreeInRow( Cells, cell1, out var matches ) )
            {
                DestroyMatchedCells( matches );
            }
            
            if ( TryGetThreeInRow( Cells, cell2, out matches ) )
            {
                DestroyMatchedCells( matches );
            }

            RefillCells();
        }

        private void RefillCells()
        {
            foreach ( var destroyedCell in _destroyedCells )
            {
                Cells[ destroyedCell.Index.x, destroyedCell.Index.y ] = (Cell)_generator.GenerateAtIndex( destroyedCell.Index );
            }
            
            _destroyedCells.Clear();
        }

        private void DestroyMatchedCells( List< Cell > matchCells )
        {
            foreach ( var cell in matchCells )
            {
                cell.Destroy();
                _destroyedCells.Add( cell );
            }

            MoveDestroyedCells( matchCells );
        }

        private void MoveDestroyedCells( List< Cell > matchCells )
        {
            matchCells.Sort( (c1, c2) => c2.Index.y - c1.Index.y );
            List< Cell > cellsToCheck = new List< Cell >();

            foreach ( var cell in matchCells )
            {
                if ( cell.Index.y >= _maxHeight ) 
                    continue;
                
                while ( true )
                {
                    if ( cell.Index.y >= _maxHeight - 1) 
                        break;
                        
                    Cell upperCell = Cells[ cell.Index.x, cell.Index.y + 1 ];
                        
                    if ( upperCell.IsDestroyed ) 
                        break; 
                            
                    SwapCells( cell.Index, upperCell.Index );

                    if ( !cellsToCheck.Contains( upperCell ) )
                    {
                        cellsToCheck.Add( upperCell );
                    }
                }
            }

            foreach ( var cellToCheck in cellsToCheck )
            {
                if ( TryGetThreeInRow( Cells, cellToCheck.Index, out var matches ) )
                {
                    DestroyMatchedCells( matches );
                }
            }
        }

        private bool CheckPossibilityToSwap( Vector2Int cell1, Vector2Int cell2 ) 
            => Math.Abs( cell1.x - cell2.x ) == 1 || Math.Abs( cell1.y - cell2.y ) == 1;

        private void SwapCells( Vector2Int cell1, Vector2Int cell2 )
        {
            Cell savedCell = Cells[ cell1.x, cell1.y ];
            savedCell.Index = cell2;
            Cells[ cell1.x, cell1.y ] = Cells[ cell2.x, cell2.y ];
            Cells[ cell1.x, cell1.y ].Index = cell1;
            Cells[ cell2.x, cell2.y ] = savedCell;
        }

        private bool TryGetThreeInRow( Cell[,] cells, Vector2Int targetCell, out List< Cell > matchCells )
        {
            matchCells = default;
         
            Cell cellCentral = cells[ targetCell.x, targetCell.y ];
            
            // Horizontal
            
            Cell cellPrev1 = targetCell.x > 0 ? cells[ targetCell.x - 1, targetCell.y ] : null;
            Cell cellPrev2 = targetCell.x > 1 ? cells[ targetCell.x - 2, targetCell.y ] : null;

            if ( CompareCellColor( cellPrev1, cellPrev2, cellCentral ) )
            {
                matchCells = new List< Cell >
                {
                    cellPrev1,
                    cellPrev2,
                    cellCentral
                };

                return true;
            }
            
            Cell cellNext1 = targetCell.x < cells.GetLength( 0 ) - 1 ? cells[ targetCell.x + 1, targetCell.y ] : null;
            Cell cellNext2 = targetCell.x < cells.GetLength( 0 ) - 2 ? cells[ targetCell.x + 2, targetCell.y ] : null;
            
            if ( CompareCellColor( cellNext1, cellNext2, cellCentral ) )
            {
                matchCells = new List< Cell >
                {
                    cellNext1,
                    cellNext2,
                    cellCentral
                };

                return true;
            }

            if ( CompareCellColor( cellPrev1, cellCentral, cellNext1 ) )
            {
                matchCells = new List< Cell >
                {
                    cellPrev1,
                    cellNext1,
                    cellCentral
                };

                return true;
            }
            
            // Vertical
            
            cellPrev1 = targetCell.y > 0 ? cells[ targetCell.x, targetCell.y - 1 ] : null;
            cellPrev2 = targetCell.y > 1 ? cells[ targetCell.x, targetCell.y - 2 ] : null;
            
            if ( CompareCellColor( cellPrev1, cellPrev2, cellCentral ) )
            {
                matchCells = new List< Cell >
                {
                    cellPrev1,
                    cellPrev2,
                    cellCentral
                };

                return true;
            }
            
            cellNext1 = targetCell.y < cells.GetLength( 1 ) - 1 ? cells[ targetCell.x, targetCell.y + 1 ] : null;
            cellNext2 = targetCell.y < cells.GetLength( 1 ) - 2 ? cells[ targetCell.x, targetCell.y + 2 ] : null;
            
            if ( CompareCellColor( cellNext1, cellNext2, cellCentral ) )
            {
                matchCells = new List< Cell >
                {
                    cellNext1,
                    cellNext2,
                    cellCentral
                };

                return true;
            }
            
            if ( CompareCellColor( cellPrev1, cellCentral, cellNext1 ) )
            {
                matchCells = new List< Cell >
                {
                    cellPrev1,
                    cellNext1,
                    cellCentral
                };

                return true;
            }

            return false;
        }
        
        private bool CompareCellColor( ICell cell1, ICell cell2, ICell cell3 )
        {
            if ( cell1 == null || cell2 == null || cell3 == null ) return false;

            return cell1.Color == cell2.Color && cell2.Color == cell3.Color;
        }
    }
}