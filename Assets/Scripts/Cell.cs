using UnityEngine;
// Khai báo các kiểu dữ liệu trong game
public class Cell
{
    // kiểu dữ liệu tự định nghĩa: khai báo biến kiểu Type
    public enum Type
    {
        Empty,
        Mine,
        Number,
    }
    // vị trí của cell trên không gian xyz( z=0 nên mặt phẳng 2 chiều)
    public Vector3Int position;
    // biến cụ thể mô tả biến type cụ thể được tham chiếu kiểu dữ liệu Type.
    public Type type;
    // kiểu dữ liệu int của biến number 
    public int number;
    // kiểu dữ liệu biến mô tả đã kích chuột vào ô hay chưa
    public bool revealed;
    // kiểu dữ liệu có cờ hay không
    public bool flagged;
    // kiểu dữ liệu kích hoạt nổ của bom có hay không
    public bool exploded;
    // kieu du lieu mở  dây ô chưa mở
    public bool chorded;
}
