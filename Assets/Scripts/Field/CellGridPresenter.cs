using UnityEngine;

namespace Match3.Field
{
    public sealed class CellGridPresenter : MonoBehaviour
    {
        [SerializeField] private float _cellOffset;
        [SerializeField] private CellPresenter _cellPrefab;
        
        private CellGrid _cellGrid;
        private CellPresenter[,] _cells;

        private Vector2 _cellSize;
        
        public void Init( CellGrid cellGrid )
        {
            _cellGrid = cellGrid;
            Vector2 ownSize = ((RectTransform)transform).rect.size;

            int gridWidth = cellGrid.Cells.GetLength( 0 );
            int gridHeight = cellGrid.Cells.GetLength( 1 );
            
            float maxCellWidth = (ownSize.x - _cellOffset * gridWidth) / gridWidth;
            float maxCellHeight = (ownSize.y - _cellOffset * gridHeight) / gridHeight;
            float cellSize = Mathf.Min( maxCellWidth, maxCellHeight );
            _cellSize = new Vector2( cellSize, cellSize );
            
            _cells = new CellPresenter[gridWidth, gridHeight];

            _cellGrid.OnFieldChanged += Render;
            
            Render();
        }

        private void OnDestroy() => _cellGrid.OnFieldChanged -= Render;

        private void Render()
        {
            Vector2Int indexOffset = new Vector2Int(
                _cellGrid.Cells.GetLength( 0 ) / 2,
                _cellGrid.Cells.GetLength( 1 ) / 2
            );

            for ( var i = 0; i < _cellGrid.Cells.GetLength( 0 ); i++ )
            {
                for ( var j = 0; j < _cellGrid.Cells.GetLength( 1 ); j++ )
                {
                    Cell cell = _cellGrid.Cells[ i, j ];

                    CellPresenter cellPresenter = _cells[ i, j ] == null ? 
                        CreateCell() 
                        : _cells[ i, j ];

                    var position = new Vector2(
                        (i - indexOffset.x + .5f) * (_cellSize.x + _cellOffset),
                        (j - indexOffset.y + .5f) * (_cellSize.y + _cellOffset)
                    );
                    
                    cellPresenter.Init( position, cell, _cellSize );
                    _cells[ i, j ] = cellPresenter;
                }
            }
        }

        private CellPresenter CreateCell()
        {
            CellPresenter cell = Instantiate( _cellPrefab, transform );
            cell.OnClicked += OnCellClickedHandler;
            return cell;
        }

        private void OnCellClickedHandler( Vector2Int index ) => _cellGrid.SelectCell( index );
    }
}