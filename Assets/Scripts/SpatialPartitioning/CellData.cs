
public class CellData
{
    // a class reference for UnitData
    public UnitData UnitData { get; private set; }

    // the index of it's current cell
    public int x, y;

    // The reference to previous and next cell data
    // intended to be used for a linked list
    public CellData prev, next;

    public CellData(UnitData unitData, int x, int y, CellData prev, CellData next)
    {
        this.UnitData = unitData;
        this.x = x;
        this.y = y;
        this.prev = prev;
        this.next = next; 
    
    }
}
