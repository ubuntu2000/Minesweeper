using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Game : MonoBehaviour
{
    // Khai báo chiều rộng của bảng = 16
    public int width = 16;
    // tương tự như trên
    public int height = 16;
    // số bom 32
    public int mineCount = 32;
    // khai báo biến bảng 
    private Board board;
    // khai báo bảng các ô cell 
    private CellGrid grid;
    // trạng thái gameover
    private bool gameover;
    // trạng thái biến tạo ra
    private bool generated;

    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        StopAllCoroutines();
        // cammera vị trí /2 khung hình.
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        gameover = false;
        generated = false;
        // Tạo bảng cell có chiều rộng và dài= 16
        grid = new CellGrid(width, height);
        // vẽ bảng tham chiếu grid trong lớp Board.
        board.Draw(grid);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            NewGame();
            return;
        }

        if (!gameover)
        {
            if (Input.GetMouseButtonDown(0)) {
                Reveal();
            } else if (Input.GetMouseButtonDown(1)) {
                Flag();
            } else if (Input.GetMouseButton(2)) {
                Chord();
            } else if (Input.GetMouseButtonUp(2)) {
                Unchord();
            }
        }
    }
    // Phuong thuc  ô chưa được mở
    private void Reveal()
    {
        // ktra  nếu mà kích chuột trong grid thì tiếp tục chạy vòng if
        if (TryGetCellAtMousePosition(out Cell cell))
        {
            // Nếu mìn và số ở bảng chưa được tạo ra (generate),  gọi 1 lần hàm tạo bom và tạo số trong game
            if (!generated)
            {
                // Tạo Bom 
                grid.GenerateMines(cell, mineCount);
                // Tạo number
                grid.GenerateNumbers();
                // Gán giá trị biến tạo ra = true sẽ không gọi thêm nữa
                generated = true;
            }
            // Gọi hàm mở ô
            Reveal(cell);
        }
    }
    // Phương thức(hàm) mở ô
    private void Reveal(Cell cell)
    {
        // Neu ô mở rồi thì return
        if (cell.revealed) return;
        // Neu ô gan co rồi thì return
        if (cell.flagged) return;

        switch (cell.type)
        {
            case Cell.Type.Mine:
                //  gọi hàm nổ bom
                Explode(cell);
                break;
                // hai Trường hợp còn lại mở ô empty và ô number(default) gọi hàm kt thắng cuộc
            case Cell.Type.Empty:
                StartCoroutine(Flood(cell));
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                CheckWinCondition();
                break;
        }
        // Cập nhật bảng 
        board.Draw(grid);
    }
    //  Phuong thức Lấp đầy các ô
    private IEnumerator Flood(Cell cell)
    {
        // nếu gameover thoát vòng lặp
        if (gameover) yield break;
        // Nếu gặp các ô đã mở rồi thoát khỏi vòng lặp
        if (cell.revealed) yield break;
        // Nếu gặp ô chứa bom ( tất nhiên là chưa mở vì mở thì gameover) thoát vòng lặp sẽ ko mở ô bom ra
        if (cell.type == Cell.Type.Mine) yield break;
        // Trạng thái ô hiện tại là true( mở ra)
        cell.revealed = true;
        // Cập nhật bảng
        board.Draw(grid);
        // 
        yield return null;
        // nếu trường hợp ô hiện tại = empty
        if (cell.type == Cell.Type.Empty)
        {
            // Thì tham chiếu ô hiện tại gán vào ô bên trái nó. Và chạy  StartCoroutine(Flood(left)) ( chạy tiếp ô bên trái nó với hàm Flood). 
            // Tiếp tục chạy đến khi gặp các trường hợp thoát khỏi vòng lặp thì dừng.
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y, out Cell left)) {
                StartCoroutine(Flood(left));
            }
            // tương tự 
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y, out Cell right)) {
                StartCoroutine(Flood(right));
            }
            // Tương tự
            if (grid.TryGetCell(cell.position.x, cell.position.y - 1, out Cell down)) {
                StartCoroutine(Flood(down));
            }
            // Tương tự
            if (grid.TryGetCell(cell.position.x, cell.position.y + 1, out Cell up)) {
                StartCoroutine(Flood(up));
            }
        }
    }

    private void Flag()
    {
        // nếu không kích ô trên bảng ô thì thoát lệnh
        if (!TryGetCellAtMousePosition(out Cell cell)) return;
        // Nếu ô đã kích hoạt rồi thì thoát lệnh
        if (cell.revealed) return;
        // trạng thái ô cờ ( có cờ hoặc ko có cờ)
        cell.flagged = !cell.flagged;
        // Vẽ cờ
        board.Draw(grid);
    }
    // Phương thuc kich chuot  khong mo ô(cell) tạo hieu ung an mo xung quanh
    private void Chord()
    {
        // tạo ô không ở vào đầu game
        // unchord previous cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].chorded = false;
            }
        }

        // chord new cells
        if (TryGetCellAtMousePosition(out Cell chord))
        {
            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    int x = chord.position.x + adjacentX;
                    int y = chord.position.y + adjacentY;

                    if (grid.TryGetCell(x, y, out Cell cell)) {
                        cell.chorded = !cell.revealed && !cell.flagged;
                    }
                }
            }
        }

        board.Draw(grid);
    }

    private void Unchord()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.chorded) {
                    Unchord(cell);
                }
            }
        }

        board.Draw(grid);
    }

    private void Unchord(Cell chord)
    {
        chord.chorded = false;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }

                int x = chord.position.x + adjacentX;
                int y = chord.position.y + adjacentY;

                if (grid.TryGetCell(x, y, out Cell cell))
                {
                    if (cell.revealed && cell.type == Cell.Type.Number)
                    {
                        if (grid.CountAdjacentFlags(cell) >= cell.number)
                        {
                            Reveal(chord);
                            return;
                        }
                    }
                }
            }
        }
    }
    // Phương thức nổ bom
    private void Explode(Cell cell)
    {
        gameover = true;
        // trạng thái biến nổ bom = true
        // trạng thái mở tất cả các ô = true
        // Set the mine as exploded
        cell.exploded = true;
        cell.revealed = true;
        // mở tất cả ô bom
        // Reveal all other mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.revealed = true;
                }
            }
        }

    }
    // Phuuwong thức kiểm tra thang hay ko
    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                // Nếu các ô hiện tại đang trong trạng thái  chưa được mở
                // (Nếu là ô hiện tại là ô empty và ô number) chưa  được mở và ô hiện tại là ô empty và ô number không phải là ô bom thì no win 
                // All non-mine cells must be revealed to have won
                if (cell.type != Cell.Type.Mine && !cell.revealed) {
                    return; // no win
                }
            }
        }

        // gameover = true;
        // Ktra ô gắn cờ có gắn đúng ô bom không?
        // Flag all the mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.flagged = true;
                }
            }
        }
    }

    private bool TryGetCellAtMousePosition(out Cell cell)
    {
        //Chuyển đổi vị trí chuột trong không gian trở thành vị trí chuột trên màn hình
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vị trí ô hiện tại = vị trí trên màn hình chuyển đổi vị trí trên không gian
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        // trả về vị trí ô cell trên bảng(grid)
        return grid.TryGetCell(cellPosition.x, cellPosition.y, out cell);
    }

}
