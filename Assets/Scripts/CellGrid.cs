using UnityEngine;

public class CellGrid
{
    // Khoi tao ngay(readonly)  mảng cells mang kiểu dữ liệu mảng 2 chiều ở lớp Cell ( chưa khai báo: chưa biết bao nhiêu phần tử, giá trị phần tử trong mang)
    
    private readonly Cell[,] cells;
    // Khởi tạo đọ rộng và dài của bảng 
    // int Width = cells.GetLength(0);(  độ rộng của bảng grid có kiểu dữ liệu thuộc lớp CellGrid = độ đai của mảng cells ở trên bắt đầu từ phần tử có chỉ số 0)
    public int Width => cells.GetLength(0);
    // int Height = cells.GetLength(1);(tương tự trên)
    public int Height => cells.GetLength(1);

    // khai báo và khoi tao 1 ô cell cụ thể có tọa độ(x,y) trong vecto khong gian oxy (this ánh xạ đối tượng hiện tại) có kiểu dữ liệu thuộc lớp Cell.
    //  biểu diễn ô cell trong mảng  Cell[,]cells.
    public Cell this[int x, int y] => cells[x, y];
    // Phương thức khởi tạo(Constructor) có tham chiếu của lớp CellGrid (tạo bảng)
    public CellGrid(int width, int height)
    {
        cells = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell
                {
                    // vị trí ô cell trong  bảng cell
                    position = new Vector3Int(x, y, 0),
                    // kiểu dữ liệu ô cell chứa.
                    type = Cell.Type.Empty
                };
            }
        }
    }
// Phương thức tạo ô bom 
    public void GenerateMines(Cell startingCell, int amount)
    {
        int width = Width;
        int height = Height;
        // amount ở hàm là số lượng bom, tạo bom bằng 
        for (int i = 0; i < amount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            Cell cell = cells[x, y];
            // Kt dk ô(cell) này kiểu bom và Ktra nếu ô(cell) này nếu liền kề với ô startingcell thì
            // thoát vòng lặp while gán luôn cell.type = Cell.Type.Mine;
            while (cell.type == Cell.Type.Mine || IsAdjacent(startingCell, cell))
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height) {
                        y = 0;
                    }
                }

                cell = cells[x, y];
                /* Ở đây nếu ta cần tránh game khi kích chuột vào ô đầu tiên trong bảng vào ô bom thì ta chỉ cần 
                sửa đảo ngược điều kiện đó là Mathf.Abs(a.position.x - b.position.x) >= 1 & Mathf.Abs(a.position.y - b.position.y) >= 1;
                ở hàm IsAdjacen. Và ta thêm code dưới đây và xóa theo hướng dẫn.
                 cell.type = Cell.Type.Mine;
                */
            }
            // Xóa ở đây( theo hướng dẫn xóa cell.type = Cell.Type.Mine;)
            cell.type = Cell.Type.Mine;
        }
    }
// Phuong thuc tao o number
    public void GenerateNumbers()
    {
        int width = Width;
        int height = Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
                // Nếu cell có kiểu mìn thì tiếp tục vòng lặp for
                if (cell.type == Cell.Type.Mine) {
                    continue;
                }

                cell.number = CountAdjacentMines(cell);
                cell.type = cell.number > 0 ? Cell.Type.Number : Cell.Type.Empty;
            }
        }
    }
    // Phương thức tạo logic ô kề cận quả bom đánh số ô cạnh quả bom trả về 1 số int( Vd: 1 ô cạnh 2 quả bom trả giá trị số int = 2)

    public int CountAdjacentMines(Cell cell)
    {
        int count = 0;
        //ở đây ta Giả sử chúng ta có ô hiện tại có giá trị cell(0,0). Vậy các ô liền kề của nó là:
        // cell(-1,1); cell(-1,0); cell(-1,1); cell(0,-1); cell(0,1); cell(1,-1); cell(1,0); cell(1,1).
        // Ở đây ta dùng 2 hàm for duyệt mảng ô liền kề với ô bom. 
        // Từ đấy lấy ra các ô liền kề.
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }
                // Ta sử dụng tính chất vecto cộng vecto 0 để tìm vị trí thực tế của ô hiện tại và cũng như là các ô liền kề thì thực hiện cộng hai vecto
                int x = cell.position.x + adjacentX;
                int y = cell.position.y + adjacentY;
                // kiểm tra hàm TryGetCell(trả về true)( lấy giá trị ô cell = ô cell của ô bom ( vị trí thực tế ô bom) và kiểu ô liền kề bom chính là ô bom)
                if (TryGetCell(x, y, out Cell adjacent) && adjacent.type == Cell.Type.Mine) {
                   // Tăng số number gần bom
                    count++;
                }
            }
        }

        return count;
    }
    // Hàm đếm số cờ được gắn ( ko đúng )
    public int CountAdjacentFlags(Cell cell)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }

                int x = cell.position.x + adjacentX;
                int y = cell.position.y + adjacentY;

                if (TryGetCell(x, y, out Cell adjacent) && !adjacent.revealed && adjacent.flagged) {
                    count++;
                }
            }
        }

        return count;
    }

    public Cell GetCell(int x, int y)
    {
        if (InBounds(x, y)) {
            return cells[x, y];
        } else {
            return null;
        }
    }
    // hàm lấy giá trị ô cell 
    public bool TryGetCell(int x, int y, out Cell cell)
    {
        cell = GetCell(x, y);
        return cell != null;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsAdjacent(Cell a, Cell b)
    {
        /*Kiem tra 2 ô này nếu giá trị a.position.x - b.position.x <=1, Mathf.Abs(a.position.y - b.position.y) <= 1 đồng thời xảy ra
        tức là 2 ô này ( startingCell và ô cell bất kỳ ta vừa lấy ở hàm GenerateMines), (starrtingCell là ô đầu tiên ta kích vào bảng dò bom))
        Do vị trí của 2 ô là kiểu int nên giá trị <=1 là 2 giá trị 0 và 1. 
        Với giá trị 0 ô startingCell và ô cell bất kỳ ta vừa lấy ở hàm GenerateMines là trùng nhau tức là cùng 1 ô, còn trường hợp bằng 1 thì 
        đây là ô liền kề ô startingCell. Ở đây nếu ta cần tránh game khi kích chuột vào ô đầu tiên trong bảng vào ô bom thì ta chỉ cần 
        sửa đảo ngược điều kiện đó là Mathf.Abs(a.position.x - b.position.x) >= 1 & Mathf.Abs(a.position.y - b.position.y) >= 1;
        */
        return Mathf.Abs(a.position.x - b.position.x) <= 1 &&
               Mathf.Abs(a.position.y - b.position.y) <= 1;
    }

}
