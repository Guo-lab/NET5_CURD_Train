namespace ESC5.ValueObject
{
    public class OrderKey
    {
        public string OrderNo { get; protected set; }
        public int ItemNo { get; protected set; }
        public OrderKey(string orderNo, int itemNo)
        {
            this.OrderNo = orderNo;
            this.ItemNo = itemNo;
        }
    }
}
