
using UnityEngine;

public struct Cell 
{
    // Các kiểu Thuộc tính ô cell như:  kiểu rỗng, mìn, số
   public enum Type
    {
        Empty,
        Mine,
        Number,
    }
    // Vị trí của ô cell biểu diễn bằng số nguyên( tọa độ x,y,z là số nguyên)
    public Vector3Int position;
    // Thuộc tính ô cell cụ thể
    public Type type;
    // Thuộc tính number của ô cell thuộc kiểu dữ liệu int( khai báo dữ liệu thuộc tính number)
    public int number;
    // kiểm tra trạng thái ô được nhấp vào hay chưa
    public bool revealed;
    // kiểm tra trạng thái ô gắn cờ hay chưa
    public bool flagaged;
    // Kiểm tra ô đã nổ hay chưa
    public bool exploded;
}
