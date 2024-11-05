using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{
    // khai báo biến Tham chiếu bản đồ ô chứa chỉ tham chiếu không cần gán dữ liệu
    public Tilemap tilemap { get; private set; }

    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;

    private void Awake()
    {
        // Tìm và gán vào biến có thành phần kiểu dữ liệu Tilemap vào biến tham chiếu tilemap ở trên 
        tilemap = GetComponent<Tilemap>();
    }

    public void Draw(CellGrid grid)
    {
        // Chieu rong va chieu dai cua bảng tilemap grid trong lớp CellGrid bằng chiều rộng và dài mảng cells 
        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }
    // Phương thức GetTile thuộc (lớp Tile trong TileMap(mặc định trong UnityEngine.Tilemaps)) 
    // Tham chiếu ô cell trong lớp Cell vào đối tướng Tile trong lớp TileMap 
    private Tile GetTile(Cell cell)
    {
        // Nếu ô Cell  kích hoạt (true) trả về  GetRevealedTile(cell)
        if (cell.revealed) {
            return GetRevealedTile(cell);
        } else if (cell.flagged) {
            return tileFlag;
        } else if (cell.chorded) {
            return tileEmpty;
        } else {
            return tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        #region Switch ... case

          /* 
           * switch (<biểu thức>)
            {
            case < giá trị thứ 1 >: < câu lệnh thứ 1 >;
            break;
            case < giá trị thứ 2 >: < câu lệnh thứ 2 >;
            break;
            . . .
            case < giá trị thứ n >: < câu lệnh thứ n >;
            break;
             } 
          */
            #endregion
        // tham chiếu đến kiểu dữ liệu của ô cell( ktra kiểu dữ liệu của ô cell là kiểu gì và trả về kiểu đó)
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }
// phương thức trả về number
    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }

}
