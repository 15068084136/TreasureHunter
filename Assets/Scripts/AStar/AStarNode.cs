
using System.Drawing;
/// <summary>
/// A*寻路节点类
/// </summary>
public class PointData
{
    public Point point;// 坐标
    public double g, h;// 评分
    public PointData parent;// 前一个节点
    public PointData(Point point, double g, double h, PointData front){
        this.point = point;
        this.g = g;
        this.h = h;
        this.parent = front;
    }

    public double F(){
        return g + h;
    }
}

public class Point{
    public int x, y;// 坐标

    public Point(int x, int y){
        this.x = x;
        this.y = y;
    }

    public new bool Equals(object obj){
        return (obj != null) && (obj is Point) && ((Point)obj).x == x && ((Point)obj).y == y;
    }
}
